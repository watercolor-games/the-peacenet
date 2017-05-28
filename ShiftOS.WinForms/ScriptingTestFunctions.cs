using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine.Scripting;

namespace ShiftOS.WinForms
{
    [Exposed("scriptingtests")]
    public class ScriptingTestFunctions
    {
        public int testVar = 127;

        public void testFunction()
        {
            Console.WriteLine("testFunction() called.");
        }

        public string testFunctionWithReturn(string arg)
        {
            return arg + "!";
        }

        public event Action testEvent;

        public string testProperty { get; set; }

        public void invokeTestEvent()
        {
            testEvent?.Invoke();
        }
    }
}
