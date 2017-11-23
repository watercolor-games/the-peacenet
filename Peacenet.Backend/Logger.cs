using System;
namespace Peacenet.Backend
{
    internal static class Logger
    {
        public static void Log(string message)
        {
            Console.WriteLine("[{0}] {1}", DateTime.Now, message);
        }
    }
}
