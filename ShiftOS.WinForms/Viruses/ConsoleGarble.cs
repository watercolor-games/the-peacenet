using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using System.Windows.Forms;

namespace ShiftOS.WinForms.Viruses
{
    [Virus("console_garble", "Console Garbler", "Sends random characters to stdout which can muck up your Terminal. The threatlevel determines the rate at which characters are sent.")]
    public class ConsoleGarble : IVirus
    {
        Timer timer = null;
        Random rnd = null;
        
        public void Disinfect()
        {
            timer.Stop();
            timer = null;
            rnd = null;
        }

        public void Infect(int threatlevel)
        {
            rnd = new Random();
            timer = new Timer();
            timer.Interval = 6000 / threatlevel;
            timer.Tick += (o, a) =>
            {
                var oldFG = ConsoleEx.ForegroundColor;
                var oldBG = ConsoleEx.BackgroundColor;
                var character = (char)rnd.Next(255);
                while (!char.IsLetterOrDigit(character))
                    character = (char)rnd.Next(255);
                var ccolormax = Enum.GetValues(typeof(ConsoleColor)).Cast<int>().Max();

                ConsoleEx.BackgroundColor = (ConsoleColor)rnd.Next(ccolormax);
                ConsoleEx.ForegroundColor = (ConsoleColor)rnd.Next(ccolormax);

                Console.Write(character);
                ConsoleEx.OnFlush?.Invoke();

                ConsoleEx.BackgroundColor = oldBG;
                ConsoleEx.ForegroundColor = oldFG;
            };
            timer.Start();
        }
    }
}
