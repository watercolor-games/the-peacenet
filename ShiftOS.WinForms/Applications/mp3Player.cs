using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShiftOS.Engine
{
    [Launcher("WAV Player", true, "wav_player_al")]
    [RequiresUpgrade("wav_player")]
    [WinOpen("wav_player")]
    public partial class UserControl1 : UserControl, IShiftOSWindow
    {
        string path;
        public UserControl1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "WAV|*.wav";

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
                mp3FilePath.Text = ofd.FileName;
            }
        }

        private void stopMp3_Click(object sender, EventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.Stop();
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

        private void button2_Click(object sender, EventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = path;
            player.Load();
            player.Play();
        }
    }
}
