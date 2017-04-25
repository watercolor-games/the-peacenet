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
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects.ShiftFS;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications {

    [MultiplayerOnly]
    [Launcher("Name Changer", false, null, "Customization")]
    [AppscapeEntry("Name Changer", "Want to change the names of applications within ShiftOS? This application lets you do just that.", 342, 500, "skinning;file_skimmer;wm_titlebar", "Customization")]
    [WinOpen("name_changer")]
    [DefaultTitle("Name Changer")]
    [DefaultIcon("iconNameChanger")]
    public partial class NameChanger : UserControl, IShiftOSWindow
    {
        public NameChanger()
        {
            InitializeComponent();
        }

        private Dictionary<string, string> names = new Dictionary<string, string>();

        public void OnLoad()
        {
            names = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(NameChangerBackend.GetCurrent()));
            SetupUI();
        }

        public void SetupUI()
        {
            flnames.Controls.Clear();
            foreach(var name in names)
            {
                var pnl = new Panel();
                var lbl = new Label();
                var txt = new TextBox();
                pnl.Controls.Add(lbl);
                lbl.Show();
                pnl.Controls.Add(txt);
                txt.Show();

                ControlManager.SetupControls(pnl);

                pnl.Width = flnames.Width - 10;
                pnl.Height = 50;
                lbl.Left = 10;
                lbl.Width = (pnl.Width / 4) - 10;
                lbl.Text = name.Key;
                lbl.Top = (pnl.Height - lbl.Height) / 2;
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                
                txt.Text = name.Value;
                
                txt.TextChanged += (o, a) =>
                {
                    names[name.Key] = txt.Text;
                };
                txt.Width = pnl.Width - (pnl.Width / 4) - 20;
                txt.Left = lbl.Width + 20;
                txt.Top = (pnl.Height - txt.Height) / 2;
                flnames.Controls.Add(pnl);
                pnl.Show();
            }
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

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnloaddefault_Click(object sender, EventArgs e)
        {
            names = NameChangerBackend.GetDefault();
            SetupUI();
        }

        private void btnimport_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { ".nme" }, FileOpenerStyle.Open, new Action<string>((path) =>
             {
                 names = JsonConvert.DeserializeObject<Dictionary<string, string>>(Utils.ReadAllText(path));
             }));
        }

        private void btnexport_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { ".nme" }, FileOpenerStyle.Save, new Action<string>((path) =>
            {
                Utils.WriteAllText(path, JsonConvert.SerializeObject(names));
            }));
        }

        private void btnapply_Click(object sender, EventArgs e)
        {
            SkinEngine.LoadedSkin.AppNames = names;
            Utils.WriteAllText(Paths.GetPath("skin.json"), SkinEngine.LoadedSkin.ToString());
            SkinEngine.LoadSkin();
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

            foreach(var def in GetDefault())
            {
                if (!SkinEngine.LoadedSkin.AppNames.ContainsKey(def.Key))
                    SkinEngine.LoadedSkin.AppNames.Add(def.Key, def.Value);
            }

            return SkinEngine.LoadedSkin.AppNames;
        }

        public static string GetName(IShiftOSWindow win)
        {
            return GetNameRaw(win.GetType());
        }

        internal static string GetNameRaw(Type type)
        {
            if (SkinEngine.LoadedSkin == null)
                return AppearanceManager.GetDefaultTitle(type);

            if (SkinEngine.LoadedSkin.AppNames == null)
                SkinEngine.LoadedSkin.AppNames = GetDefault();

            if (!SkinEngine.LoadedSkin.AppNames.ContainsKey(type.Name))
                SkinEngine.LoadedSkin.AppNames.Add(type.Name, AppearanceManager.GetDefaultTitle(type));

            return SkinEngine.LoadedSkin.AppNames[type.Name];
        }
    }
}
