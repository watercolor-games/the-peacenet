using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;
using static ShiftOS.Engine.SkinEngine;

namespace ShiftOS.Frontend.Apps
{
    [FileHandler("Shell script", ".trm", "fileicontrm")]
    [Launcher("{TITLE_TERMINAL}", false, null, "{AL_UTILITIES}")]
    [WinOpen("{WO_TERMINAL}")]
    [DefaultTitle("{TITLE_TERMINAL}")]
    [DefaultIcon("iconTerminal")]
    public class Terminal : GUI.Control, IShiftOSWindow
    {
        private TerminalControl _terminal = null;

        public Terminal()
        {
            Width = 493;
            Height = 295;
        }

        public void OnLoad()
        {
            _terminal = new Apps.TerminalControl();
            _terminal.Dock = GUI.DockStyle.Fill;
            AddControl(_terminal);
            AppearanceManager.ConsoleOut = _terminal;
            AppearanceManager.StartConsoleOut();
            TerminalBackend.PrintPrompt();
            SaveSystem.GameReady += () =>
            {
                Console.WriteLine("[sessionmgr] Starting system UI...");
                AppearanceManager.SetupWindow(new SystemStatus());
                TerminalBackend.PrintPrompt();
            };
        }

        protected override void OnLayout(GameTime gameTime)
        {
            if (ContainsFocusedControl || IsFocusedControl)
                AppearanceManager.ConsoleOut = _terminal;
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
    }

    public class TerminalControl : GUI.TextInput, ITerminalWidget
    {
        public TerminalControl()
        {
            Dock = GUI.DockStyle.Fill;
            
        }

        public string[] Lines
        {
            get
            {
                return Text.Split(new[] { "\r\n" }, StringSplitOptions.None);

            }
        }

        public void Clear()
        {
            Text = "";
            Index = 0;
            _vertOffset = 0;
            Invalidate();
        }

        public void SelectBottom()
        {
           Index = Text.Length - 1;
            RecalculateLayout();
            InvalidateTopLevel();
        }

        

        public void Write(string text)
        {
            Engine.Desktop.InvokeOnWorkerThread(() =>
            {
                Text += Localization.Parse(text);
                SelectBottom();
                Index += text.Length;
                RecalculateLayout();
                InvalidateTopLevel();
            });
        }

        public void WriteLine(string text)
        {
            Write(text + Environment.NewLine);
        }

        
        public int GetCurrentLine()
        {
            int line = 0;
            for(int i = 0; i < Index; i++)
            {
                if(Text[i]=='\n')
                {
                    line++;
                    continue;
                }
            }
            return line;
        }

        float _vertOffset = 0.0f;

        protected void RecalculateLayout()
        {
        }

        private bool blinkStatus = false;
        private double blinkTime = 0.0;

        protected override void OnLayout(GameTime gameTime)
        {
            blinkTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (blinkTime > 500.0)
                blinkTime = 0;
            bool prev = blinkStatus;
            blinkStatus = blinkTime > 250.0;
            if (prev != blinkStatus)
                Invalidate();
        }

        /// <summary>
        /// Gets the X and Y coordinates (in pixels) of the caret.
        /// </summary>
        /// <param name="gfx">A <see cref="System.Drawing.Graphics"/> object used for font measurements</param>
        /// <returns>An absolute fucking mess. Seriously, can someone fix this method so it uhh WORKS PROPERLY?</returns>
        public System.Drawing.Point GetPointAtIndex(Graphics gfx)
        {
            int vertMeasure = 2;
            int horizMeasure = 2;
            int lineindex = 0;
            int line = GetCurrentLine();
            for (int l = 0; l < line; l++)
            {
                lineindex += Lines[l].Length;
                var stringMeasure = gfx.SmartMeasureString(Lines[l] == "\r" ? " " : Lines[l], LoadedSkin.TerminalFont, Width - 4);
                vertMeasure += (int)stringMeasure.Height;

            }
            var lnMeasure = gfx.SmartMeasureString(Text.Substring(lineindex, Index - lineindex), LoadedSkin.TerminalFont);
            int w = (int)Math.Floor(lnMeasure.Width);
            while (w > Width - 4)
            {
                w = w - (Width - 4);
                vertMeasure += (int)lnMeasure.Height;
            }
            horizMeasure += w;
            return new System.Drawing.Point(horizMeasure, vertMeasure);
        }

        private PointF CaretPosition = new PointF(2, 2);
        private Size CaretSize = new Size(2, 15);

        protected override void OnKeyEvent(KeyEvent a)
        {
            if (a.Key == Keys.Enter)
            {
                try
                {
                    if (!TerminalBackend.PrefixEnabled)
                    {
                        string textraw = Lines[Lines.Length - 1];
                        TerminalBackend.SendText(textraw);
                        return;
                    }
                    var text = Lines;
                    var text2 = text[text.Length - 1];
                    var text3 = "";
                    var text4 = Regex.Replace(text2, @"\t|\n|\r", "");
                        WriteLine("");

                        if (TerminalBackend.PrefixEnabled)
                        {
                            text3 = text4.Remove(0, $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ".Length);
                        }
                    if (!string.IsNullOrWhiteSpace(text3))
                    {
                        TerminalBackend.LastCommand = text3;
                        TerminalBackend.SendText(text4);
                        if (TerminalBackend.InStory == false)
                        {
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
                    }
                }
                catch
                {
                }
                finally
                {
                    if (TerminalBackend.PrefixEnabled)
                    {
                        TerminalBackend.PrintPrompt();
                    }
                    AppearanceManager.CurrentPosition = 0;

                }
            }
            else if (a.Key == Keys.Back)
            {
                try
                {
                    var tostring3 = Lines[Lines.Length - 1];
                    var tostringlen = tostring3.Length + 1;
                    var workaround = $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ";
                    var derp = workaround.Length + 1;
                    if (tostringlen != derp)
                    {
                        AppearanceManager.CurrentPosition--;
                        base.OnKeyEvent(a);
                        RecalculateLayout();
                        InvalidateTopLevel();
                    }
                }
                catch
                {
                    Debug.WriteLine("Drunky alert in terminal.");
                }
            }
            else if(a.Key == Keys.Right)
            {
                if(Index < Text.Length)
                {
                    Index++;
                    AppearanceManager.CurrentPosition++;
                    RecalculateLayout();
                    InvalidateTopLevel();
                }
            }
            else if (a.Key == Keys.Left)
            {
                if (SaveSystem.CurrentSave != null)
                {
                    var getstring = Lines[Lines.Length - 1];
                    var stringlen = getstring.Length + 1;
                    var header = $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ";
                    var headerlen = header.Length + 1;
                    var selstart = Index;
                    var remstrlen = Text.Length - stringlen;
                    var finalnum = selstart - remstrlen;

                    if (finalnum > headerlen)
                    {
                        AppearanceManager.CurrentPosition--;
                        base.OnKeyEvent(a);
                    }
                }
            }
            else if (a.Key == Keys.Up)
            {
                var tostring3 = Lines[Lines.Length - 1];
                if (tostring3 == $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ")
                    Console.Write(TerminalBackend.LastCommand);
                ConsoleEx.OnFlush?.Invoke();
                return;

            }
            else
            {
                if (TerminalBackend.InStory)
                {
                    return;
                }
                if (a.KeyChar != '\0')
                {
                    Text = Text.Insert(Index, a.KeyChar.ToString());
                    Index++;
                    AppearanceManager.CurrentPosition++;
//                    RecalculateLayout();
                    InvalidateTopLevel();
                }
            }
            blinkStatus = true;
            blinkTime = 250;
        }

        protected override void OnPaint(GraphicsContext gfx)
        { 
            gfx.Clear(LoadedSkin.TerminalBackColorCC.ToColor().ToMonoColor());
            if (!string.IsNullOrEmpty(Text))
            {
                //Draw the caret.
                if (blinkStatus == true)
                {
                    PointF cursorPos;
                    using (var cgfx = System.Drawing.Graphics.FromImage(new System.Drawing.Bitmap(1, 1)))
                    {
                        cursorPos = GetPointAtIndex(cgfx);

                    }
                    var cursorSize = gfx.MeasureString(Text[Index - 1].ToString(), LoadedSkin.TerminalFont);

                    var lineMeasure = gfx.MeasureString(Lines[GetCurrentLine()], LoadedSkin.TerminalFont);
                    if (cursorPos.X > lineMeasure.X)
                    {
                        cursorPos.X = lineMeasure.X;
                    }

                    gfx.DrawRectangle((int)cursorPos.X, (int)cursorPos.Y - (int)_vertOffset, (int)cursorSize.X, (int)cursorSize.Y, LoadedSkin.TerminalForeColorCC.ToColor().ToMonoColor());
                }
                //Draw the text


                gfx.DrawString(Text, 2, 2 - (int)Math.Floor(_vertOffset), LoadedSkin.TerminalForeColorCC.ToColor().ToMonoColor(), LoadedSkin.TerminalFont, Width - 4);
            }
        }

    }

    public static class ConsoleColorExtensions
    {
        public static System.Drawing.Color ToColor(this ConsoleColor cc)
        {
            switch (cc)
            {
                case ConsoleColor.Black:
                    return System.Drawing.Color.Black;
                case ConsoleColor.Blue:
                    return System.Drawing.Color.Blue;
                case ConsoleColor.Cyan:
                    return System.Drawing.Color.Cyan;
                case ConsoleColor.DarkBlue:
                    return System.Drawing.Color.DarkBlue;
                case ConsoleColor.DarkCyan:
                    return System.Drawing.Color.DarkCyan;
                case ConsoleColor.DarkGray:
                    return System.Drawing.Color.DarkGray;
                case ConsoleColor.DarkGreen:
                    return System.Drawing.Color.DarkGreen;
                case ConsoleColor.DarkMagenta:
                    return System.Drawing.Color.DarkMagenta;
                case ConsoleColor.DarkRed:
                    return System.Drawing.Color.DarkRed;
                case ConsoleColor.DarkYellow:
                    return System.Drawing.Color.Orange;
                case ConsoleColor.Gray:
                    return System.Drawing.Color.Gray;
                case ConsoleColor.Green:
                    return System.Drawing.Color.Green;
                case ConsoleColor.Magenta:
                    return System.Drawing.Color.Magenta;
                case ConsoleColor.Red:
                    return System.Drawing.Color.Red;
                case ConsoleColor.White:
                    return System.Drawing.Color.White;
                case ConsoleColor.Yellow:
                    return System.Drawing.Color.Yellow;
            }
            return System.Drawing.Color.Empty;
        }
    }

    public static class GraphicsExtensions
    {
        public static SizeF SmartMeasureString(this Graphics gfx, string s, Font font, int width)
        {
            if (string.IsNullOrEmpty(s))
                s = " ";
            var textformat = new StringFormat(StringFormat.GenericTypographic);
            textformat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            //textformat.Trimming = StringTrimming.Character;
            //textformat.FormatFlags |= StringFormatFlags.NoClip;
            
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            var measure = gfx.MeasureString(s, font, width, textformat);
            return new SizeF((float)Math.Ceiling(measure.Width), (float)Math.Ceiling(measure.Height));
        }

        public static SizeF SmartMeasureString(this Graphics gfx, string s, Font font)
        {
            return SmartMeasureString(gfx, s, font, int.MaxValue);
        }

    }
}
