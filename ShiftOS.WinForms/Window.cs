using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using ShiftOS.Engine.Scripting;


namespace ShiftOS.WinForms
{
    public partial class Window : UserControl, IShiftOSWindow
    {
        public Window()
        {
            InitializeComponent();
        }

        public void OnLoad()
        {
            LuaInterpreter.RaiseEvent("on_window_load", this);
        }

        public void OnSkinLoad()
        {
            LuaInterpreter.RaiseEvent("on_window_skin_load", this);
        }

        public bool OnUnload()
        {
            LuaInterpreter.RaiseEvent("on_window_unload", this);
            return true;
        }

        public void OnUpgrade()
        {
            LuaInterpreter.RaiseEvent("on_window_upgrade", this);
        }
    }
}
