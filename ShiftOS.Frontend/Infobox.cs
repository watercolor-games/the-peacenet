using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine;
using Plex.Frontend.Desktop;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;

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

    public class InfoboxMessage : Control, IPlexWindow
    {
        private Action _okAction = null;
        private Action<bool> _yesNoAction = null;

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
            _okAction = callback;
            btnok.Click += () =>
            {
                AppearanceManager.Close(this);
            };
        }

        public void ShowYesNo(Action<bool> callback)
        {
            AppearanceManager.SetupDialog(this);
            flyesno.Visible = true;
            txtinput.Visible = false;
            btnok.Visible = false;
            _yesNoAction = callback;
            btnyes.Click += () =>
            {
                _yesNoResult = true;
                AppearanceManager.Close(this);
            };
            btnno.Click += () =>
            {
                AppearanceManager.Close(this);
            };
            _yesNoResult = false;
        }

        private bool _yesNoResult = false;

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
            _okAction?.Invoke();
            _yesNoAction?.Invoke(_yesNoResult);
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
            lbmessage.AutoSize = true;
            lbmessage.MaxWidth = UIManager.Viewport.Width / 3;

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
            this.btnyes.Image = FontAwesome.check.ToTexture2D(UIManager.GraphicsDevice);
            // 
            // btnno
            // 
            this.btnno.AutoSize = true;
            this.btnno.Text = Localization.Parse("{GEN_NO}");
            this.btnno.Image = FontAwesome.times.ToTexture2D(UIManager.GraphicsDevice);
            // 
            // btnok
            // 
            this.btnok.AutoSize = true;
            this.btnok.X = 140;
            this.btnok.Y = 140;
            this.btnok.Text = Localization.Parse("{GEN_OK}");
            this.btnok.Image = FontAwesome.check.ToTexture2D(UIManager.GraphicsDevice);
            // 
            // pbicon
            // 
            this.pbicon.X = 14;
            this.pbicon.Y = 19;
            this.pbicon.Width = 64;
            this.pbicon.Height = 64;
            this.pbicon.Image = FontAwesome.warning.ToTexture2D(UIManager.GraphicsDevice);
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

            OnLayout(new GameTime());
        }

        protected override void OnLayout(GameTime gameTime)
        {
            try
            {
                int contentHeight = Math.Max(pbicon.Height, lbmessage.Height);

                pbicon.X = 15;
                pbicon.Y = 15;

                lbmessage.Y = pbicon.Y;
                lbmessage.X = pbicon.X + pbicon.Width + 15;

                int width = lbmessage.X + lbmessage.Width + 15;

                Width = width;
                if (txtinput.Visible)
                {
                    txtinput.Y = lbmessage.Y + lbmessage.Height + 10;
                    txtinput.Width = lbmessage.Width;
                    txtinput.X = lbmessage.X;
                    contentHeight = Math.Max(pbicon.Height, lbmessage.Height + 15 + txtinput.Height);
                    UIManager.FocusedControl = txtinput;
                }
                else
                {
                    UIManager.FocusedControl = this;
                }

                Height = 15 + contentHeight + 15 + btnok.Height + 15;

                btnok.Y = this.Height - btnok.Height - 10;
                btnok.X = (Width - btnok.Width) / 2;

                flyesno.Y = this.Height - flyesno.Height - 10;
                flyesno.X = (Width - flyesno.Width) / 2;

                this.Parent.X = (UIManager.Viewport.Width - Width) / 2;
                this.Parent.Y = (UIManager.Viewport.Height - Height) / 2;

            }
            catch { }
        }


        private TextControl lbmessage;
        private ItemGroup flyesno;
        private Button btnyes;
        private Button btnno;
        private Button btnok;
        private PictureBox pbicon;
        private TextInput txtinput;

        

    }
}
