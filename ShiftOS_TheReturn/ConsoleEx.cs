using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Provides extra eye candy data that can be used by ShiftOS terminals.
    /// </summary>
    public static class ConsoleEx
    {
        /// <summary>
        /// Initializes the <see cref="ConsoleEx"/> class, performing core configuration. 
        /// </summary>
        static ConsoleEx()
        {
            ForegroundColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Black;

            Bold = false;
            Italic = false;
            Underline = false;
        }

        /// <summary>
        /// Gets or sets the foreground color of text in the Terminal.
        /// </summary>
        public static ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the background color of text in the Terminal.
        /// </summary>
        public static ConsoleColor BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets whether text in the Terminal is bold.
        /// </summary>
        public static bool Bold { get; set; }

        /// <summary>
        /// Gets or sets whether text in the Terminal is italic.
        /// </summary>
        public static bool Italic { get; set; }

        /// <summary>
        /// Gets or sets whether text in the Terminal is underlined.
        /// </summary>
        public static bool Underline { get; set; }

        internal static void Flush()
        {
            OnFlush?.Invoke();
        }

        public static Action OnFlush;
    }
}
