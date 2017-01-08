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
using System.Threading;
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
using ShiftOS.Objects;
using ShiftOS.Objects.ShiftFS;
using ShiftOS.Engine;

namespace ShiftOS.Wpf
{
    /// <summary>
    /// Interaction logic for OOBE.xaml
    /// </summary>
    public partial class OOBE : UserControl, IOobe, IShiftOSWindow
    {
        public OOBE()
        {
            InitializeComponent();
        }

        private Save MySave = null;

        public void StartShowing(Save save)
        {
            MySave = save;
            AppearanceManager.SetupWindow(this);
            SetupLanguages();
            SaveSystem.GameReady += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    AppearanceManager.Close(this);
                });
            };
        }

        public void SetupLanguages()
        {
            lblanguages.Items.Clear();

            foreach(var lang in AppearanceManager.GetLanguages())
            {
                lblanguages.Items.Add(lang);
            }
        }

        public void OnLoad()
        {
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        private void lblanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MySave.Language = lblanguages.SelectedItem.ToString();
                userInfo.Visibility = Visibility.Visible;
            }
            catch { }
        }

        private void txtusername_TextChanged(object sender, TextChangedEventArgs e)
        {
            string usr = MySave.Username;

            MySave.Username = txtusername.Text;

            if(string.IsNullOrWhiteSpace(txtsysname.Text) || txtsysname.Text == usr + "-PC")
            {
                txtsysname.Text = MySave.Username + "-PC";
            }
            nextLabel.Visibility = Visibility.Visible;
        }

        public void moveforward_Click(object s, RoutedEventArgs a)
        {
            tabControl.SelectedItem = tabControl.Items[1];
            StartInstall();
        }

        public void StartInstall()
        {
            Startup.ConsoleOut = txtterm;
            pgprogress.Value = 0;

            StartOOBESlideshow();

        }

        public void StartOOBESlideshow()
        {
            var t = new System.Timers.Timer(10000);

            int currentslide = 0;

            t.Elapsed += (o, a) =>
            {
                Dispatcher.Invoke(() =>
                {
                    switch (currentslide)
                    {
                        case 0:
                            oobehead.Text = "Introduction";
                            oobedesc.Text = @"Welcome to ShiftOS.

You have successfully registered to take part in the testing and development of ShiftOS. We are currently installing the system's core components.

This may take a while. There is a progress indicator at the bottom of this window. You can also see a log of everything that's happening in the terminal on the right.

While you wait, we'll take the time to tell you about what you'll be doing.";
                            break;
                        case 1:
                            oobehead.Text = "About the project";
                            oobedesc.Text = @"You may wanna know what exactly we're working on here. Basically, we want to make ShiftOS one of the most powerful, customizable, user-friendly operating systems in the world. Not just ONE of the most, we want to make it THE most.

We'll do whatever we can to do this - and having you onboard is just the beginning. At ShiftOS, we strive to make things user-friendly, and versatile. We want to put a barebones operating system on your computer and see what you do with it, what you make, so we can use that data to make ShiftOS awesome.";
                            break;
                        case 2:
                            oobehead.Text = "The Humble Beginnings";
                            oobedesc.Text = @"It may look complicated but we're really just wasting your time installing a blank desktop with a terminal. Seriously, that's all ShiftOS is right now. It's not about that, though.
                            
It's about what you do in this blank desktop with just a terminal. We want to see what you make of it. It's a build-your-own-system type deal - one of the best in the world, in fact.";
                            break;
                        case 3:
                            oobehead.Text = "Stay connected.";
                            oobedesc.Text = @"With ShiftOS, you're always connected to something. Like literally. Even if you turn off your home router, unplug the computer or smash it you're still connected to ShiftOS's multi-user domain. There's just no escape.
                            
Uhhhhhhh whoops.... I meant to make that not sound fishy.";

                            break;
                        case 4:
                            oobehead.Text = "Now it's too late to go back.";
                            oobedesc.Text = @"I guess you weren't paying attention to that terminal seeings as you're reading this. Now, it's too late for you to go back to your old system. You're enrolled as a tester for the ShiftOS project and all your files have been deleted.

There's no turning back. And now that you can't leave.... I don't have to keep inticing you.";
                            break;
                        case 5:
                            oobehead.Text = "Thanks for not choosing ShiftOS!";
                            oobedesc.Text = @"By now it's definitely too late. We really value our users to the point where they can never uninstall ShiftOS. ShiftOS will haunt you in your sleep and take over your entire network. It will spread to all your friends, your family, your coworkers and even other worlds.
                            
We hope you enjoy your.... eternal stay.";
                            break;
                    }
                });

                if (currentslide == 5)
                    currentslide = 0;
                else
                    currentslide++;
            };

            t.Start();

            var th = new System.Threading.Thread(new ThreadStart(() =>
            {
                Console.WriteLine("Loading useless out-of-box-experience stuff...");
                Thread.Sleep(10000);
                Console.WriteLine("Introducing ShiftOS to the user...");
                Thread.Sleep(1000);
                Console.Write("Formatting drive 0:/ with ShiftFS 2.0...");
                Thread.Sleep(100);
                Console.WriteLine(" done");
                Thread.Sleep(500);
                Console.WriteLine("Pirating programming music...");
                Thread.Sleep(250);
                Console.Write("Deafening the user... ");
                for (int i = 0; i < 100; i++)
                {
                    var th2 = new Thread(new ThreadStart(() =>
                    {
                        Console.Beep(1024, 1000);
                    }));
                    th2.Start();

                }
                Console.WriteLine("failed, whatever");

                Thread.Sleep(750);
                Console.WriteLine("Writing directories to the system...");
                foreach(var path in Paths.GetAllWithoutKey())
                {
                    Thread.Sleep(50);
                    if(path.StartsWith("0") && Utils.DirectoryExists(path))
                        Console.WriteLine(path);
                }
                Thread.Sleep(500);
                Console.WriteLine("Procrastinating...");
                Thread.Sleep(2000);
                Console.WriteLine("Making it look like we're doing things...");

                var rand = new Random();

                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        Thread.Sleep(1000 / i);
                        Console.WriteLine(rand.Next(int.MaxValue - 1000000000, Int32.MaxValue));
                    }
                    catch (DivideByZeroException e)
                    {
                        Console.WriteLine($"Attempting to divide by zero.... but getting {e.GetType().Name}s...");
                        Thread.Sleep(1000);
                    }
                }
                Console.WriteLine("Still procrastinating...");

                Thread.Sleep(5000);
                Console.Write("Enabling all the upgrades for free...");
                foreach(var upg in Shiftorium.GetDefaults())
                {
                    Shiftorium.Silent = true;
                    Thread.Sleep(100);
                    Dispatcher.Invoke(() =>
                    {
                        Shiftorium.Buy(upg.ID, 0);
                    });
                }
                Shiftorium.Silent = false;
                foreach(var upg in Shiftorium.GetDefaults())
                {
                    SaveSystem.CurrentSave.Upgrades[upg.ID] = false;
                }
                Console.WriteLine(" ...and disabling them because we can.");
                Dispatcher.Invoke(() => Shiftorium.InvokeUpgradeInstalled());

                Console.WriteLine("Showing more useless stuff...");

                while(currentslide != 0)
                {

                }
                Console.WriteLine("Done installing.");

                SaveSystem.CurrentSave = this.MySave;
                SaveSystem.CurrentSave.StoryPosition = 5;
                SaveSystem.SaveGame();
            }));
            th.IsBackground = true;
            th.Start();
        }

        private void txtpassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            MySave.Password = txtpassword.Password;
        }

        private void txtsysname_TextChanged(object sender, TextChangedEventArgs e)
        {
            MySave.SystemName = txtsysname.Text;
        }
    }
}
