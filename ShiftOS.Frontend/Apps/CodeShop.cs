using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Frontend.Apps
{
    [Launcher("Upgrades", false, null, "Utilities")]
    [DefaultTitle("Upgrades")]
    [WinOpen("upgrademgr")]
    public class CodeShop : GUI.Control, IPlexWindow
    {
        private GUI.TextControl _mainTitle = new GUI.TextControl();
        private GUI.ListBox upgradelist = null;
        private ShiftoriumUpgrade selectedUpgrade = null;
        private GUI.ProgressBar upgradeprogress = null;
        private GUI.Button buy = null;
        private GUI.TextControl _upgradeTitle = new GUI.TextControl();
        private GUI.TextControl _upgradeDescription = new GUI.TextControl();

        public CodeShop()
        {
            Width = 720;
            Height = 480;
            AddControl(_upgradeTitle);
            AddControl(_upgradeDescription);
            AddControl(_mainTitle);
            _mainTitle.Text = "Upgrades";
            _mainTitle.AutoSize = true;
        }

        protected override void OnLayout(GameTime gameTime)
        {
            try
            {
                upgradelist.X = 30;
                upgradelist.Y = 75;
                upgradelist.Width = this.Width / 2;
                upgradelist.Width -= 30;
                upgradelist.Height = this.Height - upgradelist.Y - 75;
                upgradeprogress.X = upgradelist.X;
                upgradeprogress.Y = upgradelist.Y + upgradelist.Height + 10;
                upgradeprogress.Width = upgradelist.Width;
                upgradeprogress.Height = 24;
                upgradeprogress.Maximum = Upgrades.GetAllPurchasable().Length;
                upgradeprogress.Value = Upgrades.CountUpgrades();
                buy.X = Width - buy.Width - 15;
                buy.Y = Height - buy.Height - 15;
                buy.Visible = (selectedUpgrade != null);
                string title = "Welcome to the Shiftorium!";
                string desc = @"The Shiftorium is a place where you can buy upgrades for your computer. These upgrades include hardware enhancements, kernel and software optimizations and features, new programs, upgrades to existing programs, and more.

As you continue through your job, going further up the ranks, you will unlock additional upgrades which can be found here. You may also find upgrades which are not available within the Shiftorium when hacking more difficult and experienced targets. These upgrades are very rare and hard to find, though. You'll find them in the ""Installed Upgrades"" list.";

                if (selectedUpgrade != null)
                {
                    title = selectedUpgrade.Category + ": " + selectedUpgrade.Name;
                    if (selectedUpgrade.Installed)
                    {
                        desc = (string.IsNullOrEmpty(selectedUpgrade.Tutorial)) ? "No tutorial has been provided for this upgrade." : selectedUpgrade.Tutorial;
                    }
                    else
                    {
                        desc = selectedUpgrade.Description;
                    }
                }
                _upgradeTitle.Font = SkinEngine.LoadedSkin.Header2Font;
                _upgradeDescription.Font = SkinEngine.LoadedSkin.MainFont;
                _upgradeTitle.Text = title;
                _upgradeDescription.Text = desc;
                _upgradeTitle.AutoSize = true;
                int wrapwidth = (Width - (upgradelist.X + upgradelist.Width)) - 45;
                _upgradeTitle.MaxWidth = wrapwidth;
                _upgradeTitle.Y = 15;
                _upgradeTitle.X = upgradelist.X + upgradelist.Width + 15;

                _upgradeDescription.X = _upgradeTitle.X;
                _upgradeDescription.Width = wrapwidth;
                _upgradeDescription.Y = _upgradeTitle.Y + _upgradeTitle.Height + 10;
                _upgradeDescription.Height = (Height - _upgradeDescription.Y - 50);

                _mainTitle.Y = upgradelist.Y - _mainTitle.Height - 5;
                _mainTitle.Font = SkinEngine.LoadedSkin.Header2Font;
                _mainTitle.MaxWidth = upgradelist.Width;
                _mainTitle.X = upgradelist.X + ((upgradelist.Width - _mainTitle.Width) / 2);
            }
            catch
            {

            }
        }

        public void OnLoad()
        {
            buy = new GUI.Button();
            buy.Text = "Buy upgrade";
            buy.AutoSize = true;
            buy.Font = SkinEngine.LoadedSkin.MainFont;
            buy.Click += () =>
            {
                if (selectedUpgrade.Installed)
                {
                    try
                    {
                        if (Upgrades.IsLoaded(selectedUpgrade.ID))
                        {
                            Upgrades.UnloadUpgrade(selectedUpgrade.ID);
                        }
                        else
                        {
                            Upgrades.LoadUpgrade(selectedUpgrade.ID);

                        }
                        SelectUpgrade(selectedUpgrade);
                    }
                    catch (UpgradeException ex)
                    {
                        Engine.Infobox.Show("Upgrade error!", ex.ErrorMessage);
                    }
                }
                else
                {
                    if (Upgrades.Buy(selectedUpgrade.ID, selectedUpgrade.Cost) == true)
                    {
                        Engine.Infobox.Show("Upgrade installed!", "You have successfully bought and installed the " + selectedUpgrade.Name + " upgrade for " + selectedUpgrade.Cost + " Experience.");
                        SelectUpgrade(null);
                        PopulateList();
                    }
                    else
                    {
                        Engine.Infobox.Show("Insufficient funds.", "You do not have enough Experience to buy this upgrade. You need " + (selectedUpgrade.Cost - SaveSystem.CurrentSave.Experience) + " more.");
                    }
                }
            };
            AddControl(buy);
            upgradelist = new GUI.ListBox();
            upgradeprogress = new GUI.ProgressBar();
            AddControl(upgradeprogress);
            AddControl(upgradelist);
            upgradelist.SelectedIndexChanged += () =>
            {
                if (upgradelist.SelectedItem != null)
                {
                    var upg = Upgrades.GetDefaults()[upgradelist.SelectedIndex];
                    if (upg != null)
                        SelectUpgrade(upg);
                }
            };
            PopulateList();
        }

        public void SelectUpgrade(ShiftoriumUpgrade upgrade)
        {
            if(selectedUpgrade != upgrade)
            {
                selectedUpgrade = upgrade;
                if (upgrade.Installed)
                {
                    string type = (Upgrades.IsLoaded(upgrade.ID)) ? "Unload" : "Load";
                    buy.Text = $"{type} upgrade";
                }
                else
                {
                    buy.Text = "Buy upgrade";
                }
                Invalidate();
            }
        }

        public void PopulateList()
        {
            upgradelist.ClearItems();
            foreach(var upgrade in Upgrades.GetDefaults())
            {
                string type = "unknown";
                if (upgrade.Purchasable)
                    type = $"{upgrade.Cost} XP";
                if (upgrade.Installed)
                {
                    type = (Upgrades.IsLoaded(upgrade.ID)) ? "loaded" : "unloaded";
                }
                upgradelist.AddItem($"{upgrade.Category}: {upgrade.Name} ({type})");
                Invalidate();
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
            PopulateList();
        }

    }
}
