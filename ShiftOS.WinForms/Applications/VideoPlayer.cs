using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using System.IO;

namespace ShiftOS.WinForms.Applications
{
    [AppscapeEntry("video_player", "Video Player", "Play .mp4 files or .wmv files as videos inside ShiftOS! Perfect for a shifted movie night.", 1524, 1000, "file_skimmer", "Entertainment")]
    [DefaultTitle("Video Player")]
    [Launcher("Video Player", false, null, "Entertainment")]
    [WinOpen("video_player")]
    public partial class VideoPlayer : UserControl, IShiftOSWindow
    {
        public VideoPlayer()
        {
            InitializeComponent();
        }

        public void OnLoad()
        {
            tmr.Interval = 50;
            tmr.Tick += (o, a) =>
            {
                try
                {
                    double end = wpaudio.currentMedia.duration;
                    double pos = wpaudio.Ctlcontrols.currentPosition;
                    pgplaytime.Maximum = (int)end;
                    pgplaytime.Value = (int)pos;
                }
                catch { }
            };
            tmr.Start();
        }

        public void OnSkinLoad()
        {
            wpaudio.uiMode = "none";
            wpaudio.Show();
            pgplaytime.Width = flcontrols.Width - btnplay.Width - 25;
            wpaudio.enableContextMenu = false;
        }

        public bool OnUnload()
        {
            if(!string.IsNullOrWhiteSpace(filePath))
                if (File.Exists(filePath))
                    File.Delete(filePath);
            tmr.Stop();
            return true;
        }

        public void OnUpgrade()
        {
        }

        Timer tmr = new Timer();

        string filePath = null;

        private void addSongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { ".mp4", ".wmv" }, FileOpenerStyle.Open, (result) =>
             {
                 var finf = ShiftOS.Objects.ShiftFS.Utils.GetFileInfo(result);
                 File.WriteAllBytes(finf.Name, ShiftOS.Objects.ShiftFS.Utils.ReadAllBytes(result));
                 filePath = finf.Name;
                 wpaudio.URL = filePath;
                 wpaudio.Ctlcontrols.play();
                 btnplay.Text = "Pause";
             });
        }

        private void btnplay_Click(object sender, EventArgs e)
        {
            if(wpaudio.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                wpaudio.Ctlcontrols.pause();
                btnplay.Text = "Play";
            }
            else if(wpaudio.playState == WMPLib.WMPPlayState.wmppsPaused)
            {
                wpaudio.Ctlcontrols.play();
                btnplay.Text = "Pause";
            }
        }

        bool scrubbing = false;

        private void startScrub(object sender, MouseEventArgs e)
        {
            scrubbing = true;
        }

        static public double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        private void pgplaytime_MouseMove(object sender, MouseEventArgs e)
        {
            if (wpaudio.playState == WMPLib.WMPPlayState.wmppsPlaying || wpaudio.playState == WMPLib.WMPPlayState.wmppsPaused)
                try
                {
                    if (scrubbing)
                    {
                        double end = wpaudio.currentMedia.duration;
                        double result = linear(e.X, 0, pgplaytime.Width, 0, end);
                        wpaudio.Ctlcontrols.currentPosition = result;
                    }
                }
                catch { }
        }

        private void pgplaytime_MouseUp(object sender, MouseEventArgs e)
        {
            scrubbing = false;
        }

    }
}
