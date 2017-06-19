using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Concurrent;

namespace ShiftOS.WinForms.Applications
{
    [WinOpen("{WO_MINDBLOW}")]
    [Launcher("{TITLE_MINDBLOW}", false, null, "{AL_PROGRAMMING}")]
    [RequiresUpgrade("mindblow")]
    public partial class MindBlow : UserControl, IShiftOSWindow, IBFListener
    {
        private bool running = true;
        private Action updateMemPtr = null, updateIPtr = null;
        private Action[] updateMem = new Action[30000];
        private AutoResetEvent evwaiting = new AutoResetEvent(false);
        private object evlock = new object();

        private class TextBoxStream : Stream
        {
            private TextBox box;
            private KeysConverter kc;
            public KeyPressEventHandler handler;
            public TextBoxStream(TextBox mybox)
            {
                kc = new KeysConverter();
                box = mybox;
            }
            public override bool CanRead { get { return true; } }

            public override bool CanSeek { get { return false; } }

            public override bool CanWrite { get { return true; } }

            public override long Length { get { return box.Text.Length; } }

            public override long Position
            {
                get
                {
                    return Length;
                }
                set
                {
                    //nothing
                }
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int ptr = offset;
                var hnd = new AutoResetEvent(false);
                handler = (o, a) =>
                {
                    if (ptr < offset + count)
                    {
                        buffer[ptr] = (byte)a.KeyChar;
                        ptr++;
                    }
                    if (ptr >= offset + count)
                        hnd.Set();
                };
                Desktop.InvokeOnWorkerThread(() => box.KeyPress += handler);
                hnd.WaitOne();
                Desktop.InvokeOnWorkerThread(() => box.KeyPress -= handler);
                return count;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                string output = "";
                foreach (byte b in buffer.Skip(offset).Take(count))
                    output += Convert.ToChar(b);
                Desktop.InvokeOnWorkerThread(() => box.Text += output);
            }
        }
        private static string[] DefaultMem;
        private BFInterpreter Interpreter;
        private Thread InterpreterThread;
        private TextBoxStream consolestr;
        private void DoLayout()
        {
            memlist.Left = 0;
            memlist.Width = monitor.Width;
            memlist.Height = monitor.Height - memlist.Top;
            program.Top = 0;
            program.Left = 0;
            programinput.Width = program.Width;
            programinput.Height = program.Height - load.Height;
            load.Top = save.Top = run.Top = stop.Top = programinput.Height;
            load.Width = save.Width = run.Width = stop.Width = save.Left = program.Width / 4;
            load.Left = 0;
            run.Left = save.Left * 2;
            stop.Left = save.Left * 3;
        }

        public MindBlow()
        {
            InitializeComponent();
            DefaultMem = new string[30000];
            for (ushort i = 0; i < 30000; i++)
                DefaultMem[i] = "0";
            memlist.Items.AddRange(DefaultMem);
            consolestr = new TextBoxStream(consoleout);
            Interpreter = new BFInterpreter(consolestr, this);
        }

        public void IPtrMoved(int newval)
        {
            lock (evlock)
            {
                updateIPtr = () => instptr.Text = "Instruction: " + newval.ToString();
                evwaiting.Set();
            }
        }

        public void MemChanged(ushort pos, byte newval)
        {
            lock (evlock)
            {
                updateMem[pos] = () =>
                {
                    memlist.Items[pos] = newval.ToString();
                };
                evwaiting.Set();
            }
        }

        public void MemReset()
        {
            Desktop.InvokeOnWorkerThread(() =>
            {
                memlist.Items.Clear();
                memlist.Items.AddRange(DefaultMem);
            });
        }

        public void OnLoad()
        {
            DoLayout();
            new Thread(() =>
            {
                while (running)
                {
                    evwaiting.WaitOne();
                    try
                    {
                        lock (evlock)
                        {
                            if (updateIPtr != null)
                            {
                                Desktop.InvokeOnWorkerThread(updateIPtr);
                                updateIPtr = null;
                            }
                            if (updateMemPtr != null)
                            {
                                Desktop.InvokeOnWorkerThread(updateMemPtr);
                                updateMemPtr = null;
                            }
                            for (int i = 0; i < updateMem.Length; i++)
                                if (updateMem[i] != null)
                                {
                                    Desktop.InvokeOnWorkerThread(updateMem[i]);
                                    updateMem[i] = null;
                                }
                        }
                    } catch { }
                    evwaiting.Reset();
                }
            }).Start();
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            running = false;
            evwaiting.Set(); // allow the display loop to die of old age
            KillThread(); // mercilessly slaughter the interpreter thread
            return true;
        }

        public void OnUpgrade()
        {
        }

        public void PointerMoved(ushort newval)
        {
            lock (evlock)
            {
                updateMemPtr = () => memptr.Text = "Memory: " + newval.ToString();
                evwaiting.Set();
            }
        }

        private void MindBlow_Resize(object sender, EventArgs e)
        {
            DoLayout();
        }

        private void run_Click(object sender, EventArgs e)
        {
            InterpreterThread = new Thread(() =>
            {
                try
                {
                    Interpreter.Reset();
                    Interpreter.Execute(programinput.Text);
                }
                catch (ThreadAbortException) { } // ignore
                catch (Exception ex)
                {
                    Desktop.InvokeOnWorkerThread(() => Infobox.Show("Program Error", "An error occurred while executing your program: " + ex.GetType().ToString()));
                }
            });
            InterpreterThread.Start();
        }

        private void tabs_Selected(object sender, TabControlEventArgs e)
        {
            DoLayout();
        }

        private void load_Click(object sender, EventArgs e)
        {
            AppearanceManager.SetupDialog(new FileDialog(new string[] { ".bf" }, FileOpenerStyle.Open, new Action<string>((file) => programinput.Text = Objects.ShiftFS.Utils.ReadAllText(file))));
        }

        private void save_Click(object sender, EventArgs e)
        {
            AppearanceManager.SetupDialog(new FileDialog(new string[] { ".bf" }, FileOpenerStyle.Save, new Action<string>((file) => Objects.ShiftFS.Utils.WriteAllText(file, programinput.Text))));
        }

        private void KillThread()
        {
            if (InterpreterThread != null)
                try
                {
                    InterpreterThread.Abort();
                }
                catch { }
            consoleout.KeyPress -= consolestr.handler;
        }

        private void stop_Click(object sender, EventArgs e)
        {
            KillThread();
        }

        private void reset_Click(object sender, EventArgs e)
        {
            if (InterpreterThread != null)
                if (InterpreterThread.IsAlive)
                    Infobox.Show("Cannot Reset", "The program is still running.");
                else
                    Interpreter.Reset();
        }
    }
}
