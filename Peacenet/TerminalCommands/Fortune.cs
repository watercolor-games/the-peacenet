using System;
using System.Collections.Generic;
using Plex.Objects;
using System.IO;

namespace Peacenet
{
    public class Fortune : ITerminalCommand
    {
        public string Name => "fortune";
        public string Description => "Nail it to your car and it'll tell you who you are.";
        public IEnumerable<string> Usages => null;

        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            var fortunes = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "fortunes.txt"));
            var random = new Random();
            console.WriteLine(fortunes[random.Next(fortunes.Length)].Replace("\\n", Environment.NewLine));
        }
    }
}
