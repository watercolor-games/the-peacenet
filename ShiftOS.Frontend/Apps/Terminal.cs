using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;
using static Plex.Engine.SkinEngine;

namespace Plex.Frontend.Apps
{
    [FileHandler("Shell script", ".trm", "fileicontrm")]
    [Launcher("{TITLE_TERMINAL}", false, null, "{AL_UTILITIES}")]
    [WinOpen("{WO_TERMINAL}")]
    [DefaultTitle("{TITLE_TERMINAL}")]
    [DefaultIcon("iconTerminal")]
    public class Terminal : GUI.Control, IPlexWindow
    {
        private TerminalControl _terminal = null;
        
        public TerminalControl TerminalControl
        {
            get
            {
                return _terminal;
            }
        }

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
        private int _zoomFactor = 1;

        public TerminalControl()
        {
            Dock = GUI.DockStyle.Fill;
            
        }
        
        private static readonly string[] delimiters = { Environment.NewLine };

        public string[] Lines
        {
            get
            {
				return Text.Split(delimiters, StringSplitOptions.None);
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
           Index = Text.Length;
            RecalculateLayout();
            InvalidateTopLevel();
        }

        

        public void Write(string text)
        {
            Engine.Desktop.InvokeOnWorkerThread(() =>
            {
                Text += Localization.Parse(text);
                SelectBottom();
                InvalidateTopLevel();
            });
        }

        public void WriteLine(string text)
        {
            Write(text + Environment.NewLine);
        }

        
        private static readonly Regex regexNl = new Regex(Regex.Escape(Environment.NewLine));
        
        public int GetCurrentLine()
        {
            return regexNl.Matches(Text.Substring(0, Index)).Count;
        }

        float _vertOffset = 0.0f;
        System.Drawing.Point cloc = System.Drawing.Point.Empty;
        Vector2 csize = Vector2.Zero;

        protected void RecalculateLayout()
        {
            cloc = GetPointAtIndex();
            csize = GraphicsContext.MeasureString("#", new Font(LoadedSkin.TerminalFont.Name, LoadedSkin.TerminalFont.Size * _zoomFactor, LoadedSkin.TerminalFont.Style), Engine.GUI.TextAlignment.TopLeft);
            if (cloc.Y - _vertOffset < 0)
            {
                _vertOffset += cloc.Y - _vertOffset;
            }
            while ((cloc.Y + csize.Y) - _vertOffset > Height)
            {
                _vertOffset += csize.Y;
            }
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

        public bool ReadOnly = false;

        /// <summary>
        /// Gets the X and Y coordinates (in pixels) of the caret.
        /// </summary>
        /// <param name="gfx">A <see cref="System.Drawing.Graphics"/> object used for font measurements</param>
        /// <returns>the correct position of the d*ng caret. yw</returns>
        public System.Drawing.Point GetPointAtIndex()
        {
			if (string.IsNullOrEmpty(Text))
                return new System.Drawing.Point(2, 2);
            var font = new Font(LoadedSkin.TerminalFont.Name, LoadedSkin.TerminalFont.Size * _zoomFactor, LoadedSkin.TerminalFont.Style);
            int currline = GetCurrentLine();
            string substring = String.Join(Environment.NewLine, Lines.Take(currline + 1));
			int h = (int)Math.Round(GraphicsContext.MeasureString(substring, font, Engine.GUI.TextAlignment.TopLeft, Width).Y - font.Height);

            int linestart = String.Join(Environment.NewLine, Lines.Take(GetCurrentLine())).Length;

            var lineMeasure = GraphicsContext.MeasureString(Text.Substring(linestart, Index - linestart), font, Engine.GUI.TextAlignment.TopLeft);
            int w = (int)Math.Floor(lineMeasure.X);
            while (w > Width)
            {
                w -= Width;
                h += (int)lineMeasure.Y;
            }
            return new System.Drawing.Point(w, h);
        }

        private PointF CaretPosition = new PointF(2, 2);
        private Size CaretSize = new Size(2, 15);
        private bool doEnter = true;

        protected override void OnKeyEvent(KeyEvent a)
        {
            if (a.Key != Keys.Enter)
                doEnter = true;
            if (a.ControlDown && (a.Key == Keys.OemPlus || a.Key == Keys.Add))
            {
                _zoomFactor *= 2;
                RecalculateLayout();
                Invalidate();
                return;
            }

            if (a.ControlDown && (a.Key == Keys.OemMinus || a.Key == Keys.Subtract))
            {
                _zoomFactor = Math.Max(1, _zoomFactor / 2);
                RecalculateLayout();
                Invalidate();
                return;
            }


            if (a.Key == Keys.Enter && !ReadOnly)
            {
                if (!PerformTerminalBehaviours)
                {
                    Text = Text.Insert(Index, Environment.NewLine);
                    Index += 2;
                    RecalculateLayout();
                    Invalidate();
                    return;
                }

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
                        text3 = text4.Remove(0, TerminalBackend.ShellOverride.Length);
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

            else if (a.Key == Keys.Back && !ReadOnly)
            {
                try
                {
                    if (PerformTerminalBehaviours)
                    {
                        var tostring3 = Lines[Lines.Length - 1];
                        var tostringlen = tostring3.Length + 1;
                        var workaround = TerminalBackend.ShellOverride;
                        var derp = workaround.Length + 1;
                        if (tostringlen != derp)
                        {
                            AppearanceManager.CurrentPosition--;
                            base.OnKeyEvent(a);
                            cloc = GetPointAtIndex();
                            InvalidateTopLevel();
                        }
                    }
                }
                catch
                {
                    Debug.WriteLine("Drunky alert in terminal.");
                }
            }
            else if (a.Key == Keys.Right)
            {
                if (Index < Text.Length)
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
                    var header = TerminalBackend.ShellOverride;
                    var headerlen = header.Length + 1;
                    var selstart = Index;
                    var remstrlen = Text.Length - stringlen;
                    var finalnum = selstart - remstrlen;
                    if (!PerformTerminalBehaviours)
                        headerlen = 0;
                    if (finalnum > headerlen)
                    {
                        AppearanceManager.CurrentPosition--;
                        base.OnKeyEvent(a);
                    }
                }
            }
            else if (a.Key == Keys.Up && PerformTerminalBehaviours)
            {
                var tostring3 = Lines[Lines.Length - 1];
                if (tostring3 == TerminalBackend.ShellOverride)
                    Console.Write(TerminalBackend.LastCommand);
                ConsoleEx.OnFlush?.Invoke();
                return;

            }
            else
            {
                if ((PerformTerminalBehaviours && TerminalBackend.InStory) || ReadOnly)
                {
                    return;
                }
                if (a.KeyChar != '\0')
                {
                    Text = Text.Insert(Index, a.KeyChar.ToString());
                    Index++;
                    AppearanceManager.CurrentPosition++;
                    cloc = GetPointAtIndex();
                    Invalidate();
                }
            }
            blinkStatus = true;
            blinkTime = 250;
        }

        public bool PerformTerminalBehaviours = true;

        protected override void RenderText(GraphicsContext gfx)
        {
            int textloc = 0 - (int)_vertOffset;
            var font = LoadedSkin.TerminalFont;
            foreach (var line in Lines)
            {
                if (!(textloc < 0 || textloc - font.Height >= Height))
                    gfx.DrawString(line, 0, textloc, LoadedSkin.TerminalForeColorCC.ToColor().ToMonoColor(), font, Engine.GUI.TextAlignment.TopLeft, Width - 4);
                if (string.IsNullOrEmpty(line))
                    textloc += font.Height;
                else
                    textloc += (int)GraphicsContext.MeasureString(line, font, Engine.GUI.TextAlignment.TopLeft, Width).Y;
            }

        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            PaintBackground = false;
            gfx.Clear(LoadedSkin.TerminalBackColorCC.ToColor().ToMonoColor());

            var font = new System.Drawing.Font(LoadedSkin.TerminalFont.Name, LoadedSkin.TerminalFont.Size * _zoomFactor, LoadedSkin.TerminalFont.Style); 
            if (!string.IsNullOrEmpty(Text))
            {
                //Draw the caret.
                if (blinkStatus == true)
                {
                    gfx.DrawRectangle((int)cloc.X, (int)(cloc.Y - _vertOffset), (int)csize.X, (int)csize.Y, LoadedSkin.TerminalForeColorCC.ToColor().ToMonoColor());
                }
                //Draw the text
                base.OnPaint(gfx, target);

            }
        }

    }


    public static class GraphicsExtensions
    {

        [Obsolete("Use GraphicsContext.MeasureString instead")]
        public static SizeF SmartMeasureString(this Graphics gfx, string s, Font font, int width)
        {
            var measure = System.Windows.Forms.TextRenderer.MeasureText(s, font, new Size(width, int.MaxValue));
            return measure;
        }

        [Obsolete("Use GraphicsContext.MeasureString instead")]
        public static SizeF SmartMeasureString(this Graphics gfx, string s, Font font)
        {
            return SmartMeasureString(gfx, s, font, int.MaxValue);
        }

    }
}
