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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Applications {

    [Launcher("Name Changer", true, "al_name_changer", "Customization")]
    [RequiresUpgrade("name_changer")]
    [WinOpen("name_changer")]
    public partial class NameChanger : UserControl, IShiftOSWindow
    {
        public NameChanger()
        {
            InitializeComponent();
        }

        public void OnLoad()
        {
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

        private void NameChanger_Load(object sender, EventArgs e)
        {

        }
        
    }

    public static class NameChangerBackend
    {
        public static Dictionary<string, string> GetDefault()
        {
            var dict = new Dictionary<string, string>();
            foreach(var winType in AppearanceManager.GetAllWindowTypes())
            {
                if (dict.ContainsKey(winType.Name))
                    dict[winType.Name] = AppearanceManager.GetDefaultTitle(winType);
                else
                    dict.Add(winType.Name, AppearanceManager.GetDefaultTitle(winType));
            }
            return dict;
        }

        public static Dictionary<string,string> GetCurrent()
        {
            if (SkinEngine.LoadedSkin == null)
                return GetDefault();

            if (SkinEngine.LoadedSkin.AppNames == null)
                SkinEngine.LoadedSkin.AppNames = GetDefault();
            return SkinEngine.LoadedSkin.AppNames;
        }

        public static string GetName(IShiftOSWindow win)
        {
            if (SkinEngine.LoadedSkin == null)
                return AppearanceManager.GetDefaultTitle(win.GetType());

            if (SkinEngine.LoadedSkin.AppNames == null)
                SkinEngine.LoadedSkin.AppNames = GetDefault();

            if (!SkinEngine.LoadedSkin.AppNames.ContainsKey(win.GetType().Name))
                SkinEngine.LoadedSkin.AppNames.Add(win.GetType().Name, AppearanceManager.GetDefaultTitle(win.GetType()));

            return SkinEngine.LoadedSkin.AppNames[win.GetType().Name];
        }
    }
}
