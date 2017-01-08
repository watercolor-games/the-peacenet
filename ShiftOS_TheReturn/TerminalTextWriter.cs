using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace ShiftOS.Engine
{
    public class TerminalTextWriter : TextWriter
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);
        

        public override Encoding Encoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }

        public ITerminalWidget UnderlyingControl
        {
            get
            {
                return AppearanceManager.ConsoleOut;
            }
        }

        public void select()
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                UnderlyingControl.SelectBottom();
                
            }));
        }

        public override void Write(char value)
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                UnderlyingControl.Write(value.ToString());
                select();
            }));
        }

        public override void WriteLine(string value)
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                UnderlyingControl.WriteLine(value);
                select();
            }));
        }

        public void SetLastText()
        {
        }

        public override void Write(string value)
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                UnderlyingControl.Write(value.ToString());
                select();
            }));
        }


    }
}
