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
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("    the");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("peacenet");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"  Copyright (C) 2018  Watercolor Games
    This program comes with ABSOLUTELY NO WARRANTY; for details see 'Warranty Info' in Settings.
    This is free software, and you are welcome to redistribute it
    under certain conditions; see the 'License' section in Settings for details.");

            Console.WriteLine();
            Console.WriteLine("--------------------");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            var prc = Process.GetCurrentProcess();
            var other = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == prc.ProcessName && x.Id != prc.Id);
            if (other != null)
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.MessageBox.Show(caption: "The Peacenet", text: "The Peacenet is already running on your system.", icon: System.Windows.Forms.MessageBoxIcon.Error, buttons: System.Windows.Forms.MessageBoxButtons.OK);
                return;
            }

#if DEBUG
            try
            {
#endif
            using (var game = new Plexgate(args))
                game.Run();
#if DEBUG
        }
            catch(Exception ex)
            {
                Logger.Log(ex.ToString(), LogType.Fatal, "monogame");
                Console.ReadKey(true);

            }
#endif
            Environment.Exit(0);
        }
    }
}
