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
using static ShiftOS.Engine.SkinEngine;

namespace ShiftOS.Engine
{
    public class LauncherAttribute : Attribute
    {
        /// <summary>
        /// Marks this form as a launcher item that, when clicked, will open the form.
        /// </summary>
        /// <param name="name">The text displayed on the launcher item</param>
        /// <param name="requiresUpgrade">Whether or not an upgrade must be installed to see the launcher</param>
        /// <param name="upgradeID">The ID of the upgrade - leave blank if requiresUpgrade is false.</param>
        public LauncherAttribute(string name, bool requiresUpgrade, string upgradeID = "")
        {
            Name = name;
            RequiresUpgrade = requiresUpgrade;
            ID = upgradeID;
        }

        public string Name { get; set; }
        public bool RequiresUpgrade { get; set; }
        public string ID { get; set; }
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


    public interface IDesktop
    {
        void SetupDesktop();
        void PopulateAppLauncher(LauncherItem[] items);
        void ShowWindow(IWindowBorder border);
        void KillWindow(IWindowBorder border);
        void PopulatePanelButtons();
        void MinimizeWindow(IWindowBorder brdr);
        void MaximizeWindow(IWindowBorder brdr);
        void RestoreWindow(IWindowBorder brdr);
        void InvokeOnWorkerThread(Action act);
        Size GetSize();
    }

    public static class Desktop
    {
        private static IDesktop _desktop = null;

        public static Size Size { get
            {
                return _desktop.GetSize();
            }
        }

        public static void Init(IDesktop desk)
        {
            _desktop = desk;
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
    }

}
