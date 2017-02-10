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
using System.IO;
using System.IO.Compression;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Downloader", false, null, "Networking")]
    [DefaultIcon("iconDownloader")]
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
            DownloadManager.DownloadStarted += started;
            DownloadManager.DownloadCompleted += completed;
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
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

            for(int i = 0; i < DownloadManager.Downloads.Length; i++)
            {
                var dctrl = new DownloadControl(i);
                if(heightMultiplier < 10)
                {
                    heightMultiplier++;
                }
                
                fllist.Controls.Add(dctrl);
                dctrl.Show();
            }

            if (heightMultiplier == 0)
                heightMultiplier = 1;

            this.ParentForm.Height = 150 * heightMultiplier;
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

        public static string Compress(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        public static string Decompress(string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray());
            }
        }

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
                    _downloads[_downloads.IndexOf(down)].Progress = (int)((float)(i / down.Bytes.Length) * 100);
                    ProgressUpdate?.Invoke(_downloads.IndexOf(down), (int)((float)(i / down.Bytes.Length) * 100));
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
