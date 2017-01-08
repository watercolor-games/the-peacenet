#define MUD_RAPIDDEV

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using static ShiftOS.Engine.SaveSystem;

namespace ShiftOS.Engine
{
    public static class AppearanceManager
    {
        [Obsolete("Please use Localization.GetAllLanguages().")]
        public static string[] GetLanguages()
        {
            return Localization.GetAllLanguages();
        }

        public static void AddFocusEvents(Control ctrl, Control child)
        {
            child.Enter += (o, a) =>
            {
                ctrl.BringToFront();
            };
            child.MouseDown += (o, a) =>
            {
                ctrl.BringToFront();
            };

            foreach (Control c in child.Controls)
            {
                c.Enter += (o, a) =>
                {
                    ctrl.BringToFront();
                };
                c.MouseDown += (o, a) =>
                {
                    ctrl.BringToFront();
                };

                try
                {
                    AddFocusEvents(ctrl, c);
                }
                catch { }
            }
        }


        public static string LastTerminalText { get; set; }
        public static int CurrentPosition { get; set; }
        public static int LastLength { get; set; }


        public static void Minimize(IWindowBorder form)
        {
            winmgr.Minimize(form);
        }

        public static void Maximize(IWindowBorder form)
        {
            winmgr.Maximize(form);
        }


        
        public static List<IWindowBorder> OpenForms = new List<IWindowBorder>();

        public static bool CanOpenWindow(IShiftOSWindow form)
        {
#if !MUD_RAPIDDEV
            if (ServerManager.IsSingleplayer)
            {
                foreach (var attr in form.GetType().GetCustomAttributes(false))
                {
                    if (attr is MultiplayerOnlyAttribute)
                        return false;
                }
            }
#endif
            return true;
        }

        public static void SetupWindow(IShiftOSWindow form)
        {
            winmgr.SetupWindow(form);
            Desktop.ResetPanelButtons();
        }

        public static void Close(IShiftOSWindow win)
        {
            winmgr.Close(win);
            Desktop.ResetPanelButtons();
        }

        public static void SetupDialog(IShiftOSWindow form)
        {
            winmgr.SetupDialog(form);
            Desktop.ResetPanelButtons();
        }

        private static WindowManager winmgr = null;

        public static double Measure(this string text, Font font)
        {
            return Graphics.FromImage(new Bitmap(1, 1)).MeasureString(text, font).Width;
        }

        public static void Initiate(WindowManager mgr)
        {
            winmgr = mgr;
        }

        [Obsolete("This is a stub.")]
        public static void DoWinformsSkinningMagicOnWpf(this UserControl ctrl)
        {
            //SetupControls(ctrl);
        }

        public static event EmptyEventHandler OnExit;

        internal static void Exit()
        {
            OnExit?.Invoke();
            //disconnect from MUD
            ServerManager.Disconnect();
        }


        internal static bool BordersHidden(Form frm)
        {
            string t = frm.Tag as string;
            if (t == null)
                return false;

            return t.Contains("hidden");
        }

        public static ITerminalWidget ConsoleOut { get; set; }

        public static void StartConsoleOut()
        {
            Console.SetOut(new TerminalTextWriter());
        }

        public static void Invoke(Action act)
        {
            winmgr.InvokeAction(act);
        }
    }

    public interface ITerminalWidget
    {
        void Write(string text);
        void WriteLine(string text);
        void Clear();
        void SelectBottom();
    }

    public abstract class WindowManager
    {
        public abstract void Minimize(IWindowBorder border);
        public abstract void Maximize(IWindowBorder border);

        public abstract void Close(IShiftOSWindow win);

        public abstract void SetupWindow(IShiftOSWindow win);
        public abstract void SetupDialog(IShiftOSWindow win);

        public abstract void InvokeAction(Action act);
    }

    public interface IWindowBorder
    {
        void Close();
        string Text { get; set; }
        IShiftOSWindow ParentWindow { get; set; }
    }
}
