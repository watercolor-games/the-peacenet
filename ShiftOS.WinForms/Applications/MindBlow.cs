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

namespace ShiftOS.WinForms.Applications
{
    [WinOpen("mindblow")]
    [Launcher("MindBlow", false, null, "Utilities")]
    [RequiresUpgrade("mindblow")]
    public partial class MindBlow : UserControl, IShiftOSWindow, IBFListener
    {
        private class TextBoxStream : Stream
        {
            private TextBox box;
            private KeysConverter kc;
            public TextBoxStream(TextBox mybox)
            {
                kc = new KeysConverter();
                box = mybox;
            }
            public override bool CanRead { get { return true; } }

            public override bool CanSeek { get { return false; } }

            public override bool CanWrite { get { return true; } }

            public override long Length { get { return box.Text.Length; } }

            public override long Position { get { return Length; } set { throw new NotImplementedException(); } }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                object lck = new object();
                int ptr = offset;
                KeyPressEventHandler handler = (o, a) =>
                {
                    lock (lck)
                    {
                        buffer[ptr] = (byte)a.KeyChar;
                        ptr++;
                    }
                };
                Desktop.InvokeOnWorkerThread(() => box.KeyPress += handler);
                while (true)
                {
                    lock (lck)
                        if (ptr >= offset + count)
                            break;
                }
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

        private void DoLayout()
        {
            memlist.Left = 0;
            memlist.Width = monitor.Width;
            memlist.Height = monitor.Height - memlist.Top;
            program.Top = 0;
            program.Left = 0;
            programinput.Width = program.Width;
            programinput.Height = program.Height - load.Height;
            load.Top = save.Top = run.Top = programinput.Height;
            load.Width = save.Width = run.Width = save.Left = program.Width / 3;
            run.Left = save.Left * 2;
        }

        public MindBlow()
        {
            InitializeComponent();
            DefaultMem = new string[30000];
            for (ushort i = 0; i < 30000; i++)
                DefaultMem[i] = "0";
            memlist.Items.AddRange(DefaultMem);
            Interpreter = new BFInterpreter(new TextBoxStream(consoleout), this);
        }

        public void IPtrMoved(int newval)
        {
            Desktop.InvokeOnWorkerThread(() => instptr.Text = "Instruction: " + newval.ToString());
        }

        public void MemChanged(ushort pos, byte newval)
        {
            Desktop.InvokeOnWorkerThread(() => memlist.Items[pos] = newval.ToString());
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
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        public void PointerMoved(ushort newval)
        {
            Desktop.InvokeOnWorkerThread(() => memptr.Text = "Memory: " + newval.ToString());
        }

        private void MindBlow_Resize(object sender, EventArgs e)
        {
            DoLayout();
        }

        private void run_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    Interpreter.Reset();
                    Interpreter.Execute(programinput.Text);
                }
                catch (Exception ex)
                {
                    Desktop.InvokeOnWorkerThread(() => Infobox.Show("Program Error", "An error occurred while executing your program: " + ex.GetType().ToString()));
                }
            }).Start();
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
    }
}
