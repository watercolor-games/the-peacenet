using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ShiftOS.Engine
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            try
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                //Taxes: Remote Desktop Connection and painting
                //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
                Application.ThreadException += (o, a) =>
                {
                    CrashHandler.Start(a.Exception);
                };

                Application.ApplicationExit += (o, a) =>
                 {
                     ServerManager.Disconnect();

                 //I really want a glass of juice.
                 //Process.GetCurrentProcess().Kill();
                 };
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Desktop());
            }
            catch(Exception ex)
            {
                CrashHandler.Start(ex);

            }
        }
    }
}
