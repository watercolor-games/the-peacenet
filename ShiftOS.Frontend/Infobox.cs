using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Frontend.GUI;

namespace ShiftOS.Frontend
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

    public class InfoboxMessage : GUI.Control, IShiftOSWindow
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
            AppearanceManager.SetWindowTitle(this, Title);
        }

        public void ShowPrompt(Action callback)
        {
            AppearanceManager.SetupDialog(this);
            flyesno.Visible = false;
            txtinput.Visible = false;
            btnok.Visible = true;
            btnok.Click += callback;
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
            };
            btnno.Click += () =>
            {
                callback?.Invoke(false);
            };
        }

        public void ShowText(Action<string> callback)
        {
            Title = "Not yet implemented.";
            lbmessage.Text = "This feature hasn't yet been implemented.";
            ShowPrompt(null);

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
           this.lbmessage.Width = 253;
            this.lbmessage.Height = 94;
            this.lbmessage.Text = "label1";
            this.lbmessage.TextAlign = TextAlign.MiddleLeft;
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
            this.btnok.Text = Localization.Parse("{GEN_OK}");
            // 
            // pbicon
            // 
            this.pbicon.X = 14;
            this.pbicon.Y = 19;
            this.pbicon.Width = 64;
            this.pbicon.Height = 64;
            // 
            // Dialog
            // 
            this.Width = 341;
            this.Height = 127;
            this.AddControl(pbicon);
            this.AddControl(btnok);
            this.AddControl(flyesno);
            this.AddControl(lbmessage);

            this.Layout();
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
