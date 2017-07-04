using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _terminal.Layout();
            AppearanceManager.ConsoleOut = _terminal;
            AppearanceManager.StartConsoleOut();
        }

        protected override void OnLayout()
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
        public string[] Lines
        {
            get
            {
                return Text.Split(new[] { "\n" }, StringSplitOptions.None);

            }
        }

        public void Clear()
        {
            Text = "";
            Index = 0;
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
                SelectBottom();
                Text = Text.Insert(Index, text);
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
            for(int i = 0; i < Text.Length; i++)
            {
                if(Text[i]=='\n')
                {
                    line++;
                    continue;
                }
                if (i == Index)
                    return line;
            }
            return 0;
        }

        float _vertOffset = 0.0f;

        protected void RecalculateLayout()
        {
            if(!string.IsNullOrEmpty(Text))
            using (var gfx = Graphics.FromImage(new Bitmap(1, 1)))
            {
                var cursorpos = GetPointAtIndex(gfx);
                    var caretSize = gfx.SmartMeasureString(Text.ToString(), LoadedSkin.TerminalFont, Width - 4);
                    float initial = ((caretSize.Height) - cursorpos.Y) - _vertOffset;
                    if (initial < 0)
                {
                    float difference = initial - Height;
                    _vertOffset += initial + difference;
                }
                if (initial > Height)
                {
                    float difference = initial - Height;
                    _vertOffset -= initial - difference;
                }

            }
        }

        /// <summary>
        /// Gets the X and Y coordinates (in pixels) of the caret.
        /// </summary>
        /// <param name="gfx">A <see cref="System.Drawing.Graphics"/> object used for font measurements</param>
        /// <returns>An absolute fucking mess. Seriously, can someone fix this method so it uhh WORKS PROPERLY?</returns>
        public PointF GetPointAtIndex(Graphics gfx)
        {
            float vertMeasure = 2.0f;
            float horizMeasure = 2.0f;
            int lineindexes = 0;
            for (int l = 0; l <= GetCurrentLine(); l++)
            { 
                var measure = gfx.SmartMeasureString(Lines[l], LoadedSkin.TerminalFont, Width - 4);
                vertMeasure += measure.Width;
                if(l == GetCurrentLine())
                {
                    string _linetext = Text.Substring(lineindexes, Index - lineindexes);
                    var lMeasure = gfx.SmartMeasureString(_linetext, LoadedSkin.TerminalFont);
                    horizMeasure = lMeasure.Width;
                    if (horizMeasure > Width - 4)
                        horizMeasure -= (Width-4);
                }
                else
                {
                    lineindexes += Lines[l].Length;
                }
            }
            return new PointF(horizMeasure, vertMeasure);
        }

        protected override void OnKeyEvent(KeyEvent e)
        {
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                Text = Text.Insert(Index, "\r\n");
                Index++;
            }
            base.OnKeyEvent(e);
            RecalculateLayout();
            InvalidateTopLevel();
        }

        protected override void OnPaint(Graphics gfx)
        {
            RecalculateLayout();
            gfx.Clear(LoadedSkin.TerminalBackColorCC.ToColor());
            if (!string.IsNullOrEmpty(Text))
            {
                //Draw the caret.
                var caretPos = GetPointAtIndex(gfx);
                var caretSize = gfx.SmartMeasureString(Text[Index - 1].ToString(), LoadedSkin.TerminalFont);
                if (IsFocusedControl)
                {
                    gfx.FillRectangle(new SolidBrush(LoadedSkin.TerminalForeColorCC.ToColor()), new RectangleF(new PointF(caretPos.X, caretPos.Y - _vertOffset), new SizeF(2, caretSize.Height)));
                }//Draw the text
                var textMeasure = gfx.MeasureString(Text, LoadedSkin.TerminalFont, Width - 4);
                gfx.DrawString(Text, LoadedSkin.TerminalFont, new SolidBrush(LoadedSkin.TerminalForeColorCC.ToColor()), 2, 2 - _vertOffset);
                

            }
        }

    }

    public static class ConsoleColorExtensions
    {
        public static Color ToColor(this ConsoleColor cc)
        {
            switch (cc)
            {
                case ConsoleColor.Black:
                    return Color.Black;
                case ConsoleColor.Blue:
                    return Color.Blue;
                case ConsoleColor.Cyan:
                    return Color.Cyan;
                case ConsoleColor.DarkBlue:
                    return Color.DarkBlue;
                case ConsoleColor.DarkCyan:
                    return Color.DarkCyan;
                case ConsoleColor.DarkGray:
                    return Color.DarkGray;
                case ConsoleColor.DarkGreen:
                    return Color.DarkGreen;
                case ConsoleColor.DarkMagenta:
                    return Color.DarkMagenta;
                case ConsoleColor.DarkRed:
                    return Color.DarkRed;
                case ConsoleColor.DarkYellow:
                    return Color.Orange;
                case ConsoleColor.Gray:
                    return Color.Gray;
                case ConsoleColor.Green:
                    return Color.Green;
                case ConsoleColor.Magenta:
                    return Color.Magenta;
                case ConsoleColor.Red:
                    return Color.Red;
                case ConsoleColor.White:
                    return Color.White;
                case ConsoleColor.Yellow:
                    return Color.Yellow;
            }
            return Color.Empty;
        }
    }

    public static class GraphicsExtensions
    {
        public static SizeF SmartMeasureString(this Graphics gfx, string s, Font font, int width)
        {
            if (string.IsNullOrEmpty(s))
                s = " ";
            var textformat = new StringFormat(StringFormat.GenericTypographic);
            textformat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
            textformat.Trimming = StringTrimming.None;
            return gfx.MeasureString(s, font, width, textformat);
        }

        public static SizeF SmartMeasureString(this Graphics gfx, string s, Font font)
        {
            if (string.IsNullOrEmpty(s))
                s = " ";
            var textformat = new StringFormat(StringFormat.GenericTypographic);
            textformat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
            textformat.Trimming = StringTrimming.None;
            return gfx.MeasureString(s, font, int.MaxValue, textformat);
        }

    }
}
