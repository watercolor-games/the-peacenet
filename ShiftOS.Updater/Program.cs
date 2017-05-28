using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShiftOS.Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Directory.Exists("updater-work"))
            {
                //Give the engine time to shutdown before invoking this app.
                Thread.Sleep(5000);
                foreach (var f in Directory.GetFiles("updater-work"))
                {
                    var bytes = File.ReadAllBytes(f);
                    var finf = new FileInfo(f);
                    File.WriteAllBytes(finf.Name, bytes); 
                }
                Directory.Delete("updater-work", true);
            }

            //Restart the actual game.
            System.Diagnostics.Process.Start("ShiftOS.WinForms.exe");
        }
    }
}
