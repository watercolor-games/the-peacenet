using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Frontend.Apps;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Frontend.GUI;

namespace Plex.Frontend
{
    public static class UIManagerTools
    {
        public static void EnterTextMode()
        {
            //Close all windows in the ui
            while(AppearanceManager.OpenForms.Count > 0)
            {
                AppearanceManager.OpenForms[0].Close();
                AppearanceManager.OpenForms.RemoveAt(0);
            }
            UIManager.ClearTopLevels();
            var term = new TerminalControl();
            AppearanceManager.ConsoleOut = term;
            AppearanceManager.StartConsoleOut();
            term.X = 0;
            term.Y = 0;
            term.Width = UIManager.Viewport.Width;
            term.Height = UIManager.Viewport.Height;

            UIManager.AddTopLevel(term);
        }

        public static bool InProtectedGUI
        {
            get
            {
                return _inprotectedgui;
            }
        }

        public static Control ProtectedGUIControl
        {
            get
            {
                return _pguictrl;
            }
        }

        private static bool _inprotectedgui = false;
        private static int _pguiregsizex = 0;
        private static int _pguiregsizey = 0;
        private static int _pguireglocx = 0;
        private static int _pguireglocy = 0;
        private static Control _pguictrl = null;
        private static Control _pguiparent = null;

        public static void EnterProtectedGUI(Control ctrl)
        {
            if(_pguictrl != null)
            {
                LeaveProtectedGUI();
            }
            _pguireglocx = ctrl.X;
            _pguireglocy = ctrl.Y;
            _pguiregsizex = ctrl.Width;
            _pguiregsizey = ctrl.Height;

            ctrl.MaxWidth = int.MaxValue;
            ctrl.MaxHeight = int.MaxValue;


            if (ctrl.Parent != null)
            {
                _pguiparent = ctrl.Parent;
                _pguiparent.RemoveControl(ctrl);
                _pguiparent.Visible = false;
            }

            _pguictrl = ctrl;
            _inprotectedgui = true;
            UIManager.AddTopLevel(ctrl);

            ctrl.Width = UIManager.Viewport.Width;
            var desk = UIManager.TopLevels.FirstOrDefault(x => x is Desktop.Desktop);

            ctrl.X = 0;
            ctrl.Height = UIManager.Viewport.Height - desk.Height - 1;
            ctrl.Y = (desk.Y == 0) ? desk.Height+1 : 0;

        }

        public static void LeaveProtectedGUI()
        {
            if (_pguictrl == null)
                throw new Exception("You're not in protected GUI mode.");
            UIManager.StopHandling(_pguictrl);
            if(_pguiparent != null)
            {
                _pguiparent.AddControl(_pguictrl);
                _pguiparent.Visible = true;
            }
            _pguictrl.X = _pguireglocx;
            _pguictrl.Y = _pguireglocy;
            _pguictrl.Width = _pguiregsizex;
            _pguictrl.Height = _pguiregsizey;
            _pguictrl = null;
            _pguiparent = null;
            _inprotectedgui = false;
        }
    }
}
