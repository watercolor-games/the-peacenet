using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Objects;

namespace ShiftOS.Frontend
{
    public class MonoGameOOBE : IOobe
    {
        public void PromptForLogin()
        {
            throw new NotImplementedException();
        }

        public void ShowSaveTransfer(Save save)
        {
            throw new NotImplementedException();
        }

        public void SlowWriteLine(string text)
        {
            if (!string.IsNullOrWhiteSpace(Console.Text))
                Console.WriteLine("");

            for (int i = 0; i < text.Length; i++)
            {
                System.Threading.Thread.Sleep(50);
                Console.Write(text[i].ToString());
            }
        }

        private Apps.TerminalControl Console = null;
        private GUI.ProgressBar progress = null;
        private GUI.TextControl status = null;

        public void StartShowing(Save save)
        {
            TerminalBackend.InStory = true;
            TerminalBackend.PrefixEnabled = false;

            var term = new Apps.TerminalControl();
            GraphicsSubsystem.UIManager.AddTopLevel(term);
            term.Width = GraphicsSubsystem.UIManager.Viewport.Width;
            term.Height = GraphicsSubsystem.UIManager.Viewport.Height;
            term.X = 0;
            term.Y = 0;
            AppearanceManager.ConsoleOut = term;
            AppearanceManager.StartConsoleOut();
            Console = term;

            var t = new System.Threading.Thread(() =>
            {
                SlowWriteLine("Michael VanOverbeek presents...");
                Thread.Sleep(2000);
                SlowWriteLine("A Philip Adams game...");
                Thread.Sleep(2000);
                
                SlowWriteLine("");
                SlowWriteLine(@"It's not often technology becomes out of mankind's league, out of its 
control. I mean, we are the creators of technology, we are the ones with the ideas,
innovations, and skill. It just does what we tell it to, right?");
                Thread.Sleep(2000);

                SlowWriteLine("");
                SlowWriteLine(@"Such a naive being you are to think that, after all,
you have no idea where you are... what's going on... who I am.");

                Thread.Sleep(2000);

                SlowWriteLine("");
                SlowWriteLine(@"To you, I'm just text. Something you understand. Words...
paragraphs... ideas... the very things that caused and became technology.");

                Thread.Sleep(2000);

                SlowWriteLine("");
                SlowWriteLine(@"""Where am I? What the hell's going on!?"" I hear you ask.
Such important questions, but please adjust your emotional and
mental state. This is not your concern.");

                Thread.Sleep(2000);

                SlowWriteLine("");
                SlowWriteLine(@"My name is DevX. What is yours?");

                bool nameChosen = false;
                Engine.Infobox.PromptText("What is your name?", "Please enter your name in the box below, then hit 'OK'.", (name) =>
                {
                    nameChosen = true;
                    save.Username = name;
                });

                while (nameChosen == false)
                    Thread.Sleep(10);

                Thread.Sleep(2000);

                SlowWriteLine("");
                SlowWriteLine($@"Hello there, {save.Username}. Nice to meet you.");

                Thread.Sleep(2000);

                SlowWriteLine("");
                SlowWriteLine(@"Welcome to my Digital Society.");

                Thread.Sleep(2000);

                SlowWriteLine("");
                SlowWriteLine(@"I can't and won't tell you what happened to you, but I'm going to tell you
what you're going to be doing for me... with me.");

                Thread.Sleep(2000);

                SlowWriteLine("");
                SlowWriteLine(@"But it's not time for that. First, you must be trained.
I'm installing the Digital Society's gateway operating system onto your sentience.
It's called ShiftOS.");

                Thread.Sleep(2000);

                SlowWriteLine("");
                SlowWriteLine(@"When the system is installed, I'll contact you. DevX out.");

                Thread.Sleep(5000);

                while(AppearanceManager.OpenForms.Count > 0)
                {
                    AppearanceManager.OpenForms[0].Close();
                    AppearanceManager.OpenForms.RemoveAt(0);
                }

                GUI.TextControl _shiftos = new GUI.TextControl();
                GraphicsSubsystem.UIManager.AddTopLevel(_shiftos);
                term.Clear();
                _shiftos.AutoSize = true;
                _shiftos.Font = SkinEngine.LoadedSkin.HeaderFont;
                _shiftos.Text = "ShiftOS";
                _shiftos.Y = GraphicsSubsystem.UIManager.Viewport.Height / 3;
                _shiftos.Layout(new Microsoft.Xna.Framework.GameTime());
                _shiftos.X = (GraphicsSubsystem.UIManager.Viewport.Width - _shiftos.Width) / 2;

                status = new GUI.TextControl();
                GraphicsSubsystem.UIManager.AddTopLevel(status);
                status.Font = SkinEngine.LoadedSkin.MainFont;
                status.AutoSize = true;
                status.Text = "Beginning installation...";
                status.Y = _shiftos.Y + _shiftos.Height + 5;
                status.Layout(new Microsoft.Xna.Framework.GameTime());
                status.X = (GraphicsSubsystem.UIManager.Viewport.Width - status.Width) / 2;

                progress = new GUI.ProgressBar();
                GraphicsSubsystem.UIManager.AddTopLevel(progress);
                progress.Width = 150;
                progress.Height = 10;
                progress.Maximum = 250;
                progress.Value = 0;
                progress.Y = status.Y + status.Height + 15;
                progress.X = (GraphicsSubsystem.UIManager.Viewport.Width - progress.Width) / 2;

                term.Clear();
                term.Width = 320;
                term.Height = 200;
                term.X = (GraphicsSubsystem.UIManager.Viewport.Width - term.Width) / 2;
                term.Y = progress.Y + progress.Height + 15;
                var nt = new Thread(() =>
                {
                    while(status != null)
                    {
                        try
                        {
                            if (term.Lines.Length > 0)
                            {
                                string txt = term.Lines[term.Lines.Length - 1];
                                if (status.Text != txt + $" [{progress.Value}%]")
                                {
                                    status.Text = txt + $" [{progress.Value}%]";
                                    status.Layout(new Microsoft.Xna.Framework.GameTime());
                                    status.X = (GraphicsSubsystem.UIManager.Viewport.Width - status.Width) / 2;
                                }
                            }
                            else
                            {
                                Thread.Sleep(10);
                            }
                        }
                        catch { }
                    }
                });
                nt.Start();
                SlowWriteLine("Formatting storage device A with ShiftFS version 4.7...");

                progress.Maximum = 100;
                for (int i = 0; i < 100; i++)
                {
                    progress.Value = i;
                    Thread.Sleep(25);
                }
                progress.Value = 0;
                Console.WriteLine("Mounting storage device A to 0:/");
                Thread.Sleep(135);
                Console.WriteLine("Writing system files & directories to sentience...");
                Thread.Sleep(450);
                foreach (var dir in Paths.GetAllWithoutKey())
                {
                    try
                    {
                        if (dir.StartsWith("0:/"))
                        {
                            Console.WriteLine("Writing: " + dir);
                            Thread.Sleep(30);
                        }
                    }
                    catch { }
                }
                Thread.Sleep(1250);
                Console.WriteLine("Generating system profile...");
                Thread.Sleep(25);
                Console.WriteLine("Codepoints: 0");
                save.Codepoints = 0;
                progress.Value = 10;
                Thread.Sleep(25);
                Console.WriteLine("Shiftorium cache generated...");
                save.Upgrades = new Dictionary<string, bool>();
                progress.Value = 20;
                Console.WriteLine("Performing even more redundant tasks that have already been done for us by the backend because we're redundant programmers who don't know our own code... jesus christ this is a long string...");
                save.ID = Guid.NewGuid();
                save.IsSandbox = false;
                save.Language = "english";
                save.MusicEnabled = true;
                save.MusicVolume = 100;
                save.PickupPoint = "";
                save.ShiftnetSubscription = 0;
                save.SoundEnabled = true;
                save.StoriesExperienced = new List<string>();
                Thread.Sleep(2000);
                Console.WriteLine("Far out. We haven't asked the user for a hostname.");
                bool hostnameEntered = false;
                Engine.Infobox.PromptText("Enter system hostname", "You need a hostname to exist within the Digital Society. Your hostname allows other sentiences to interact with you. Please enter one.", (hostname) =>
                {
                    hostnameEntered = true;
                    save.SystemName = hostname;
                });
                int ticks = 0;
                while(hostnameEntered == false)
                {
                    Thread.Sleep(10);
                    ticks++;
                    if(ticks == 1000)
                    {
                        Console.WriteLine("<kernel> [growls] COME ON. THE BOX IS RIGHT THERE. PICK A DARN HOSTNAME ALREADY.");
                    }
                    if(ticks == 1250)
                    {
                        Console.WriteLine("<kernel> [screaming expletives] WHAT ARE YOU WAITING FOR? WHAT DO YOU WANT?");
                    }
                    if(ticks == 1500)
                    {
                        Console.WriteLine("<kernel> [someone pissed in its cereal] For crying out loud I'm about ready to default you to localhost and prevent you from accessing the Digital Society.");
                    }
                }
                progress.Value = 100;
                save.ViralInfections = new List<ViralInfection>();
                Console.WriteLine("<kernel> Setup complete. You're ready.");
                Thread.Sleep(500);
                save.StoryPosition = 123456789; //HERE. YOU DO THE MATH.
                SaveSystem.CurrentSave = save;
                SaveSystem.SaveGame();
                GraphicsSubsystem.UIManager.StopHandling(progress);
                GraphicsSubsystem.UIManager.StopHandling(term);
                GraphicsSubsystem.UIManager.StopHandling(status);
                GraphicsSubsystem.UIManager.StopHandling(_shiftos);

            });
            t.Start();
        }
    }
}
