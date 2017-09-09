using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Frontend.GUI;
using Plex.Extras;
using Plex.Frontend.GraphicsSubsystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GUI;

namespace Plex.Frontend.PlexnetSites
{
    [Plexnet("main.moneymate", "home.rnp")]
    public class MoneyMate : PlexnetSite
    {
        private PictureBox _moneyMate = null;
        private TextControl _title = new TextControl();
        private TextControl _description = new TextControl();
        private Button _download = new Button();

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.DrawRectangle(0, 0, Width, 200, Color.Green);
        }

        public MoneyMate()
        {
            _moneyMate = new PictureBox();
            _moneyMate.ImageLayout = System.Windows.Forms.ImageLayout.Center;

            AddControl(_moneyMate);
            AddControl(_title);
            AddControl(_description);
            _title.AutoSize = true;
            _title.Text = "The MoneyMate Manager";
            _description.AutoSize = true;
            _description.Text = $@"Download our FREE MoneyMate Manager tool for Plexgate TODAY!

The MoneyMate Manager allows you to manage your account balance, transactions, and much more, all from a simple, pure, and lightweight user-interface. Install today, lifetime satisfaction guaranteed!";
            _description.Alignment = TextAlignment.Middle;

            _download.Text = "Download MoneyMate!";
            _download.AutoSize = true;
            AddControl(_download);
            _download.Click += () =>
            {
                var stp = Apps.Installer.Generate<Apps.MoneyMateManager>();
                FileSkimmerBackend.GetFile(new[] { ".pst" }, FileOpenerStyle.Save, (path) =>
                 {
                     Objects.ShiftFS.Utils.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(stp));
                 });
            };
        }

        public override void OnLoad()
        {
            _moneyMate.Image = Properties.Resources.moneymate_transparent.ToTexture2D(UIManager.GraphicsDevice);
            
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _moneyMate.X = 0;
            _moneyMate.Y = 0;
            _moneyMate.Width = Width;
            _moneyMate.Height = 200;
            _title.Font = SkinEngine.LoadedSkin.Header2Font;
            _title.X = (Width - _title.Width) / 2;
            _title.Y = _moneyMate.Height + _title.Height + 30;

            _description.Y = _title.Y + _title.Height + 15;
            _description.MaxWidth = (Width - 60);
            _description.X = (Width - _description.Width) / 2;

            _download.Font = SkinEngine.LoadedSkin.Header3Font;
            _download.Y = _description.Y + _description.Height + 30;
            _download.X = (Width - _download.Width) / 2;


        }

    }
}
