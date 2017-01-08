using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ShiftOS.Engine;

namespace ShiftOS.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            global::ShiftOS.Wpf.Startup.DestroyShiftOSEngine();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SkinEngine.SkinLoaded += () =>
            {
                Resources["desktoppanelbg"] = SkinEngine.LoadedSkin.DesktopPanelColor.CreateBrush();
                Resources["desktoppanelheight"] = (double)SkinEngine.LoadedSkin.DesktopPanelHeight;
                switch (SkinEngine.LoadedSkin.DesktopPanelPosition)
                {
                    case 0:
                        Resources["desktoppanelpos"] = VerticalAlignment.Top;
                        break;
                    case 1:
                        Resources["desktoppanelpos"] = VerticalAlignment.Bottom;
                        break;
                }

                Resources["MenuBG"] = SkinEngine.LoadedSkin.Menu_MenuStripGradientBegin.CreateBrush();

                Resources["MenuItem"] = SkinEngine.LoadedSkin.Menu_MenuStripGradientBegin.CreateBrush();
                Resources["MenuItemText"] = SkinEngine.LoadedSkin.Menu_TextColor.CreateBrush();

                Resources["MenuItemPressed"] = SkinEngine.LoadedSkin.Menu_MenuItemPressedGradientBegin.CreateBrush();
                Resources["MenuItemPressedText"] = SkinEngine.LoadedSkin.Menu_SelectedTextColor.CreateBrush();

                Resources["MenuItemHover"] = SkinEngine.LoadedSkin.Menu_MenuItemSelectedGradientBegin.CreateBrush();
                Resources["MenuItemHoverText"] = SkinEngine.LoadedSkin.Menu_SelectedTextColor.CreateBrush();


                Resources["MainBackColor"] = SkinEngine.LoadedSkin.ControlColor.CreateBrush();
                Resources["MainForeColor"] = SkinEngine.LoadedSkin.ControlTextColor.CreateBrush();
                Resources["TerminalBG"] = SkinEngine.LoadedSkin.TerminalBackColor.CreateBrush();
                Resources["TerminalFG"] = SkinEngine.LoadedSkin.TerminalForeColor.CreateBrush();

            };
        }
    }
}
