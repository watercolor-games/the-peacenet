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

namespace ShiftOS.WinForms.Applications {
    [Launcher("Terminal", false)]
    [WinOpen("terminal")]
    public partial class Terminal : UserControl, IShiftOSWindow {
        public static Stack<string> ConsoleStack = new Stack<string>();

        public static System.Windows.Forms.Timer ti = new System.Windows.Forms.Timer();

        public static string latestCommmand = "";

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

        [Obsolete("This is used for compatibility with old parts of the backend. Please use TerminalBackend instead.")]
        public static void InvokeCommand(string text)
        {
            TerminalBackend.InvokeCommand(text);
        }

        public Terminal() {

            InitializeComponent();
            SaveSystem.GameReady += () => {
                try {
                    this.Invoke(new Action(() => {
                        ResetAllKeywords();
                        rtbterm.Text = "";
                        TerminalBackend.PrefixEnabled = true;
                        TerminalBackend.InStory = false;
                        Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                        if (SaveSystem.CurrentSave.StoryPosition == 6) {
                            Infobox.Show("Welcome to ShiftOS.", "Welcome to the ShiftOS multi-user domain. Your goal is to upgrade your system as much as possible, and gain as much wealth as possible. The first step is to get a feel for the environment. Go forth and explore, young Shifter.");
                            SaveSystem.CurrentSave.StoryPosition++;
                        }
                    }));
                } catch { }
            };


            this.DoubleBuffered = true;

        }

        public void FocusOnTerminal() {
            rtbterm.Focus();
        }

        public static string GetTime() {
            var time = DateTime.Now;
            if (ShiftoriumFrontend.UpgradeInstalled("full_precision_time")) {
                return DateTime.Now.ToString("h:mm:ss tt");
            } else if (ShiftoriumFrontend.UpgradeInstalled("clock_am_and_pm")) {
                return time.TimeOfDay.TotalHours > 12 ? $"{time.Hour - 12} PM" : $"{time.Hour} AM";
            } else if (ShiftoriumFrontend.UpgradeInstalled("clock_hours")) {
                return ((int)time.TimeOfDay.TotalHours).ToString();
            } else if (ShiftoriumFrontend.UpgradeInstalled("clock_minutes")) {
                return ((int)time.TimeOfDay.TotalMinutes).ToString();
            } else if (ShiftoriumFrontend.UpgradeInstalled("clock")) {
                return ((int)time.TimeOfDay.TotalSeconds).ToString();
            }

            return "";
        }

        
        public static event TextSentEventHandler TextSent;

        public void ResetAllKeywords() {
            string primary = SaveSystem.CurrentSave.Username + " ";
            string secondary = "shiftos ";


            var asm = Assembly.GetExecutingAssembly();

            var types = asm.GetTypes();

            foreach (var type in types) {
                foreach (var a in type.GetCustomAttributes(false)) {
                    if (ShiftoriumFrontend.UpgradeAttributesUnlocked(type)) {
                        if (a is Namespace) {
                            var ns = a as Namespace;
                            if (!primary.Contains(ns.name)) {
                                primary += ns.name + " ";
                            }
                            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
                                if (ShiftoriumFrontend.UpgradeAttributesUnlocked(method)) {
                                    foreach (var ma in method.GetCustomAttributes(false)) {
                                        if (ma is Command) {
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

        public static void MakeWidget(Controls.TerminalBox txt) {
            AppearanceManager.StartConsoleOut();
            txt.GotFocus += (o, a) => {
                AppearanceManager.ConsoleOut = txt;
            };
            txt.KeyDown += (o, a) => {
                if (a.KeyCode == Keys.Enter) {
                    try {
                        a.SuppressKeyPress = true;
                        Console.WriteLine("");
                        var text = txt.Lines.ToArray();
                        var text2 = text[text.Length - 2];
                        var text3 = "";
                        var text4 = Regex.Replace(text2, @"\t|\n|\r", "");

                        if (TerminalBackend.PrefixEnabled) {
                            text3 = text4.Remove(0, $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ".Length);
                        }
                        TerminalBackend.LastCommand = text3;
                        TextSent?.Invoke(text4);
                        if (TerminalBackend.InStory == false) {
                            TerminalBackend.InvokeCommand(text3);
                        }
                        if (TerminalBackend.PrefixEnabled) {
                            Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                        }
                    } catch {
                    }
                } else if (a.KeyCode == Keys.Back) {
                    var tostring3 = txt.Lines[txt.Lines.Length - 1];
                    var tostringlen = tostring3.Length + 1;
                    var workaround = $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ";
                    var derp = workaround.Length + 1;
                    if (tostringlen != derp) {
                        AppearanceManager.CurrentPosition--;
                    } else {
                        a.SuppressKeyPress = true;
                    }
                } else if (a.KeyCode == Keys.Left)
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
                  //( ͡° ͜ʖ ͡° ) You found the lennyface without looking at the commit message. Message Michael in the #shiftos channel on Discord saying "ladouceur" somewhere in your message if you see this.
                  else if (a.KeyCode == Keys.Up) {
                    var tostring3 = txt.Lines[txt.Lines.Length - 1];
                    if (tostring3 == $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ")
                        Console.Write(TerminalBackend.LastCommand);
                    a.SuppressKeyPress = true;

                } else {

                    AppearanceManager.CurrentPosition++;
                }

            };

            AppearanceManager.ConsoleOut = txt;

            txt.Focus();

            txt.Font = LoadedSkin.TerminalFont;
            txt.ForeColor = LoadedSkin.TerminalForeColor;
            txt.BackColor = LoadedSkin.TerminalBackColor;

        }

        private void Terminal_Load(object sender, EventArgs e) {

        }

        private void Terminal_FormClosing(object sender, FormClosingEventArgs e) {
            ti.Stop();
        }

        public void OnLoad() {
            MakeWidget(rtbterm);

            if (SaveSystem.CurrentSave != null) {
                if (!ShiftoriumFrontend.UpgradeInstalled("window_manager")) {
                    rtbterm.Text = AppearanceManager.LastTerminalText;
                    rtbterm.Select(rtbterm.TextLength, 0);
                }
                Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");

            }


        }

        public void OnSkinLoad() {
            try {
                rtbterm.Font = LoadedSkin.TerminalFont;
                rtbterm.ForeColor = LoadedSkin.TerminalForeColor;
                rtbterm.BackColor = LoadedSkin.TerminalBackColor;
            } catch {

            }

        }

        public bool OnUnload() {
            if (SaveSystem.ShuttingDown == false) {
                if (!ShiftoriumFrontend.UpgradeInstalled("desktop")) {
                    if (AppearanceManager.OpenForms.Count <= 1) {
                        Console.WriteLine("");
                        Console.WriteLine("{WIN_CANTCLOSETERMINAL}");
                        try {
                            Console.WriteLine("");

                            if (TerminalBackend.PrefixEnabled) {
                                Console.Write($"{SaveSystem.CurrentSave.Username}@shiftos:~$ ");
                            }
                        } catch (Exception ex) {
                            Console.WriteLine(ex);
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        public void OnUpgrade() {
        }

        private void rtbterm_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
