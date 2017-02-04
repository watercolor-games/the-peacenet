using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;

namespace ModLauncher
{
    [Namespace("modlauncher")]
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ShiftOS.WinForms.Program.Main();
        }

        [Command("throwcrash")]
        public static bool ThrowCrash()
        {
            new Thread(() =>
            {
                throw new Exception("User triggered crash using modlauncher.throwcrash command.");
            }).Start();
            return true;
        }
    }
}
