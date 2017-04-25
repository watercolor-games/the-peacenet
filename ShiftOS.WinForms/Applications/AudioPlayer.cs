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
using ShiftOS.Engine;
using NAudio;
using System.Threading;

namespace ShiftOS.WinForms.Applications
{
    [AppscapeEntry("Audio Player", "Play music and other sounds on your computer.", 3047, 1000, "file_skimmer", "Entertainment")]
    [Launcher("Audio Player", false, null, "Entertainment")]
    [WinOpen("audio_player")]
    [DefaultTitle("Audio Player")]
    public partial class AudioPlayer : UserControl, IShiftOSWindow
    {
        public AudioPlayer()
        {
            InitializeComponent();
        }

        NAudio.Wave.WaveOut o = null;
        

        public void OnLoad()
        {
            wpaudio.Hide();
        }

        public void OnSkinLoad()
        {
            pgplaytime.Width = flcontrols.Width - btnplay.Width - 25;
        }

        public bool OnUnload()
        {
            o?.Dispose();
            mp3?.Dispose();
            memstream?.Dispose();
            return true;
        }

        public void OnUpgrade()
        {
            
        }

        private void addSongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { ".mp3", ".wav" }, FileOpenerStyle.Open, (path) =>
             {
                 if (!lbtracks.Items.Contains(path))
                     lbtracks.Items.Add(path);
                 else
                     Infobox.Show("Song already added!", "That song is already added to the Audio Player playlist.");
             });
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbtracks.Items.Clear();
        }

        private void shuffleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lst = new object[lbtracks.Items.Count];
            lbtracks.Items.CopyTo(lst, 0);
            var shuffle = new List<object>(lst);
            shuffle.Shuffle();
            lbtracks.Items.Clear();
            lbtracks.Items.AddRange(shuffle.ToArray());
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbtracks.Items.RemoveAt(lbtracks.SelectedIndex);
        }

        private void btnplay_Click(object sender, EventArgs e)
        {
            if(o == null)
            {
                if (lbtracks.Items.Count > 0)
                {
                    Play(lbtracks.Items[0].ToString());
                    btnplay.Text = "Pause";
                }
                else
                    Infobox.Show("Error", "No tracks to play! Please add a track to the playlist.");
            }
            else if(o.PlaybackState == NAudio.Wave.PlaybackState.Paused)
            {
                o.Resume();
                btnplay.Text = "Pause";
            }
            else if(o.PlaybackState == NAudio.Wave.PlaybackState.Playing)
            {
                o.Pause();
                btnplay.Text = "Play";
            }
        }

        System.IO.Stream memstream = null;
        NAudio.Wave.Mp3FileReader mp3 = null;

        public void Play(string track)
        {
            if (o != null)
            {
                o.Dispose();
                mp3.Dispose();
                memstream.Dispose();
            }
            var bytes = ShiftOS.Objects.ShiftFS.Utils.ReadAllBytes(track);
            memstream = new System.IO.MemoryStream(bytes);
            mp3 = new NAudio.Wave.Mp3FileReader(memstream);
            o = new NAudio.Wave.WaveOut();
            o.Init(mp3);
            o.Play();

            pgplaytime.Value = 0;
            pgplaytime.Maximum = (int)mp3.Length;
            new Thread(() =>
            {
                while (o.PlaybackState == NAudio.Wave.PlaybackState.Playing || o.PlaybackState == NAudio.Wave.PlaybackState.Paused)
                {
                    long time = mp3.Position;
                    this.Invoke(new Action(() =>
                    {
                        pgplaytime.Value = (int)time;
                    }));
                    Thread.Sleep(50);
                }
            }).Start();
        }

        private void lbtracks_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Play(lbtracks.SelectedItem.ToString());
            }
            catch { }
        }
    }

    public static class ListExtensions
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
