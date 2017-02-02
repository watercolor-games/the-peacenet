using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using ShiftOS.Engine;
using Newtonsoft.Json;
using ShiftOS.WinForms.Controls;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Downloader", false, null, "Networking")]
    public partial class Downloader : UserControl, IShiftOSWindow
    {
        public Downloader()
        {
            InitializeComponent();
        }

        Action<int, int> pupdate = null;
        Action<string> completed = null;
        Action<Download> started = null;

        public void OnLoad()
        {
            SetupUI();
            pupdate = (i, o) =>
            {
                this.Invoke(new Action(() =>
                {
                    SetupUI();
                }));
            };
            started = (i) =>
            {
                this.Invoke(new Action(() =>
                {
                    SetupUI();
                }));
            }; completed = (i) =>
            {
                this.Invoke(new Action(() =>
                {
                    SetupUI();
                }));
            };
            DownloadManager.ProgressUpdate += pupdate;
            DownloadManager.DownloadStarted += started;
            DownloadManager.DownloadCompleted += completed;
        }

        public void OnSkinLoad()
        {
            SetupUI();
        }

        public bool OnUnload()
        {
            DownloadManager.ProgressUpdate -= pupdate;
            DownloadManager.DownloadStarted -= started;
            DownloadManager.DownloadCompleted -= completed;
            return true;
        }

        public void OnUpgrade()
        {
        }

        public void SetupUI()
        {
            fllist.Controls.Clear();

            int heightMultiplier = 0;

            foreach(var download in DownloadManager.Downloads)
            {
                var pnl = new Panel();
                pnl.Width = fllist.Width;
                pnl.Height = 50;
                var picpreview = new PictureBox();
                picpreview.Size = new Size(42, 42);
                picpreview.Image = FileSkimmerBackend.GetImage(download.Destination);
                picpreview.Location = new Point(4, 4);
                if (heightMultiplier < 5)
                    heightMultiplier++;
                pnl.Controls.Add(picpreview);
                picpreview.Show();
                var prg = new ShiftedProgressBar();
                prg.Maximum = 100;
                prg.Value = download.Progress;
                prg.Width = pnl.Width - 8;
                prg.Left = 4;
                prg.Top = picpreview.Height + 8;
                prg.Height = 20;
                var lbtitle = new Label();
                lbtitle.Tag = "header1";
                lbtitle.Text = download.ShiftnetUrl;
                lbtitle.Top = 4;
                lbtitle.Left = 8 + picpreview.Height;
                pnl.Controls.Add(lbtitle);
                lbtitle.Show();
                lbtitle.AutoSize = true;
                pnl.Controls.Add(prg);
                prg.Show();

                fllist.Controls.Add(pnl);
                pnl.Show();
                ControlManager.SetupControls(pnl);
            }

            if (heightMultiplier == 0)
                heightMultiplier = 1;

            this.Parent.Height = 50 * heightMultiplier;
        }
    }

    public static class DownloadManager
    {
        public static Download[] Downloads
        {
            get
            {
                return _downloads.ToArray();
            }
        }

        private static List<Download> _downloads = new List<Download>();

        public static event Action<int, int> ProgressUpdate;

        public static event Action<string> DownloadCompleted;

        public static event Action<Download> DownloadStarted;

        public static void StartDownload(Download down)
        {
            var t = new Thread(() =>
            {
                int byteWrite = 256;
                _downloads.Add(down);
                DownloadStarted?.Invoke(down);
                for (int i = 0; i < down.Bytes.Length; i += byteWrite)
                {
                    Thread.Sleep(1000);
                    _downloads[_downloads.IndexOf(down)].Progress = i / down.Bytes.Length;
                    ProgressUpdate?.Invoke(_downloads.IndexOf(down), i / down.Bytes.Length);
                }
                ShiftOS.Objects.ShiftFS.Utils.WriteAllBytes(down.Destination, down.Bytes);
                _downloads.Remove(down);
                DownloadCompleted?.Invoke(down.Destination);
            });
            t.IsBackground = true;
            t.Start();
        }
    }

    public class Download
    {

        public string ShiftnetUrl { get; set; }
        public string Destination { get; set; }
        public byte[] Bytes { get; set; }
        public int Progress { get; set; }
    }
}
