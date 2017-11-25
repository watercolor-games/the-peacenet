using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine
{
    public static class Logger
    {
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

    public enum LogType
    {
        Info,
        Warning,
        Error,
        Fatal
    }
}
