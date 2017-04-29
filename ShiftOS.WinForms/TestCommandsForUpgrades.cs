using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms
{
    [Namespace("test")]
    public static class TestCommandsForUpgrades
    {
        [ShiftoriumUpgrade("Test Command", 50, "This is a simple test command", null, "Test")]
        [Command("simpletest")]
        public static bool Simple()
        {
            return true;
        }
    }
}
