using System;

namespace Plex.Engine
{
    internal class QA
    {
        internal static void Assert(bool evaluation, bool expected, string error)
        {
            if(evaluation != expected)
            {
                CrashHandler.Start(new FatalBug(error));
            }
        }

        internal static void TestFor(Func<bool> method, bool expected, string error)
        {
#if DEBUG
            if(method?.Invoke() != expected)
                CrashHandler.Start(new FatalBug(error));
#endif
        }
    }

    public class FatalBug : Exception
    {
        public FatalBug(string message) : base("A fatal bug has occurred: " + message)
        {

        }
    }
}