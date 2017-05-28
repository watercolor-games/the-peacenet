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
                        if (Shiftorium.UpgradeInstalled("first_steps"))
                        {
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
                        }
                        else
                        {
                            Story.Start("first_steps");

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
            string primary = SaveSystem.CurrentUser.Username + " ";
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
                        if (!TerminalBackend.PrefixEnabled)
                        {
                            string textraw = txt.Lines[txt.Lines.Length - 1];
                            TextSent?.Invoke(textraw);
                            TerminalBackend.SendText(textraw);
                            return;
                        }
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
                                text3 = text4.Remove(0, $"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ".Length);
                            }
                            TerminalBackend.LastCommand = text3;
                            TextSent?.Invoke(text4);
                            TerminalBackend.SendText(text4);
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
                        var workaround = $"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ";
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
                    var header = $"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ";
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
                    if (tostring3 == $"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ")
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

        public static string RemoteSystemName { get; set; }
        public static string RemoteUser { get; set; }
        public static string RemotePass { get; set; }

        [Story("first_steps")]
        public static void FirstSteps()
        {
            TerminalBackend.PrefixEnabled = false;
            new Thread(() =>
            {
                TerminalBackend.InStory = true;
                Console.WriteLine("Hey there, and welcome to ShiftOS.");
                Thread.Sleep(2000);
                Console.WriteLine("My name is DevX. I am the developer of this operating system.");
                Thread.Sleep(2000);
                Console.WriteLine("Right now, I am using the Terminal application as a means of talking to you.");
                Thread.Sleep(2000);
                Console.WriteLine("ShiftOS is a very early operating system, but I have big plans for it.");
                Thread.Sleep(2000);
                Console.WriteLine("I can't reveal all my plans to you at this moment, but you play a big role.");
                Thread.Sleep(2000);
                Console.WriteLine("Your role in all of this is to help me develop ShiftOS more.");
                Thread.Sleep(2000);
                Console.WriteLine("You may not know how to program, but that's perfectly fine. You don't need to.");
                Thread.Sleep(2000);
                Console.WriteLine("What you do need to do, is simply use the operating system - like you would a regular computer.");
                Thread.Sleep(2000);
                Console.WriteLine("As you use ShiftOS, you will earn a special currency called Codepoints.");
                Thread.Sleep(2000);
                Console.WriteLine("The more things you do, the more Codepoints you get! Simple, right?");
                Thread.Sleep(2000);
                Console.WriteLine("Once you rack up enough Codepoints, you can use them inside the Shiftorium to buy new features for ShiftOS.");
                Thread.Sleep(2000);
                Console.WriteLine("These features include new programs, system enhancements, Terminal commands, and so much more!");
                Thread.Sleep(2000);
                Console.WriteLine("Ahh, that reminds me. I suppose you don't know how to use the Terminal yet, do you...");
                Thread.Sleep(2000);
                Console.WriteLine("Well, the ShiftOS terminal is similar to a regular Linux terminal, however things are a bit... how you say.... primitive.");
                Thread.Sleep(2000);
                Console.WriteLine("Let's just say.... I've been focusing more on function than form with this one.... Anyways, here's how the terminal works.");
                Thread.Sleep(2000);
                Console.WriteLine("Each command is categorized into a \"Namespace\". All a namespace is, is a nice way of distinguishing between commands.");
                Thread.Sleep(2000);
                Console.WriteLine("...For example you may have a bunch of commands for managing files, and others for opening/closing programs.");
                Thread.Sleep(2000);
                Console.WriteLine("The three main namespaces you'll be using for the next while are the \"sos\", \"shiftorium\", and \"win\" namespaces.");
                Thread.Sleep(2000);
                Console.WriteLine("To run a command, simply type its namespace, followed by a period/full-stop, followed by the command name.");
                Thread.Sleep(2000);
                Console.WriteLine("Give it a try! Type \"sos.help\" in the following prompt to view a list of all ShiftOS commands.");
                Thread.Sleep(2000);
                TerminalBackend.InStory = false;
                TerminalBackend.PrefixEnabled = true;
                TerminalBackend.PrintPrompt();
                bool help_entered = false;
                TerminalBackend.CommandProcessed += (text, args) =>
                {
                    if (text.EndsWith("sos.help") && help_entered == false)
                        help_entered = true;
                };
                while (help_entered == false)
                    Thread.Sleep(10);
                TerminalBackend.InStory = true;
                TerminalBackend.PrefixEnabled = false;
                Thread.Sleep(2000);
                Console.WriteLine("Good job! Next, we will look at how to pass data to a command, such as win.open.");
                Thread.Sleep(2000);
                Console.WriteLine("In ShiftOS, passing data to a command is quite simple! After the command name, place an opening and closing curly brace, like so: \"win.open{}\".");
                Thread.Sleep(2000);
                Console.WriteLine("Everything between those two curly braces is treated as command data.");
                Thread.Sleep(2000);
                Console.WriteLine("However, you can't just spam a bunch of 1s and 0s and call it a day, nonono!");
                Thread.Sleep(2000);
                Console.WriteLine("Command data is split into a list of keys and values.");
                Thread.Sleep(2000);
                Console.WriteLine("The key tells the command the name of the data, and the value is what the command will see when it looks at the key.");
                Thread.Sleep(2000);
                Console.WriteLine("There are three main types of values. Booleans, which can be either \"true\" or \"false\", Numbers, which can be any integer number, positive or negative, and Strings - any piece of text as long as it is surrounded by double-quotes.");
                Thread.Sleep(2000);
                Console.WriteLine("For example, we could write every programmer's first program - by typing \"trm.echo{msg:\"Hello, world!\"}\". Running this will cause the Terminal to print, well, \"Hello, world!\"");
                Thread.Sleep(2000);
                Console.WriteLine("To open an application in ShiftOS, you can use this principle with the \"win.open\" command.");
                Thread.Sleep(2000);
                Console.WriteLine("First, type \"win.open\" with no data to see a list of all installed programs.");
                Thread.Sleep(2000);
                TerminalBackend.InStory = false;
                bool winopenEntered = false;
                TerminalBackend.PrefixEnabled = true;
                TerminalBackend.PrintPrompt();
                TerminalBackend.CommandProcessed += (text, args) =>
                {
                    if (help_entered == true)
                        if (text.EndsWith("win.open") && winopenEntered == false)
                            winopenEntered = true;
                };
                while (winopenEntered == false)
                    Thread.Sleep(10);
                TerminalBackend.InStory = true;
                TerminalBackend.PrefixEnabled = false;

                Thread.Sleep(2000);
                Console.WriteLine("Pretty cool, it gave you a nice list of other win.open commands that will let you open each program.");
                Thread.Sleep(2000);
                Console.WriteLine("You've got the just of using ShiftOS. Now, for your goal.");
                Thread.Sleep(2000);
                Console.WriteLine("As you know, ShiftOS doesn't have very many features.");
                Thread.Sleep(2000);
                Console.WriteLine("Using the applications you have, I need you to earn 50,000 Codepoints.");
                Thread.Sleep(2000);
                Console.WriteLine("You can use the Codepoints you earn to buy new applications and features in the Shiftorium, to help earn Codepoints.");
                Thread.Sleep(2000);
                Console.WriteLine("Start small, try to earn 500. Once you do, I'll contact you with more details.");
                Thread.Sleep(2000);
                Console.WriteLine("I'll leave you to it, you've got the hang of it! One last thing, if ever you find yourself in another program, and want to exit, simply press CTRL+T to return to the Terminal.");
                Thread.Sleep(2000);
                TerminalBackend.PrefixEnabled = true;
                TerminalBackend.InStory = false;
                SaveSystem.SaveGame();
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
                                Console.Write($"{SaveSystem.CurrentUser.Username}@shiftos:~$ ");
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