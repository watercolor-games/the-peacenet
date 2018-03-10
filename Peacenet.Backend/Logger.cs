using System;
namespace Peacenet.Backend
{
    internal static class Logger
    {
        public static void Log(string message)
        {
            lock (Console.Out)
            {
                Console.WriteLine("[{0}] <server> {1}", DateTime.Now, message);
            }
        }
    }
}
