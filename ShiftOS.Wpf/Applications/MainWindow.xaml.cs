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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShiftOS.Engine;
using backend = ShiftOS.Engine.TerminalBackend;

namespace ShiftOS.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Launcher("Terminal", false)]
    public partial class Terminal : UserControl, IShiftOSWindow
    {
        public Terminal()
        {
            InitializeComponent();
            Startup.ConsoleOut = this.txtterm;

            if (SaveSystem.CurrentSave == null)
            {
                Startup.InitiateEngine(new WpfTerminalTextWriter());
                SaveSystem.GameReady += () =>
                {
                    try
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            txtterm.Text = "";
                            backend.PrefixEnabled = true;
                            backend.InStory = false;
                            if (SaveSystem.CurrentSave.StoryPosition != 8)
                            {
                                Story.RunFromInternalResource("sys_shiftoriumstory");
                                SaveSystem.CurrentSave.StoryPosition = 8;
                            }
                            Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                            txtterm.Background = SkinEngine.LoadedSkin.TerminalBackColor.CreateBrush();
                            txtterm.Foreground = SkinEngine.LoadedSkin.TerminalForeColor.CreateBrush();
                            txtterm.SetFont(SkinEngine.LoadedSkin.TerminalFont);
                        }));
                    }
                    catch { }
                };
            }
            else
            {
                Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
            }
            txtterm.Background = SkinEngine.LoadedSkin.TerminalBackColor.CreateBrush();
            txtterm.Foreground = SkinEngine.LoadedSkin.TerminalForeColor.CreateBrush();

            txtterm.GotFocus += (o, a) =>
            {
                Startup.ConsoleOut = txtterm;
            };

            txtterm.Focus();

        }

        public void OnLoad()
        {
            this.SetTitle("Terminal");
        }

        public void OnSkinLoad()
        {
            txtterm.SetFont(SkinEngine.LoadedSkin.TerminalFont);
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        public void txtterm_KeyDown(object o, KeyEventArgs a) {
            if (a.Key == Key.Enter)
            {
                try
                {
                    a.Handled = true;
                    var text2 = txtterm.GetLineText(txtterm.LineCount - 1);
                    Console.WriteLine("");
                    var text3 = "";
                    var text4 = Regex.Replace(text2, @"\t|\n|\r", "");

                    if (backend.PrefixEnabled)
                    {
                        text3 = text4.Remove(0, $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ".Length);
                    }
                    backend.LastCommand = text3;
                    if (backend.InStory == false)
                    {
                        backend.InvokeCommand(text3);
                    }
                    if (backend.PrefixEnabled)
                    {
                        Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else if (a.Key == Key.Back)
            {
                var tostring3 = txtterm.GetLineText(txtterm.LineCount - 1);
                var tostringlen = tostring3.Length + 1;
                var workaround = $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ";
                var derp = workaround.Length + 1;
                if (tostringlen != derp)
                {
                    AppearanceManager.CurrentPosition--;
                }
                else
                {
                    a.Handled = true;
                }
            }
            else if (a.Key == Key.Left)
            {
                var getstring = txtterm.GetLineText(txtterm.LineCount - 1);
                var stringlen = getstring.Length + 1;
                var header = $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ";
                var headerlen = header.Length + 1;
                var selstart = txtterm.SelectionStart;
                var remstrlen = txtterm.Text.Length - stringlen;
                var finalnum = selstart - remstrlen;

                if (finalnum != headerlen)
                {
                    AppearanceManager.CurrentPosition--;
                }
                else
                {
                    a.Handled = true;
                }
            }
            //( ͡° ͜ʖ ͡° ) You found the lennyface without looking at the commit message. Message Michael in the #shiftos channel on Discord saying "ladouceur" somewhere in your message if you see this.
            else if (a.Key == Key.Up)
            {
                var tostring3 = txtterm.GetLineText(txtterm.LineCount - 1);
                if (tostring3 == $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ")
                    Console.Write(backend.LastCommand);
                a.Handled = true;

            }
            else
            {

                AppearanceManager.CurrentPosition++;
            }

        }

        private void txtterm_GotFocus(object sender, RoutedEventArgs e)
        {
            Startup.ConsoleOut = txtterm;
        }
    }
}
