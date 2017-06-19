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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Objects.ShiftFS;
using static ShiftOS.Engine.SkinEngine;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Denotes that this class is launchable from the App Launcher.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class LauncherAttribute : Attribute
    {
        /// <summary>
        /// Marks this form as a launcher item that, when clicked, will open the form.
        /// </summary>
        /// <param name="name">The text displayed on the launcher item</param>
        /// <param name="requiresUpgrade">Whether or not an upgrade must be installed to see the launcher</param>
        /// <param name="upgradeID">The ID of the upgrade - leave blank if requiresUpgrade is false.</param>
        /// <param name="category">The category that the item will appear in.</param>
        public LauncherAttribute(string name, bool requiresUpgrade, string upgradeID = "", string category = "Other")
        {
            Category = category;
            Name = name;
            RequiresUpgrade = requiresUpgrade;
            ID = upgradeID;
        }

        /// <summary>
        /// Gets or sets the name of the launcher item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether this entry requires a Shiftorium upgrade.
        /// </summary>
        public bool RequiresUpgrade { get; set; }

        /// <summary>
        /// Gets or sets the ID of the required upgrade.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets this item's category.
        /// </summary>
        public string Category { get; internal set; }

        /// <summary>
        /// Gets whether or not the required upgrade is installed.
        /// </summary>
        public bool UpgradeInstalled
        {
            get
            {
                if (!RequiresUpgrade)
                    return true;

                return Shiftorium.UpgradeInstalled(ID);
            }
        }
    }

    /// <summary>
    /// Provides core functionality for a typical ShiftOS desktop.
    /// </summary>
    public interface IDesktop
    {
        /// <summary>
        /// Gets the name of the desktop.
        /// </summary>
        string DesktopName { get; }

        /// <summary>
        /// Show a notification on the desktop.
        /// </summary>
        /// <param name="app">An application ID (for determining what system icon to show the notification alongside)</param>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">Isn't this.... self explanatory?</param>
        void PushNotification(string app, string title, string message);

        /// <summary>
        /// Performs most of the skinning and layout handling for the desktop.
        /// </summary>
        void SetupDesktop();

        /// <summary>
        /// Hides the currently-opened app launcher menu.
        /// </summary>
        void HideAppLauncher();


        /// <summary>
        /// Populates the app launcher menu.
        /// </summary>
        /// <param name="items">All items to be placed in the menu.</param>
        void PopulateAppLauncher(LauncherItem[] items);

        /// <summary>
        /// Handles desktop-specific routines for showing ShiftOS windows.
        /// </summary>
        /// <param name="border">The calling window.</param>
        void ShowWindow(IWindowBorder border);

        /// <summary>
        /// Handles desktop-specific routines for closing ShiftOS windows.
        /// </summary>
        /// <param name="border">The calling window.</param>
        void KillWindow(IWindowBorder border);

        /// <summary>
        /// Populates the panel button list with all open windows.
        /// </summary>
        void PopulatePanelButtons();

        /// <summary>
        /// Performs desktop-specific routines for minimizing a window.
        /// </summary>
        /// <param name="brdr">The calling window.</param>
        void MinimizeWindow(IWindowBorder brdr);


        /// <summary>
        /// Performs desktop-specific routines for maximizing a window.
        /// </summary>
        /// <param name="brdr">The calling window.</param>
        void MaximizeWindow(IWindowBorder brdr);


        /// <summary>
        /// Performs desktop-specific routines for restoring a window to its default state.
        /// </summary>
        /// <param name="brdr">The calling window.</param>
        void RestoreWindow(IWindowBorder brdr);

        /// <summary>
        /// Invokes an action on the UI thread.
        /// </summary>
        /// <param name="act">The action to invoke.</param>
        void InvokeOnWorkerThread(Action act);

        /// <summary>
        /// Calculates the screen size of the desktop.
        /// </summary>
        /// <returns>The desktop's screen size.</returns>
        Size GetSize();

        /// <summary>
        /// Opens the app launcher at a specific point.
        /// </summary>
        /// <param name="loc">Where the app launcher should be opened.</param>
        void OpenAppLauncher(Point loc);

        /// <summary>
        /// Opens the desktop.
        /// </summary>
        void Show();

        /// <summary>
        /// Closes the desktop.
        /// </summary>
        void Close();
    }

    public static class Desktop
    {
        /// <summary>
        /// The underlying desktop object.
        /// </summary>
        private static IDesktop _desktop = null;

        public static Size Size
        {
            get
            {
                return _desktop.GetSize();
            }
        }

        public static IDesktop CurrentDesktop
        {
            get
            {
                return _desktop;
            }
        }

        public static void Init(IDesktop desk, bool show = false)
        {
            IDesktop deskToClose = null;
            if (_desktop != null)
                deskToClose = _desktop;
            _desktop = desk;
            if (show == true)
                _desktop.Show();
            deskToClose?.Close();
        }

        public static void MinimizeWindow(IWindowBorder brdr)
        {
            _desktop.MinimizeWindow(brdr);
        }

        public static void MaximizeWindow(IWindowBorder brdr)
        {
            _desktop.MaximizeWindow(brdr);
        }

        public static void RestoreWindow(IWindowBorder brdr)
        {
            _desktop.RestoreWindow(brdr);
        }


        public static void InvokeOnWorkerThread(Action act)
        {
            _desktop.InvokeOnWorkerThread(act);
        }

        public static void ResetPanelButtons()
        {
            _desktop.PopulatePanelButtons();
        }

        public static void ShowWindow(IWindowBorder brdr)
        {
            _desktop.ShowWindow(brdr);
        }

        public static void PopulateAppLauncher()
        {
            _desktop.PopulateAppLauncher(AppLauncherDaemon.Available().ToArray());
        }

        public static void OpenAppLauncher(Point loc)
        {
            _desktop.OpenAppLauncher(loc);
        }

        public static void HideAppLauncher()
        {
            _desktop.HideAppLauncher();
        }

        public static void PushNotification(string app, string title, string msg)
        {
            InvokeOnWorkerThread(() =>
            {
                _desktop.PushNotification(app, title, msg);
            });
        }
    }
    // sorry i almost killed everything :P
}
