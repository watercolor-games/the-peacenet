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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using static ShiftOS.Engine.SkinEngine;
using backend = ShiftOS.Engine.Shiftorium;
namespace ShiftOS.WinForms.Applications
{
    [Launcher("{TITLE_SHIFTORIUM}", true, "al_shiftorium", "{AL_UTILITIES}")]
    [RequiresUpgrade("shiftorium_gui")]
    [MultiplayerOnly]
    [WinOpen("{WO_SHIFTORIUM}")]
    [DefaultTitle("{TITLE_SHIFTORIUM}")]
    [DefaultIcon("iconShiftorium")]
    public partial class ShiftoriumFrontend : UserControl, IShiftOSWindow
    {
        public int CategoryId = 0;
        private string[] cats;
        private ShiftoriumUpgrade[] avail;


        public void updatecounter()
        {
            Desktop.InvokeOnWorkerThread(() => { lbcodepoints.Text = $"You have {SaveSystem.CurrentSave.Codepoints} Codepoints."; });
        }

        public ShiftoriumFrontend()
        {
            InitializeComponent();
            updatecounter();
            Populate();
            SetList();
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
            var upg = upgrades[CategoryId][name];
            lbupgradetitle.Text = Localization.Parse(upg.Name);
            lbupgradedesc.Text = Localization.Parse(upg.Description);
        }

        Dictionary<string, ShiftoriumUpgrade>[] upgrades;
        
        private void Populate()
        {
            cats = Shiftorium.GetCategories();
            upgrades = new Dictionary<string, ShiftoriumUpgrade>[cats.Length];
            int numComplete = 0;
            avail = backend.GetAvailable();
            foreach (var it in cats.Select((catName, catId) => new { catName, catId }))
            {
                var upl = new Dictionary<string, ShiftoriumUpgrade>();
                upgrades[it.catId] = upl;
                var t = new Thread((tupobj) =>
                {
                    foreach (var upg in avail.Where(x => x.Category == it.catName))
                        upl.Add(Localization.Parse(upg.Name) + " - " + upg.Cost.ToString() + "CP", upg);
                    if (it.catId == CategoryId)
                        this.Invoke(new Action(() =>
                        {
                            SetList();
                        }));
                    numComplete++;
                });
                t.Start();
            }
        }

        private void SetList()
        {
            lbupgrades.Items.Clear();
            if (upgrades.Length == 0)
                return;
            lbnoupgrades.Hide();
            if (CategoryId > upgrades.Length)
                CategoryId = 0;
            try
            {
                lbupgrades.Items.AddRange(upgrades[CategoryId].Keys.ToArray());
            }
            catch
            {
                Engine.Infobox.Show("Shiftorium Machine Broke", "Category ID " + CategoryId.ToString() + " is invalid, modulo is broken, and the world is doomed. Please tell Declan about this.");
                return;
            }
            if (lbupgrades.Items.Count == 0)
            {
                lbnoupgrades.Show();
                lbnoupgrades.Location = new Point(
                    (lbupgrades.Width - lbnoupgrades.Width) / 2,
                    lbupgrades.Top + (lbupgrades.Height - lbnoupgrades.Height) / 2
                    );
            }
            else
                lbnoupgrades.Hide();
            lblcategorytext.Text = cats[CategoryId];
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
            ulong cpCost = 0;
            backend.Silent = true;
            Dictionary<string, ulong> UpgradesToBuy = new Dictionary<string, ulong>(); 
            foreach (var itm in lbupgrades.SelectedItems)
            {
                var upg = upgrades[CategoryId][itm.ToString()];
                cpCost += upg.Cost;
                UpgradesToBuy.Add(upg.ID, upg.Cost);
            }
            if (SaveSystem.CurrentSave.Codepoints < cpCost)
            {
                Infobox.Show("Insufficient Codepoints", $"You do not have enough Codepoints to perform this action. You need {cpCost - SaveSystem.CurrentSave.Codepoints} more.");
                
            }
            else
            {
                foreach(var upg in UpgradesToBuy)
                {
                    if (SaveSystem.CurrentSave.Upgrades.ContainsKey(upg.Key))
                    {
                        SaveSystem.CurrentSave.Upgrades[upg.Key] = true;
                    }
                    else
                    {
                        SaveSystem.CurrentSave.Upgrades.Add(upg.Key, true);
                    }
                    SaveSystem.SaveGame();
                    backend.InvokeUpgradeInstalled();
                }
                SaveSystem.CurrentSave.Codepoints -= cpCost;
            }

                backend.Silent = false;
            btnbuy.Hide();
        }

        public void OnLoad()
        {
            lbnoupgrades.Hide();
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
            lbupgrades.SelectionMode = (UpgradeInstalled("shiftorium_gui_bulk_buy") == true) ? SelectionMode.MultiExtended : SelectionMode.One;
            lbcodepoints.Visible = Shiftorium.UpgradeInstalled("shiftorium_gui_codepoints_display");
            Populate();
            
        }

        private void moveCat(short direction) // direction is -1 to move backwards or 1 to move forwards
        {
            if (cats.Length == 0) return;
            CategoryId += direction;
            CategoryId %= cats.Length;
            if (CategoryId < 0) CategoryId += cats.Length; // fix modulo on negatives
            SetList();
        }

        private void btncat_back_Click(object sender, EventArgs e)
        {
            moveCat(-1);
        }

        private void btncat_forward_Click(object sender, EventArgs e)
        {
            moveCat(1);
        }
    }
}
