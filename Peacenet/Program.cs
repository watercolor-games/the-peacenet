using System;
using Plex.Engine;
using System.Diagnostics;
using System.Linq;

namespace Peacenet
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var prc = Process.GetCurrentProcess();
            var other = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == prc.ProcessName && x.Id != prc.Id);
            if(other != null)
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.MessageBox.Show(caption: "The Peacenet", text: "The Peacenet is already running on your system.", icon: System.Windows.Forms.MessageBoxIcon.Error, buttons: System.Windows.Forms.MessageBoxButtons.OK);
                return;
            }

            using (var game = new Plexgate())
                game.Run();
            Environment.Exit(0);
        }
    }
}
