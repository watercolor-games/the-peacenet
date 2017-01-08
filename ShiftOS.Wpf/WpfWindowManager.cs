using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ShiftOS.Engine;

namespace ShiftOS.Wpf
{
    public class WpfWindowManager : WindowManager
    {
        public override void Close(IShiftOSWindow win)
        {
            IWindowBorder brdrToClose = null;

            foreach(WpfWindowBorder brdr in AppearanceManager.OpenForms)
            {
                if(brdr.ParentWindow == win)
                {
                    brdrToClose = brdr;
                }
            }

            if (brdrToClose != null)
            {
                brdrToClose.Close();
                AppearanceManager.OpenForms.Remove(brdrToClose);
            }

        }

        public override void InvokeAction(Action act)
        {
            App.Current.Dispatcher.Invoke(act);
        }

        public override void Maximize(IWindowBorder border)
        {
            var wb = (WpfWindowBorder)border;
        }

        public override void Minimize(IWindowBorder border)
        {
            throw new NotImplementedException();
        }

        public override void SetupDialog(IShiftOSWindow win)
        {
            var brdr = new WpfWindowBorder(win);
            brdr.IsDialog = true;
            AppearanceManager.OpenForms.Add(brdr);
        }

        public override void SetupWindow(IShiftOSWindow win)
        {
            var brdr = new WpfWindowBorder(win);

            AppearanceManager.OpenForms.Add(brdr);
        }
    }
}
