using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine
{
    /// <summary>
    /// Provides a better way of logging information to the engine debug console.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Log text to the console.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="type">The type of log</param>
        /// <param name="source">The source of the log</param>
        public static void Log(string message, LogType type = LogType.Info, string source = "engine")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{DateTime.Now}] <{source}/");
            switch(type)
            {
                case LogType.Info:
                    sb.Append("info");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogType.Warning:
                    sb.Append("warning");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Error:
                    sb.Append("error");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case LogType.Fatal:
                    sb.Append("FATAL");
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

            }
            sb.Append($"> {message}");
            Console.WriteLine(sb.ToString());

        }
    }

    /// <summary>
    /// Represents a type of debug log.
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Regular log message, no need to worry.
        /// </summary>
        Info,
        /// <summary>
        /// Something might be wrong but it probably doesn't matter.
        /// </summary>
        Warning,
        /// <summary>
        /// Something's definitely wrong but we can carry on.
        /// </summary>
        Error,
        /// <summary>
        /// Something's gone completely nuts. The engine's not carrying on.
        /// </summary>
        Fatal
    }
}
