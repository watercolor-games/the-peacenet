using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine;
using Plex.Frontend.Desktop;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Frontend.GUI;

namespace Plex.Frontend
{
    public class Infobox : IInfobox
    {
        public void Open(string title, string msg, Action callback = null)
        {
            var imsg = new InfoboxMessage(title, msg);
            imsg.ShowPrompt(callback);
        }

        public void PromptText(string title, string message, Action<string> callback, bool isPassword)
        {
            var imsg = new InfoboxMessage(title, message);
            imsg.ShowText(callback);
        }

        public void PromptYesNo(string title, string message, Action<bool> callback)
        {
            var imsg = new InfoboxMessage(title, message);
            imsg.ShowYesNo(callback);

        }
    }

    public class InfoboxMessage : GUI.Control, IPlexWindow
    {
        public InfoboxMessage(string title, string message)
        {
            InitializeComponent();
            lbmessage.Text = Localization.Parse(message);
            Title = title;
        }

        public string Title { get; private set; }

        public void OnLoad()
        {
            AudioPlayerSubsystem.Infobox();
            AppearanceManager.SetWindowTitle(this, Title);
        }

        public void ShowPrompt(Action callback)
        {
            AppearanceManager.SetupDialog(this);
            flyesno.Visible = false;
            txtinput.Visible = false;
            btnok.Visible = true;
            btnok.Click += () =>
            {
                callback?.Invoke();
                AppearanceManager.Close(this);
            };
        }

        public void ShowYesNo(Action<bool> callback)
        {
            AppearanceManager.SetupDialog(this);
            flyesno.Visible = true;
            txtinput.Visible = false;
            btnok.Visible = false;
            btnyes.Click += () =>
            {
                callback?.Invoke(true);
                AppearanceManager.Close(this);
            };
            btnno.Click += () =>
            {
                callback?.Invoke(false);
                AppearanceManager.Close(this);
            };
        }

        public void ShowText(Action<string> callback)
        {
            AppearanceManager.SetupDialog(this);
            flyesno.Visible = false;
            btnok.Visible = true;
            txtinput.Visible = true;
            btnok.Click += () =>
            {
                callback?.Invoke(txtinput.Text);
                AppearanceManager.Close(this);
            };
            txtinput.BringToFront();
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

        //NOTE: The following code is ported over from Windows Forms.
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtinput = new TextInput();
            this.lbmessage = new TextControl();
            this.flyesno = new ItemGroup();
            this.btnyes = new Button();
            this.btnno = new Button();
            this.btnok = new Button();
            this.pbicon = new PictureBox();
            // 
            // txtinput
            // 
            this.txtinput.X = 88;
            this.txtinput.Y = 116;
            this.txtinput.Width = 250;
            this.txtinput.Height = 20;
            // 
            // lbmessage
            // 
            this.lbmessage.X = 85;
            this.lbmessage.Y = 19;
           this.lbmessage.Width = 213;
            this.lbmessage.Height = 94;
            this.lbmessage.Text = "label1";
            this.lbmessage.Alignment = Engine.GUI.TextAlignment.Left;
            // 
            // flyesno
            // 
            this.flyesno.AutoSize = true;
            this.flyesno.AddControl(this.btnyes);
            this.flyesno.AddControl(this.btnno);
            this.flyesno.X = 129;
            this.flyesno.Y = 134;
            // 
            // btnyes
            // 
            this.btnyes.AutoSize = true;
            this.btnyes.Text = Localization.Parse("{GEN_YES}");
            // 
            // btnno
            // 
            this.btnno.AutoSize = true;
            this.btnno.Text = Localization.Parse("{GEN_NO}");
            // 
            // btnok
            // 
            this.btnok.AutoSize = true;
            this.btnok.X = 140;
            this.btnok.Y = 140;
            this.btnok.Text = Localization.Parse("{GEN_OK}");
            // 
            // pbicon
            // 
            this.pbicon.X = 14;
            this.pbicon.Y = 19;
            this.pbicon.Width = 64;
            this.pbicon.Height = 64;
            this.pbicon.Image = Properties.Resources.justthes.ToTexture2D(UIManager.GraphicsDevice);
            this.pbicon.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            // 
            // Dialog
            // 
            this.Width = 341;
            this.Height = 157;
            this.AddControl(pbicon);
            this.AddControl(txtinput);
            this.AddControl(btnok);
            this.AddControl(flyesno);
            this.AddControl(lbmessage);
        }

        protected override void OnLayout(GameTime gameTime)
        {
            try
            {
                btnok.Y = this.Height - btnok.Height - 10;
                flyesno.Y = this.Height - flyesno.Height - 10;
                txtinput.Width = lbmessage.Width;
                txtinput.X = lbmessage.X;
                txtinput.Y = btnok.Y - txtinput.Height - 2;
                if (txtinput.Visible)
                {
                    lbmessage.Height = (txtinput.Y - lbmessage.Y) - 2;
                }
                
            }
            catch { }
        }


        private Control panel1;
        private TextControl lbmessage;
        private ItemGroup flyesno;
        private Button btnyes;
        private Button btnno;
        private Button btnok;
        private PictureBox pbicon;
        private TextInput txtinput;

    }
}
