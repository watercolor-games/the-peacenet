using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Plex.Frontend;

namespace Plex.Engine
{
    public partial class CrashHandlerUI : Form
    {
        //I'm fucking sorry...
        //but I can't do visual designers.
        //I'm doing this the Plex way.

        private Label _title = new Label();
        private Label _description = new Label();
        private RichTextBox _crashdata = new RichTextBox();
        private Button _quit = new Button();
        private Button _continue = new Button();
        private Exception _exception = null;
        private string _summary = "";

        public CrashHandlerUI(Exception ex, string summary)
        {
            _exception = ex;
            _summary = summary;
            InitializeComponent();

            this.BackColor = SkinEngine.LoadedSkin.ControlColor;
            this.ForeColor = SkinEngine.LoadedSkin.ControlTextColor;
            this.Font = SkinEngine.LoadedSkin.MainFont;

            this.Controls.Add(_title);
            this.Controls.Add(_description);
            this.Controls.Add(_crashdata);
            this.Controls.Add(_quit);
            this.Controls.Add(_continue);
        }

        private void CrashHandlerUI_Load(object sender, EventArgs e)
        {
            _title.Left = 15;
            _title.Top = 15;
            _title.AutoSize = true;
            _title.Font = SkinEngine.LoadedSkin.HeaderFont;
            _title.Text = _exception.GetType().Namespace + "." + _exception.GetType().Name;

            _description.Left = 15;
            _description.Top = _title.Top + _title.Height + 5;
            _description.MaximumSize = new Size(this.ClientSize.Width - 30, 0);
            _description.AutoSize = true;
            _description.Text = _exception.Message + "\r\n\r\nYou can view more information in the text box below. Additionally, you may choose to either shut down the engine, quitting the game and returning you to your development environment, or to attempt to continue the engine's main loop, continuing the game. If you see this error repeatedly it may be time to bring out the debugger.";

            _quit.Text = "Quit the game";
            _quit.AutoSize = true;
            _quit.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _quit.Left = ClientSize.Width - _quit.Width - 15;
            _quit.Top = ClientSize.Height - _quit.Height - 15;

            _continue.Text = "Continue the game";
            _continue.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _continue.AutoSize = true;
            _continue.Left = _quit.Left - _continue.Width - 10;
            _continue.Top = _quit.Top;

            _crashdata.Text = _summary;
            _crashdata.BackColor = SkinEngine.LoadedSkin.TerminalBackColorCC.ToColor();
            _crashdata.ForeColor = SkinEngine.LoadedSkin.TerminalForeColorCC.ToColor();
            _crashdata.Font = SkinEngine.LoadedSkin.TerminalFont;
            _crashdata.Left = 15;
            _crashdata.Top = _description.Top + _description.Height + 15;
            _crashdata.Width = ClientSize.Width - 30;
            _crashdata.Height = _quit.Top - _crashdata.Top - 30;

            _continue.Click += (o, a) =>
            {
                this.Close();
            };
            _quit.Click += (o,a) =>
            {
                DialogResult = DialogResult.Retry;
                this.Close();
            };

            _title.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _description.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _continue.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _quit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _crashdata.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        }
    }
}
