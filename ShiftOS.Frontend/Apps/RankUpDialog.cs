using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using Plex.Engine;
using Plex.Frontend.GUI;
using Microsoft.Xna.Framework;
using static Plex.Engine.SkinEngine;

namespace Plex.Frontend.Apps
{
    [DefaultTitle("You've ranked up.")]
    public class RankUpDialog : Control, IPlexWindow
    {
        private TextControl _title = new TextControl();
        private TextControl _subtitle = new TextControl();
        private TextControl _description = new TextControl();
        private TextControl _upgrades = new TextControl();
        private TextControl _upgradelist = new TextControl();
        private Rank _rank = null;

        private Button _okay = new Button();

        public RankUpDialog(Rank rank)
        {
            _rank = rank;
            Width = 800;
            Height = 600;

            AddControl(_okay);
            AddControl(_title);
            AddControl(_subtitle);
            AddControl(_description);
            AddControl(_upgrades);
            AddControl(_upgradelist);

            _okay.Click += () => AppearanceManager.Close(this);

            _okay.Text = "OK, cool.";
            _title.Text = "You've ranked up!";

            _subtitle.Text = $"You are now rank {SaveSystem.CurrentSave.Rank}: {_rank.Name}.";

            _description.Text = $"You have ranked up from your previous rank to {_rank.Name}.\r\n\r\nWith your new rank, you can now load up to {_rank.UpgradeMax} upgrades at once!";

            if(_rank.UnlockedUpgrades != null)
            {
                _description.Text += "\r\n\r\nYou have also unlocked these upgrades:";

                _upgrades.Text = "Upgrades";

                _upgradelist.Text = "";
                foreach(var upg in _rank.UnlockedUpgrades)
                {
                    var udata = Upgrades.GetDefaults().FirstOrDefault(x => x.ID == upg);
                    _upgradelist.Text += $" - {udata.Category}: {udata.Name}\r\n";
                }
                _upgradelist.Text += "\r\nSee the Upgrade Manager for more details on these upgrades, and to load them.";
            }


        }

        protected override void OnLayout(GameTime gameTime)
        {
            _title.AutoSize = true;
            _subtitle.AutoSize = true;
            _description.AutoSize = true;
            _upgradelist.AutoSize = true;
            _upgrades.AutoSize = true;
            _okay.AutoSize = true;

            _okay.Font = LoadedSkin.MainFont;
            _title.Font = LoadedSkin.HeaderFont;
            _subtitle.Font = LoadedSkin.Header3Font;
            _upgrades.Font = LoadedSkin.Header3Font;
            _upgradelist.Font = LoadedSkin.MainFont;
            _description.Font = LoadedSkin.MainFont;

            int _width = Width - 30;

            _title.Y = 15;
            _title.MaxWidth = _width;
            _title.X = (Width - _title.Width) / 2;

            _subtitle.Y = _title.Y + _title.Height + 10;
            _subtitle.MaxWidth = _title.MaxWidth;
            _subtitle.X = (Width - _subtitle.Width) / 2;

            _description.X = 15;
            _description.Y = _subtitle.Y + _subtitle.Height + 15;
            _description.MaxWidth = _width;

            _upgrades.Visible = _rank.UnlockedUpgrades != null;
            _upgrades.Y = _description.Y + _description.Height + 10;
            _upgrades.X = 15;

            _upgradelist.Visible = _upgrades.Visible;
            _upgradelist.Y = _upgrades.Y + _upgrades.Height + 10;
            _upgradelist.X = 15;

            _okay.X = (Width - _okay.Width) - 15;
            _okay.Y = (Height - _okay.Height) - 15;
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
    }
}
