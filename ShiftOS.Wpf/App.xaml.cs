/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
