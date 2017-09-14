using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Frontend.Apps;
using Plex.Frontend.GraphicsSubsystem;

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
    }
}
