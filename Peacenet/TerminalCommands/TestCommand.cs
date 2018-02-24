using System.Collections.Generic;
using Plex.Objects;
using Plex.Engine;

namespace Peacenet.TerminalCommands
{
    /// <summary>
    /// Lists all available commands and their descriptions.
    /// </summary>
    public class TestCommand : ITerminalCommand
    {
        [Dependency]
        private TerminalManager _terminal = null;

        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Shifty\'s a comin\' for ya!";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "ashifter";
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Usages
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            console.WriteLine("\nShifty\'s a comin\' for ya!");
            console.SlowWrite("                              ");
            console.SlowWrite("          NNhyhhm             ");
            console.SlowWrite("         moooooooN            ");
            console.SlowWrite("         dooooooosN           ");
            console.SlowWrite("         sooo soooy           ");
            console.SlowWrite("        Nooso  ooooym         ");
            console.SlowWrite("        hyosy   ooooohd       ");
            console.SlowWrite("        sosossssoooood        ");
            console.SlowWrite("       doooosyhdMsoooy        ");
            console.SlowWrite("       soooohd   dsoooN       ");
            console.SlowWrite("       ooooN      dsosh       ");
            console.SlowWrite("       ooos        yooy       ");
            console.SlowWrite("       hysm         mood      ");
            console.SlowWrite("                     mdN      ");
            console.SlowWrite("                              ");

        }
    }
}
