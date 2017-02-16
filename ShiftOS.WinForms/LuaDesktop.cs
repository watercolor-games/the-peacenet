using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShiftOS.Engine;
using ShiftOS.Engine.Scripting;

namespace ShiftOS.WinForms
{
    public partial class LuaDesktop : Form, IDesktop
    {
        public LuaDesktop()
        {
            InitializeComponent();
        }

        private LuaInterpreter interpreter = null;

        public string DesktopName
        {
            get
            {
                string name = "Unknown";
                try
                {
                    name = (string.IsNullOrWhiteSpace(interpreter.Lua.deskName)) ? "Unknown" : interpreter.Lua.deskName; 
                }
                catch
                {

                }
                return name;
            }
        }

        public Size GetSize()
        {
            return this.Size;
        }

        public void InvokeOnWorkerThread(Action act)
        {
            this.Invoke(act);
        }

        public void KillWindow(IWindowBorder border)
        {
            throw new NotImplementedException();
        }

        public void MaximizeWindow(IWindowBorder brdr)
        {
            throw new NotImplementedException();
        }

        public void MinimizeWindow(IWindowBorder brdr)
        {
            throw new NotImplementedException();
        }

        public void PopulateAppLauncher(LauncherItem[] items)
        {
            
        }

        public void PopulatePanelButtons()
        {
            throw new NotImplementedException();
        }

        public void RestoreWindow(IWindowBorder brdr)
        {
            throw new NotImplementedException();
        }

        public void SetupDesktop()
        {
            throw new NotImplementedException();
        }

        public void ShowWindow(IWindowBorder border)
        {
            throw new NotImplementedException();
        }
    }
}
