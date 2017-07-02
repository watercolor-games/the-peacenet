using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;

namespace ShiftOS.Frontend.Desktop
{
    public class WindowManager : Engine.WindowManager
    {
        public override void Close(IShiftOSWindow win)
        {
            
        }

        public override void InvokeAction(Action act)
        {
            act?.Invoke();
        }

        public override void Maximize(IWindowBorder border)
        {
            throw new NotImplementedException();
        }

        public override void Minimize(IWindowBorder border)
        {
            throw new NotImplementedException();
        }

        public override void SetTitle(IShiftOSWindow win, string title)
        {
            throw new NotImplementedException();
        }

        public override void SetupDialog(IShiftOSWindow win)
        {
            throw new NotImplementedException();
        }

        public override void SetupWindow(IShiftOSWindow win)
        {
            throw new NotImplementedException();
        }
    }

    public class WindowBorder : GUI.Control, IWindowBorder
    {
        private string _text = "ShiftOS window";
        private GUI.Control _hostedwindow = null;

        public IShiftOSWindow ParentWindow
        {
            get
            {
                return (IShiftOSWindow)_hostedwindow;
            }

            set
            {
                _hostedwindow = (GUI.Control)value;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
            }
        }

        public void Close()
        {
            Visible = false;
            UIManager.StopHandling(this);
        }

        public override void MouseStateChanged()
        {
            //todo: close, minimize, maximize, drag, resize

        }
    }
}
