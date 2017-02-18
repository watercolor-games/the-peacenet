using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine.Scripting;
using System.Drawing;
using ShiftOS.Engine;

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
    }

    [Exposed("clr")]
    public class CommonLanguageRuntimeInterop
    {
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
