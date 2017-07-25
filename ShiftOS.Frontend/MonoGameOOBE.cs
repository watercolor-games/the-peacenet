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

                GraphicsSubsystem.UIManager.StopHandling(term);
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


            });
            t.Start();
        }
    }
}
