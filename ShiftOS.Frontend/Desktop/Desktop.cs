using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;

namespace ShiftOS.Frontend.Desktop
{
    public class Desktop : GUI.Control, IDesktop
    {
        public string DesktopName
        {
            get
            {
                return "ShiftOS MonoGame Desktop";
            }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public Size GetSize()
        {
            return new Size(Width, Height);
        }

        public void HideAppLauncher()
        {
            
        }

        public void InvokeOnWorkerThread(Action act)
        {
            UIManager.CrossThreadOperations.Enqueue(act);
        }

        public void KillWindow(IWindowBorder border)
        {
        }

        public void MaximizeWindow(IWindowBorder brdr)
        {
        }

        public void MinimizeWindow(IWindowBorder brdr)
        {
        }

        public void OpenAppLauncher(Point loc)
        {
        }

        public void PopulateAppLauncher(LauncherItem[] items)
        {
        }

        public void PopulatePanelButtons()
        {
        }

        public void PushNotification(string app, string title, string message)
        {
        }

        public void RestoreWindow(IWindowBorder brdr)
        {
        }

        public void SetupDesktop()
        {
        }

        public void Show()
        {
        }

        public void ShowWindow(IWindowBorder border)
        {
        }
    }
}
