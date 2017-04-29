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

//#define TRAILER
//#define CRASHONSTART
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using static ShiftOS.Engine.SkinEngine;
using ShiftOS.Engine;
using ShiftOS.Objects;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Terminal", false, null, "Utilities")]
    [WinOpen("terminal")]
    [DefaultIcon("iconTerminal")]
    public partial class Terminal : UserControl, IShiftOSWindow
    {
        public static Stack<string> ConsoleStack = new Stack<string>();

        public static System.Windows.Forms.Timer ti = new System.Windows.Forms.Timer();

        public static string latestCommmand = "";

        public static bool IsInRemoteSystem = false;
        public static string RemoteGuid = "";

        [Obsolete("This is used for compatibility with old parts of the backend. Please use TerminalBackend instead.")]
        public static bool PrefixEnabled
        {
            get
            {
                return TerminalBackend.PrefixEnabled;
            }
            set
            {
                TerminalBackend.PrefixEnabled = value;
            }
        }

        [Obsolete("This is used for compatibility with old parts of the backend. Please use TerminalBackend instead.")]
        public static bool InStory
        {
            get
            {
                return TerminalBackend.InStory;
            }
            set
            {
                TerminalBackend.InStory = value;
            }
        }

        [Obsolete("This is used for compatibility with old parts of the backend. Please use TerminalBackend instead.")]
        public static string LastCommand
        {
            get
            {
                return TerminalBackend.LastCommand;
            }
            set
            {
                TerminalBackend.LastCommand = value;
            }
        }

        public int TutorialProgress
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("This is used for compatibility with old parts of the backend. Please use TerminalBackend instead.")]
        public static void InvokeCommand(string text)
        {

            TerminalBackend.InvokeCommand(text);
        }

        public Terminal()
        {

            InitializeComponent();
            SaveSystem.GameReady += () =>
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        ResetAllKeywords();
                        rtbterm.Text = "";
                        if (!Shiftorium.UpgradeInstalled("desktop"))
                        {
                            TerminalBackend.PrefixEnabled = true;
                            TerminalBackend.InStory = false;
                            TerminalBackend.PrintPrompt();
                            if (Shiftorium.UpgradeInstalled("wm_free_placement"))
                            {
                                this.ParentForm.Width = 640;
                                this.ParentForm.Height = 480;
                                this.ParentForm.Left = (Screen.PrimaryScreen.Bounds.Width - 640) / 2;
                                this.ParentForm.Top = (Screen.PrimaryScreen.Bounds.Height - 480) / 2;

                            }
                        }
                        else
                        {
                            AppearanceManager.Close(this);
                        }
                    }));
                }
                catch { }
            };


            this.DoubleBuffered = true;

        }

        public void FocusOnTerminal()
        {
            rtbterm.Focus();
        }

        public static string GetTime()
        {
            var time = DateTime.Now;
            if (ShiftoriumFrontend.UpgradeInstalled("full_precision_time"))
            {
                return DateTime.Now.ToString("h:mm:ss tt");
            }
            else if (ShiftoriumFrontend.UpgradeInstalled("clock_am_and_pm"))
            {
                return time.TimeOfDay.TotalHours > 12 ? $"{time.Hour - 12} PM" : $"{time.Hour} AM";
            }
            else if (ShiftoriumFrontend.UpgradeInstalled("clock_hours"))
            {
                return ((int)time.TimeOfDay.TotalHours).ToString();
            }
            else if (ShiftoriumFrontend.UpgradeInstalled("clock_minutes"))
            {
                return ((int)time.TimeOfDay.TotalMinutes).ToString();
            }
            else if (ShiftoriumFrontend.UpgradeInstalled("clock"))
            {
                return ((int)time.TimeOfDay.TotalSeconds).ToString();
            }

            return "";
        }


        public static event TextSentEventHandler TextSent;

        public void ResetAllKeywords()
        {
            string primary = SaveSystem.CurrentSave.Username + " ";
            string secondary = "shiftos ";


            var asm = Assembly.GetExecutingAssembly();

            var types = asm.GetTypes();

            foreach (var type in types)
            {
                foreach (var a in type.GetCustomAttributes(false))
                {
                    if (ShiftoriumFrontend.UpgradeAttributesUnlocked(type))
                    {
                        if (a is Namespace)
                        {
                            var ns = a as Namespace;
                            if (!primary.Contains(ns.name))
                            {
                                primary += ns.name + " ";
                            }
                            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                            {
                                if (ShiftoriumFrontend.UpgradeAttributesUnlocked(method))
                                {
                                    foreach (var ma in method.GetCustomAttributes(false))
                                    {
                                        if (ma is Command)
                                        {
                                            var cmd = ma as Command;
                                            if (!secondary.Contains(cmd.name))
                                                secondary += cmd.name + " ";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


        }

        public static void MakeWidget(Controls.TerminalBox txt)
        {
            AppearanceManager.StartConsoleOut();
            txt.GotFocus += (o, a) =>
            {
                AppearanceManager.ConsoleOut = txt;
            };
            txt.KeyDown += (o, a) =>
            {
                if (a.Control == true || a.Alt == true)
                {
                    a.SuppressKeyPress = true;
                    return;
                }

                if (a.KeyCode == Keys.Enter)
                {
                    try
                    {
                        a.SuppressKeyPress = true;
                        Console.WriteLine("");
                        var text = txt.Lines.ToArray();
                        var text2 = text[text.Length - 2];
                        var text3 = "";
                        var text4 = Regex.Replace(text2, @"\t|\n|\r", "");

                        if (IsInRemoteSystem == true)
                        {
                            ServerManager.SendMessage("trm_invcmd", JsonConvert.SerializeObject(new
                            {
                                guid = RemoteGuid,
                                command = text4
                            }));
                        }
                        else
                        {
                            if (TerminalBackend.PrefixEnabled)
                            {
                                text3 = text4.Remove(0, $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ".Length);
                            }
                            TerminalBackend.LastCommand = text3;
                            TextSent?.Invoke(text4);
                            if (TerminalBackend.InStory == false)
                            {
                                if (text3 == "stop theme")
                                {
                                    CurrentCommandParser.parser = null;
                                }
                                else
                                {
                                    if (CurrentCommandParser.parser == null)
                                    {
                                        TerminalBackend.InvokeCommand(text3);
                                    }
                                    else
                                    {
                                        var result = CurrentCommandParser.parser.ParseCommand(text3);

                                        if (result.Equals(default(KeyValuePair<KeyValuePair<string, string>, Dictionary<string, string>>)))
                                        {
                                            Console.WriteLine("Syntax Error: Syntax Error");
                                        }
                                        else
                                        {
                                            TerminalBackend.InvokeCommand(result.Key.Key, result.Key.Value, result.Value);
                                        }
                                    }
                                }
                            }
                            if (TerminalBackend.PrefixEnabled)
                            {
                                TerminalBackend.PrintPrompt();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                else if (a.KeyCode == Keys.Back)
                {
                    try
                    {
                        var tostring3 = txt.Lines[txt.Lines.Length - 1];
                        var tostringlen = tostring3.Length + 1;
                        var workaround = $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ";
                        var derp = workaround.Length + 1;
                        if (tostringlen != derp)
                        {
                            AppearanceManager.CurrentPosition--;
                        }
                        else
                        {
                            a.SuppressKeyPress = true;
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Drunky alert in terminal.");
                    }
                }
                else if (a.KeyCode == Keys.Left)
                {
                    var getstring = txt.Lines[txt.Lines.Length - 1];
                    var stringlen = getstring.Length + 1;
                    var header = $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ";
                    var headerlen = header.Length + 1;
                    var selstart = txt.SelectionStart;
                    var remstrlen = txt.TextLength - stringlen;
                    var finalnum = selstart - remstrlen;

                    if (finalnum != headerlen)
                    {
                        AppearanceManager.CurrentPosition--;
                    }
                    else
                    {
                        a.SuppressKeyPress = true;
                    }
                }
                else if (a.KeyCode == Keys.Up)
                {
                    var tostring3 = txt.Lines[txt.Lines.Length - 1];
                    if (tostring3 == $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ")
                        Console.Write(TerminalBackend.LastCommand);
                    a.SuppressKeyPress = true;

                }
                else
                {
                    if (TerminalBackend.InStory)
                    {
                        a.SuppressKeyPress = true;
                    }
                    AppearanceManager.CurrentPosition++;
                }

            };

            AppearanceManager.ConsoleOut = txt;

            txt.Focus();

            txt.Font = LoadedSkin.TerminalFont;
            txt.ForeColor = ControlManager.ConvertColor(LoadedSkin.TerminalForeColorCC);
            txt.BackColor = ControlManager.ConvertColor(LoadedSkin.TerminalBackColorCC);

        }

        private void Terminal_Load(object sender, EventArgs e)
        {
            ServerManager.MessageReceived += (msg) =>
            {
                if (msg.Name == "trm_handshake_guid")
                {
                    IsInRemoteSystem = true;
                    RemoteGuid = msg.GUID;
                }
                else if (msg.Name == "trm_handshake_stop")
                {
                    IsInRemoteSystem = false;
                    RemoteGuid = "";
                }
            };
        }

        private void Terminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            ti.Stop();
            IsInRemoteSystem = false;
            RemoteGuid = "";
        }

        public void OnLoad()
        {
            MakeWidget(rtbterm);

            if (SaveSystem.CurrentSave != null)
            {
                if (!ShiftoriumFrontend.UpgradeInstalled("window_manager"))
                {
                    rtbterm.Select(rtbterm.TextLength, 0);
                }
            }

            new Thread(() =>
            {
                Thread.Sleep(1000);
                TerminalBackend.PrintPrompt();
            }).Start();
        }

        public void OnSkinLoad()
        {
            try
            {
                rtbterm.Font = LoadedSkin.TerminalFont;
                rtbterm.ForeColor = ControlManager.ConvertColor(LoadedSkin.TerminalForeColorCC);
                rtbterm.BackColor = ControlManager.ConvertColor(LoadedSkin.TerminalBackColorCC);
            }
            catch
            {

            }

        }

        public bool OnUnload()
        {
            if (SaveSystem.ShuttingDown == false)
            {
                if (!ShiftoriumFrontend.UpgradeInstalled("desktop"))
                {
                    if (AppearanceManager.OpenForms.Count <= 1)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("{WIN_CANTCLOSETERMINAL}");
                        try
                        {
                            Console.WriteLine("");

                            if (TerminalBackend.PrefixEnabled)
                            {
                                Console.Write($"{SaveSystem.CurrentSave.Username}@shiftos:~$ ");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        public void OnUpgrade()
        {
        }

        private void rtbterm_TextChanged(object sender, EventArgs e)
        {

        }

        internal void ClearText()
        {
            rtbterm.Text = "";
        }
    }
}