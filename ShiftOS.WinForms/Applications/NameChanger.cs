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
    [FileHandler("Name Pack", ".nme", "fileiconnames")]
    [MultiplayerOnly]
    [Launcher("{TITLE_NAMECHANGER}", false, null, "{AL_CUSTOMIZATION}")]
    [AppscapeEntry("name_changer", "{TITLE_NAMECHANGER}", "{DESC_NAMECHANGER}", 342, 500, "skinning;file_skimmer;wm_titlebar", "{AL_CUSTOMIZATION}")]
    [WinOpen("{WO_NAMECHANGER}")]
    [DefaultTitle("{TITLE_NAMECHANGER}")]
    [DefaultIcon("iconNameChanger")]
    public partial class NameChanger : UserControl, IShiftOSWindow, IFileHandler
    {
        public NameChanger()
        {
            InitializeComponent();
        }

        public void OpenFile(string file)
        {
            AppearanceManager.SetupWindow(this);
            names = JsonConvert.DeserializeObject<Dictionary<string, string>>(ShiftOS.Objects.ShiftFS.Utils.ReadAllText(file));
            SetupUI();
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
            foreach(var type in ReflectMan.Types.Where(x => Shiftorium.UpgradeAttributesUnlocked(x) && x.GetInterfaces().Contains(typeof(IShiftOSWindow))))
            {
                var title = type.GetCustomAttributes(false).FirstOrDefault(x => x is DefaultTitleAttribute) as DefaultTitleAttribute;
                if(title != null)
                {
                    var lbl = new Label();
                    lbl.AutoSize = true;
                    lbl.Text = Localization.Parse(title.Title);
                    lbl.Tag = "header3";
                    ControlManager.SetupControl(lbl);
                    flnames.Controls.Add(lbl);
                    lbl.Show();
                    flnames.SetFlowBreak(lbl, true);

                    var txt = new TextBox();
                    ControlManager.SetupControl(txt);
                    if (!names.ContainsKey(type.Name))
                        names.Add(type.Name, title.Title);
                    txt.Text = Localization.Parse(names[type.Name]);
                    txt.TextChanged += (o, a) =>
                    {
                        if(txt.Text == Localization.Parse(title.Title))
                        {
                            names[type.Name] = title.Title;
                        }
                        else
                        {
                            names[type.Name] = txt.Text;
                        }
                    };
                    flnames.Controls.Add(txt);
                    txt.Show();
                    txt.Width = flnames.Width - 10;
                    flnames.SetFlowBreak(txt, true);
                }
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
            names = new Dictionary<string, string>();
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
            return new Dictionary<string, string>();
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
            return GetNameRaw(win.GetType());
        }

        internal static string GetNameRaw(Type type)
        {
            if (SkinEngine.LoadedSkin == null)
                return AppearanceManager.GetDefaultTitle(type);

            if (SkinEngine.LoadedSkin.AppNames == null)
                SkinEngine.LoadedSkin.AppNames = GetDefault();

            if (!SkinEngine.LoadedSkin.AppNames.ContainsKey(type.Name))
                return Localization.Parse(AppearanceManager.GetDefaultTitle(type));

            return SkinEngine.LoadedSkin.AppNames[type.Name];
        }
    }
}
