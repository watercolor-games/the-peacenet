/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
    [MultiplayerOnly]
    [Launcher("{TITLE_DOWNLOADER}", true, "al_downloader", "{AL_NETWORKING}")]
    [DefaultIcon("iconDownloader")]
    [WinOpen("{WO_DOWNLOADER}")]
    [DefaultTitle("{TITLE_DOWNLOADER}")]
    [RequiresUpgrade("downloader")]
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

        /// <summary>
        /// Gets a Shiftnet download speed in bytes based on the user's subscription.
        /// </summary>
        /// <returns>Download speed in bytes.</returns>
        public static int GetDownloadSpeed()
        {
            return GetAllSubscriptions()[SaveSystem.CurrentSave.ShiftnetSubscription].DownloadSpeed;
        }

        public static ShiftOS.Objects.EngineShiftnetSubscription[] GetAllSubscriptions()
        {
            //For now we'll have them hard-coded into the client but in future they'll be in the MUD.
            return JsonConvert.DeserializeObject<ShiftOS.Objects.EngineShiftnetSubscription[]>(Properties.Resources.ShiftnetServices);
        }

        public static void StartDownload(Download down)
        {
            var t = new Thread(() =>
            {
                _downloads.Add(down);
                DownloadStarted?.Invoke(down);
                for (int i = 0; i < down.Bytes.Length; i += GetDownloadSpeed())
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

    public static class DownloaderDebugCommands
    {
        [Command("setsubscription", description ="Use to set the current shiftnet subscription.", usage ="{value:int32}")]
        [RequiresArgument("value")]
        public static bool SetShiftnetSubscription(Dictionary<string, object> args)
        {
            int val = 0;
            if(int.TryParse(args["value"].ToString(), out val) == true)
            {
                SaveSystem.CurrentSave.ShiftnetSubscription = val;
                SaveSystem.SaveGame();
            }
            else
            {
                Console.WriteLine("Not a valid 32-bit integer.");
            }
            return true;
        }
    }
}
