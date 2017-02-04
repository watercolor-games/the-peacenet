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
using ShiftOS.Engine;
using static ShiftOS.Engine.SkinEngine;
using backend = ShiftOS.Engine.Shiftorium;
namespace ShiftOS.WinForms.Applications
{
    [Launcher("Shiftorium", true, "al_shiftorium", "Utilities")]
    [RequiresUpgrade("shiftorium_gui")]
    [WinOpen("shiftorium")]
    [DefaultTitle("Shiftorium")]
    public partial class ShiftoriumFrontend : UserControl, IShiftOSWindow
    {

        public static System.Timers.Timer timer100;


        public ShiftoriumFrontend()
        {

            InitializeComponent();
            PopulateShiftorium();
            lbupgrades.SelectedIndexChanged += (o, a) =>
            {
                try
                {
                    lbupgrades.Refresh();
                    SelectUpgrade(lbupgrades.SelectedItem.ToString());
                }
                catch { }
            };
            this.pgupgradeprogress.Maximum = backend.GetDefaults().Count;
            this.pgupgradeprogress.Value = SaveSystem.CurrentSave.CountUpgrades();
            backend.Installed += () =>
            {
                this.pgupgradeprogress.Maximum = backend.GetDefaults().Count;
                this.pgupgradeprogress.Value = SaveSystem.CurrentSave.CountUpgrades();
            };

        }

        public void SelectUpgrade(string name)
        {
            btnbuy.Show();
            var upg = upgrades[name];
            lbupgradetitle.Text = Localization.Parse(upg.Name);
            lbupgradedesc.Text = Localization.Parse(upg.Description);
        }

        Dictionary<string, ShiftoriumUpgrade> upgrades = new Dictionary<string, ShiftoriumUpgrade>();

        public void PopulateShiftorium()
        {
            lbupgrades.Items.Clear();
            upgrades.Clear();
            Timer();
            label2.Text = "You have: " + SaveSystem.CurrentSave.Codepoints.ToString() + " Codepoints";

            foreach (var upg in backend.GetAvailable())
            {
                String name = Localization.Parse(upg.Name) + " - " + upg.Cost.ToString() + "CP";
                upgrades.Add(name, upg);
                lbupgrades.Items.Add(name);
            }
        }

        public static bool UpgradeInstalled(string upg)
        {
            return backend.UpgradeInstalled(upg);
        }

        public static bool UpgradeAttributesUnlocked(FieldInfo finf)
        {
            return backend.UpgradeAttributesUnlocked(finf);
        }

        public static bool UpgradeAttributesUnlocked(MethodInfo finf)
        {
            return backend.UpgradeAttributesUnlocked(finf);
        }

        public static bool UpgradeAttributesUnlocked(Type finf)
        {
            return backend.UpgradeAttributesUnlocked(finf);
        }

        public static bool UpgradeAttributesUnlocked(PropertyInfo finf)
        {
            return backend.UpgradeAttributesUnlocked(finf);
        }

        private void lbupgrades_DrawItem(object sender, DrawItemEventArgs e)
        {
            var foreground = new SolidBrush(LoadedSkin.ControlTextColor);
            var background = new SolidBrush(LoadedSkin.ControlColor);

            e.Graphics.FillRectangle(background, e.Bounds);
            try
            {
                if (lbupgrades.GetSelected(e.Index) == true)
                {
                    e.Graphics.FillRectangle(foreground, e.Bounds);
                    e.Graphics.DrawString(lbupgrades.Items[e.Index].ToString(), e.Font, background, e.Bounds.Location);
                }
                else
                {
                    e.Graphics.FillRectangle(background, e.Bounds);
                    e.Graphics.DrawString(lbupgrades.Items[e.Index].ToString(), e.Font, foreground, e.Bounds.Location);
                }
            }
            catch
            {
            }
        }

        private void btnbuy_Click(object sender, EventArgs e)
        {
            backend.Silent = true;
            backend.Buy(upgrades[lbupgrades.SelectedItem.ToString()].ID, upgrades[lbupgrades.SelectedItem.ToString()].Cost);
            backend.Silent = false;
            PopulateShiftorium();
            btnbuy.Hide();
        }

        private void Shiftorium_Load(object sender, EventArgs e) {

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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        void Timer()
        {
            timer100 = new System.Timers.Timer();
            timer100.Interval = 2000;
            //timer100.Elapsed += ???;
            timer100.AutoReset = true;
            timer100.Enabled = true;
        }
    }
}
