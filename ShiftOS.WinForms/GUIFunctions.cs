using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine.Scripting;
using System.Drawing;
using ShiftOS.Engine;
using System.Windows.Forms;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms
{
    [Exposed("gui")]
    public class GUIFunctions
    {
        public dynamic color(int r, int g, int b)
        {
            return Color.FromArgb(r, g, b);
        }

        public dynamic color(string name)
        {
            return Color.FromName(name);
        }

        public dynamic point(int x, int y)
        {
            return new Point(x, y);
        }

        public dynamic size(int w, int h)
        {
            return new Size(w, h);
        }

        public dynamic font(string name, float size, bool bold, bool italic, bool strikethrough, bool underline)
        {
            FontStyle fs = FontStyle.Regular;
            if (bold)
                fs = fs | FontStyle.Bold;
            if (italic)
                fs = fs | FontStyle.Italic;
            if (underline)
                fs = fs | FontStyle.Underline;
            if (strikethrough)
                fs = fs | FontStyle.Strikeout;

            return new Font(name, size, fs);
        }

        public dynamic createWindow(string name, dynamic size)
        {
            var win = new Window();
            win.Size = size;
            AppearanceManager.SetupWindow(win);
            return win;
        }

        public dynamic panel(Control parent, Point loc, Size size)
        {
            var pnl = new Panel();
            pnl.Location = loc;
            pnl.Size = size;
            parent.Controls.Add(pnl);
            pnl.Show();
            return pnl;
        }

        public DockStyle dock(string type)
        {
            DockStyle d = DockStyle.None;
            switch (type.ToLower())
            {
                case "top":
                    d = DockStyle.Top;
                    break;
                case "bottom":
                    d = DockStyle.Bottom;
                    break;
                case "left":
                    d = DockStyle.Left;
                    break;
                case "right":
                    d = DockStyle.Right;
                    break;
                case "fill":
                    d = DockStyle.Fill;
                    break;
            }
            return d;
        }

        public AnchorStyles anchor(bool top, bool left, bool bottom, bool right)
        {
            AnchorStyles a = AnchorStyles.None;
            if (top)
                a = a | AnchorStyles.Top;
            if (left)
                a = a | AnchorStyles.Left;
            if (bottom)
                a = a | AnchorStyles.Bottom;
            if (right)
                a = a | AnchorStyles.Right;
            return a;
        }

        public dynamic flow(Control parent, Point loc, Size size)
        {
            var pnl = new FlowLayoutPanel();
            pnl.Size = size;
            pnl.Location = loc;
            parent.Controls.Add(pnl);
            pnl.Show();
            return pnl;
        }

        private Control addToParent(Control parent, Point loc, Size size, Control newControl)
        {
            newControl.Size = size;
            newControl.Location = loc;
            parent.Controls.Add(newControl);
            newControl.Show();
            ControlManager.SetupControls(parent);
            return newControl;
        }

        public dynamic button(Control parent, Point loc, Size size, string text)
        {
            var btn = new Button();
            btn.Text = text;
            btn.AutoSize = false;
            return addToParent(parent, loc, size, btn);
        }

        public dynamic label(Control parent, Point loc, Size size, string text)
        {
            var lbl = new Label();
            lbl.Text = text;
            lbl.AutoSize = false;
            return addToParent(parent, loc, size, lbl);
        }

        public dynamic menu(Control parent)
        {
            var mnu = new MenuStrip();
            parent.Controls.Add(mnu);
            mnu.Dock = DockStyle.Top;
            mnu.Show();
            return mnu;
        }

        public dynamic menuitem(MenuStrip parent, string text, Action onClick = null)
        {
            var mitem = new ToolStripMenuItem();
            parent.Items.Add(mitem);
            mitem.Text = text;
            mitem.Click += (o, a) =>
            {
                onClick?.Invoke();
            };
            return mitem;
        }

        public dynamic menuitem(ToolStripMenuItem parent, string text, Action onClick = null)
        {
            var mitem = new ToolStripMenuItem();
            parent.DropDownItems.Add(mitem);
            mitem.Text = text;
            mitem.Click += (o, a) =>
            {
                onClick?.Invoke();
            };
            return mitem;
        }

        public dynamic textbox(Control parent, Point loc, int width)
        {
            var txt = new TextBox();
            txt.Location = loc;
            txt.Width = width;
            parent.Controls.Add(txt);
            txt.Show();
            ControlManager.SetupControls(parent);
            return txt;
        }

        public dynamic timer(int interval, Action elapsed)
        {
            var tmr = new System.Windows.Forms.Timer();
            tmr.Interval = interval;
            tmr.Tick += (o, a) =>
            {
                elapsed?.Invoke();
            };
            return tmr;
        }
    }

    [Exposed("clr")]
    public class CommonLanguageRuntimeInterop
    {
        public void tryCode(Action code, Action<Exception> error)
        {
            try
            {
                code?.Invoke();
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
            }
        }

        public void throwError(string error)
        {
            throw new Exception(error);
        }


        public dynamic construct(Type type, dynamic[] ctorParams)
        {
            return Activator.CreateInstance(type, ctorParams);
        }

        public dynamic typeOf(string typeName)
        {
            return Type.GetType(typeName);
        }

        public void throwException(string message)
        {
            throw new UserException(message);
        }
    }

    public class UserException : Exception
    {
        public UserException(string message) :base("User threw exception using clr:throwException().\r\n\r\n" + message)
        {

        }
    }
}
