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
    [FileHandler("Shell script", ".trm", "fileicontrm")]
    [Launcher("{TITLE_TERMINAL}", false, null, "{AL_UTILITIES}")]
    [WinOpen("{WO_TERMINAL}")]
    [DefaultTitle("{TITLE_TERMINAL}")]
    [DefaultIcon("iconTerminal")]
    public partial class Terminal : UserControl, IShiftOSWindow, IFileHandler
    {
        public void OpenFile(string file)
        {
            int lastline = 0;
            string lastlinetext = "";

            try
            {
                var lines = new List<string>(ShiftOS.Objects.ShiftFS.Utils.ReadAllText(file).Split(new[] { Environment.NewLine.ToString() }, StringSplitOptions.RemoveEmptyEntries));
                AppearanceManager.SetupWindow(this);
                var parser = CommandParser.GenerateSample();
                foreach (var line in lines)
                {
                    lastline = lines.IndexOf(line) + 1;
                    lastlinetext = line;
                    var command = parser.ParseCommand(line);
                    TerminalBackend.InvokeCommand(command.Key, command.Value);
                }
            }
            catch(Exception ex)
            {
                ConsoleEx.ForegroundColor = ConsoleColor.Red;
                ConsoleEx.Bold = true;
                Console.WriteLine("Script exception");
                ConsoleEx.ForegroundColor = ConsoleColor.Yellow;
                ConsoleEx.Bold = false;
                ConsoleEx.Italic = true;
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                Console.WriteLine(ex.StackTrace);
                if(lastline > 0)
                {
                    Console.WriteLine(" at " + lastlinetext + " (line " + lastline + ") in " + file);
                }
            }
            TerminalBackend.PrintPrompt();
        }

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
                        if (!TerminalBackend.InStory)
                            a.SuppressKeyPress = false;
                        if (!TerminalBackend.PrefixEnabled)
                        {
                            string textraw = txt.Lines[txt.Lines.Length - 1];
                            TextSent?.Invoke(textraw);
                            TerminalBackend.SendText(textraw);
                            return;
                        }
                        var text = txt.Lines.ToArray();
                        var text2 = text[text.Length - 1];
                        var text3 = "";
                        txt.AppendText(Environment.NewLine);
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
                                    var result = SkinEngine.LoadedSkin.CurrentParser.ParseCommand(text3);

                                    if (result.Equals(default(KeyValuePair<string, Dictionary<string, string>>)))
                                    {
                                        Console.WriteLine("{ERR_SYNTAXERROR}");
                                    }
                                    else
                                    {
                                        TerminalBackend.InvokeCommand(result.Key, result.Value);
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
                    if (SaveSystem.CurrentSave != null)
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
                }
                else if (a.KeyCode == Keys.Up)
                {
                    var tostring3 = txt.Lines[txt.Lines.Length - 1];
                    if (tostring3 == $"{SaveSystem.CurrentUser.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ")
                        Console.Write(TerminalBackend.LastCommand);
                    ConsoleEx.OnFlush?.Invoke();
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
                TerminalBackend.PrintPrompt();
                if (!ShiftoriumFrontend.UpgradeInstalled("window_manager"))
                {
                    rtbterm.Select(rtbterm.TextLength, 0);
                }
            }

        }

        public static string RemoteSystemName { get; set; }
        public static string RemoteUser { get; set; }
        public static string RemotePass { get; set; }

        [Story("first_steps")]
        public static void FirstSteps()
        {
            TerminalBackend.PrefixEnabled = false;
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
                Console.WriteLine("Well, you know what you say, things are best learned by doing them!");
                Thread.Sleep(2000);
                Console.WriteLine("Give it a try! Type \"help\" in the following prompt to view a list of all ShiftOS commands.");
                Thread.Sleep(2000);
                TerminalBackend.InStory = false;
                TerminalBackend.PrefixEnabled = true;
                TerminalBackend.PrintPrompt();
                bool help_entered = false;
                TerminalBackend.CommandProcessed += (text, args) =>
                {
                    if (text.EndsWith("help") && help_entered == false)
                        help_entered = true;
                };
                while (help_entered == false)
                    Thread.Sleep(10);
                TerminalBackend.InStory = true;
                TerminalBackend.PrefixEnabled = false;
                Thread.Sleep(2000);
                Console.WriteLine("Good job! Next, we will look at how to pass data to a command, such as buy.");
                Thread.Sleep(2000);
                Console.WriteLine("In ShiftOS, passing data to a command is quite simple! All you have to do is put --, and then the name of the argument! For example, you could do \"buy --upgrade \"name_of_upgrade\"\"!");
                Thread.Sleep(2000);
                Console.WriteLine("However, you can't just spam a bunch of 1s and 0s and call it a day, nonono! You have to give it a value!");
                Thread.Sleep(2000);
                Console.WriteLine("There are three main types of values. Booleans, which can be either \"true\" or \"false\", Numbers, which can be any integer number, positive or negative, and Strings - any piece of text as long as it is surrounded by double-quotes.");
                Thread.Sleep(10000);
                Console.WriteLine("Now that you've got the gist of using ShiftOS, I have a goal for you.");
                Thread.Sleep(2000);
                Console.WriteLine("As you know, ShiftOS doesn't have very many features.");
                Thread.Sleep(2000);
                Console.WriteLine("Using the applications you have, I need you to earn as many Codepoints as you can.");
                Thread.Sleep(2000);
                Console.WriteLine("You can use the Codepoints you earn to buy new applications and features in the Shiftorium, to help earn even more Codepoints.");
                Thread.Sleep(2000);
                Console.WriteLine("Once you earn 1,000 Codepoints, I will check back with you and see how well you've done.");
                Thread.Sleep(2000);
                Console.WriteLine("I'll leave you to it, you've got the hang of it! One last thing, if ever you find yourself in another program, and want to exit, simply press CTRL+T to return to the Terminal.");
                Thread.Sleep(2000);
                TerminalBackend.PrefixEnabled = true;
                TerminalBackend.InStory = false;
                SaveSystem.SaveGame();
                Thread.Sleep(1000);

            Story.Context.AutoComplete = false;

            Console.WriteLine(@"Welcome to the ShiftOS Newbie's Guide.

This tutorial will guide you through the more intermediate features of ShiftOS,
such as earning Codepoints, buying Shiftoorium Upgrades, and using the objectives system.

Every now and then, you'll get a notification in your terminal of a ""NEW OBJECTIVE"".
This means that someone has instructed you to do something inside ShiftOS. At any moment
you may type ""status"" in your Terminal to see your current objectives.

This command is very useful as not only does it allow you to see your current objectives
but you can also see the amount of Codepoints you have as well as the upgrades you've
installed and how many upgrades are available.

Now, onto your first objective! All you need to do is earn 200 Codepoints using any
program on your system.");

            Story.PushObjective("First Steps: Your First Codepoints", "Play a few rounds of Pong, or use another program to earn 200 Codepoints.", () =>
            {
                return SaveSystem.CurrentSave.Codepoints >= 200;
            }, () =>
            {
                Desktop.InvokeOnWorkerThread(() =>
                {
                    AppearanceManager.SetupWindow(new Terminal());
                });
                Console.WriteLine("Good job! You've earned " + SaveSystem.CurrentSave.Codepoints + @" Codepoints! You can use these inside the
Shiftorium to buy new upgrades inside ShiftOS.

These upgrades can give ShiftOS more features, fixes and programs,
which can make the operating system easier to use and navigate around
and also make it easier for you to earn more Codepoints and thus upgrade
the system further.

Be cautious though! Only certain upgrades offer the ability to earn more
Codepoints. These upgrades are typically in the form of new programs.

So, try to get as many new programs as possible for your computer, and save
the system feature upgrades for later unless you absolutely need them.

The worst thing that could happen is you end up stuck with very little Codepoints
and only a few small programs to use to earn very little amounts of Codepoints.

Now, let's get you your first Shiftorium upgrade!");

                Story.PushObjective("First Steps: The Shiftorium", "Buy your first Shiftorium upgrade with your new Codepoints using the upgrades, upgradeinfo and buy commands.",
                    () =>
                    {
                        return SaveSystem.CurrentSave.CountUpgrades() > 0;
                    }, () =>
                    {
                        Console.WriteLine("This concludes the ShiftOS Newbie's Guide! Now, go, and shift it your way!");
                        Console.WriteLine(@"
Your goal: Earn 1,000 Codepoints.");
                        Story.Context.MarkComplete();
                        Story.Start("first_steps_transition");
                    });
                TerminalBackend.PrintPrompt();
            });

            TerminalBackend.PrintPrompt();
        }


        [Story("first_steps_transition")]
        public static void FirstStepsTransition()
        {
            Story.PushObjective("Earn 1000 Codepoints", "You now know the basics of ShiftOS. Let's get your system up and running with a few upgrades, and get a decent amount of Codepoints.", () =>
            {
                return SaveSystem.CurrentSave.Codepoints >= 1000;
            },
            () =>
            {
                Story.Context.MarkComplete();
                Story.Start("victortran_shiftnet");
            });

            Story.Context.AutoComplete = false;
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
                        //Console.WriteLine("");
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