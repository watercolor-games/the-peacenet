using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Applications
{
    [MultiplayerOnly]
    [Launcher("MUD Chat", true, "al_mud_chat")]
    [RequiresUpgrade("mud_fundamentals")]
    [WinOpen("chat")]
    public partial class Chat : UserControl, IShiftOSWindow
    {
        public Chat(string chatId)
        {
            InitializeComponent();
            id = chatId;
            ServerManager.MessageReceived += (msg) =>
            {
                if (msg.Name == "cbroadcast")
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            rtbchat.Text += msg.Contents + Environment.NewLine;
                        }));
                    }
                    catch { }
                }
            };
        }

        private string id = "";

        public void OnLoad()
        {
            var save = SaveSystem.CurrentSave;
            ServerManager.SendMessage("chat_join", $@"{{
    id: ""{id}"",
    user: {JsonConvert.SerializeObject(save)}
}}");
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            var save = SaveSystem.CurrentSave;
            ServerManager.SendMessage("chat_leave", $@"{{
    id: ""{id}"",
    user: {JsonConvert.SerializeObject(save)}
}}");

            return true;
        }

        public void OnUpgrade()
        {
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void txtuserinput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                var save = SaveSystem.CurrentSave;

                ServerManager.SendMessage("chat", $@"{{
    id: ""{id}"",
    user: {JsonConvert.SerializeObject(save)},
    msg: ""{txtuserinput.Text}""
}}");
                txtuserinput.Text = "";
            }
        }
    }
}
