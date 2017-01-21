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
using System.Threading.Tasks;
using ShiftOS.Engine;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;

/// <summary>
/// Coherence commands.
/// </summary>
namespace ShiftOS.WinForms
{
    [Namespace("trm")]
    public static class TerminalExtensions
    {
        [Command("setpass", true)]
        [RequiresArgument("pass")]
        public static bool setPass(Dictionary<string, object> args)
        {
            SaveSystem.CurrentSave.Password = args["pass"] as string;
            return true;
        }

        [Command("remote", "username:,sysname:,pass:", "Allows you to control a remote system on the multi-user domain given a username, password and system name.")]
        [RequiresArgument("username")]
        [RequiresArgument("sysname")]
        [RequiresArgument("pass")]
        public static bool RemoteControl(Dictionary<string, object> args)
        {
            ServerManager.SendMessage("trm_handshake_request", JsonConvert.SerializeObject(args));
            return true;
        }
    }


    [Namespace("coherence")]
    [RequiresUpgrade("kernel_coherence")]
    public static class CoherenceCommands
    {
		/// <summary>
		/// Sets the window position.
		/// </summary>
		/// <returns>The window position.</returns>
		/// <param name="hWnd">H window.</param>
		/// <param name="hWndInsertAfter">H window insert after.</param>
		/// <param name="X">X.</param>
		/// <param name="Y">Y.</param>
		/// <param name="cx">Cx.</param>
		/// <param name="cy">Cy.</param>
		/// <param name="uFlags">U flags.</param>
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		/// <summary>
		/// The HWN d TOPMOS.
		/// </summary>
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

		/// <summary>
		/// The SW p SHOWWINDO.
		/// </summary>
        const UInt32 SWP_SHOWWINDOW = 0x0040;

        
        [DllImport("user32.dll")]
		/// <summary>
		/// Gets the window rect.
		/// </summary>
		/// <returns>The window rect.</returns>
		/// <param name="hWnd">H window.</param>
		/// <param name="lpRect">Lp rect.</param>
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

		/// <summary>
		/// REC.
		/// </summary>
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
		/// <summary>
		/// Launchs the app.
		/// </summary>
		/// <returns>The app.</returns>
		/// <param name="args">Arguments.</param>
        public static bool LaunchApp(Dictionary<string, object> args)
        {
            string process = args["process"].ToString();
            var prc = Process.Start(process);
            StartCoherence(prc);
            return true;
        }

		/// <summary>
		/// Starts the coherence.
		/// </summary>
		/// <returns>The coherence.</returns>
		/// <param name="prc">Prc.</param>
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

		/// <summary>
		/// The W s BORDE.
		/// </summary>
        const int WS_BORDER = 8388608;

		/// <summary>
		/// The W s DLGFRAM.
		/// </summary>
        const int WS_DLGFRAME = 4194304;

		/// <summary>
		/// The W s CAPTIO.
		/// </summary>
        const int WS_CAPTION = WS_BORDER | WS_DLGFRAME;

		/// <summary>
		/// The W s SYSMEN.
		/// </summary>
        const int WS_SYSMENU = 524288;

		/// <summary>
		/// The W s THICKFRAM.
		/// </summary>
        const int WS_THICKFRAME = 262144;

		/// <summary>
		/// The W s MINIMIZ.
		/// </summary>
        const int WS_MINIMIZE = 536870912;

		/// <summary>
		/// The W s MAXIMIZEBO.
		/// </summary>
        const int WS_MAXIMIZEBOX = 65536;

		/// <summary>
		/// The GW l STYL.
		/// </summary>
        const int GWL_STYLE = -16;

		/// <summary>
		/// The GW l EXSTYL.
		/// </summary>
        const int GWL_EXSTYLE = -20;

		/// <summary>
		/// The W s E x DLGMODALFRAM.
		/// </summary>
        const int WS_EX_DLGMODALFRAME = 0x1;

		/// <summary>
		/// The SW p NOMOV.
		/// </summary>
        const int SWP_NOMOVE = 0x2;

		/// <summary>
		/// The SW p NOSIZ.
		/// </summary>
        const int SWP_NOSIZE = 0x1;

		/// <summary>
		/// The SW p FRAMECHANGE.
		/// </summary>
        const int SWP_FRAMECHANGED = 0x20;

		/// <summary>
		/// The M f BYPOSITIO.
		/// </summary>
        const uint MF_BYPOSITION = 0x400;

		/// <summary>
		/// The M f REMOV.
		/// </summary>
        const uint MF_REMOVE = 0x1000;

		/// <summary>
		/// Gets the window long.
		/// </summary>
		/// <returns>The window long.</returns>
		/// <param name="hWnd">H window.</param>
		/// <param name="nIndex">N index.</param>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        
		/// <summary>
		/// Sets the window long.
		/// </summary>
		/// <returns>The window long.</returns>
		/// <param name="hWnd">H window.</param>
		/// <param name="nIndex">N index.</param>
		/// <param name="dwNewLong">Dw new long.</param>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        
		/// <summary>
		/// Sets the window position.
		/// </summary>
		/// <returns>The window position.</returns>
		/// <param name="hWnd">H window.</param>
		/// <param name="hWndInsertAfter">H window insert after.</param>
		/// <param name="X">X.</param>
		/// <param name="Y">Y.</param>
		/// <param name="cx">Cx.</param>
		/// <param name="cy">Cy.</param>
		/// <param name="uFlags">U flags.</param>
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
