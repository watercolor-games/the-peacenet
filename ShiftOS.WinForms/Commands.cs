using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ShiftOS.WinForms
{
    [Namespace("coherence")]
    [RequiresUpgrade("kernel_coherence")]
    public static class CoherenceCommands
    {
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const UInt32 SWP_SHOWWINDOW = 0x0040;

        

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [Command("launch", "process: \"C:\\path\\to\\process\" - The process path to launch.", "Launch a process inside kernel coherence.")]
        [RequiresArgument("process")]
        public static bool LaunchApp(Dictionary<string, object> args)
        {
            string process = args["process"].ToString();
            var prc = Process.Start(process);
            StartCoherence(prc);
            return true;
        }

        private static void StartCoherence(Process prc)
        {
            RECT rct = new RECT();
                

                while (!GetWindowRect(prc.MainWindowHandle, ref rct))
                {
                }


            
            AppearanceManager.Invoke(new Action(() =>
                {
                    IShiftOSWindow coherenceWindow = new Applications.CoherenceOverlay(prc.MainWindowHandle, rct);

                    AppearanceManager.SetupWindow(coherenceWindow);
                    SetWindowPos(prc.MainWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

                    //MakeExternalWindowBorderless(prc.MainWindowHandle);
                }));
            
        }

        const int WS_BORDER = 8388608;
        const int WS_DLGFRAME = 4194304;
        const int WS_CAPTION = WS_BORDER | WS_DLGFRAME;
        const int WS_SYSMENU = 524288;
        const int WS_THICKFRAME = 262144;
        const int WS_MINIMIZE = 536870912;
        const int WS_MAXIMIZEBOX = 65536;
        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x1;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOSIZE = 0x1;
        const int SWP_FRAMECHANGED = 0x20;
        const uint MF_BYPOSITION = 0x400;
        const uint MF_REMOVE = 0x1000;
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        public static void MakeExternalWindowBorderless(IntPtr MainWindowHandle)
        {
            int Style = 0;
            Style = GetWindowLong(MainWindowHandle, GWL_STYLE);
            Style = Style & ~WS_CAPTION;
            Style = Style & ~WS_SYSMENU;
            Style = Style & ~WS_THICKFRAME;
            Style = Style & ~WS_MINIMIZE;
            Style = Style & ~WS_MAXIMIZEBOX;
            SetWindowLong(MainWindowHandle, GWL_STYLE, Style);
            Style = GetWindowLong(MainWindowHandle, GWL_EXSTYLE);
            SetWindowLong(MainWindowHandle, GWL_EXSTYLE, Style | WS_EX_DLGMODALFRAME);
            SetWindowPos(MainWindowHandle, new IntPtr(0), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);
        }
    }
}
