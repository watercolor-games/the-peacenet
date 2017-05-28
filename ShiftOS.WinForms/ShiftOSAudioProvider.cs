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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShiftOS.Engine;

namespace ShiftOS.WinForms
{
    class ShiftOSAudioProvider : IAudioProvider
    {
        public int Count
        {
            get
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                try {
                    string result = wc.DownloadString("http://getshiftos.ml/api.php?q=soundtrack");
                    return JsonConvert.DeserializeObject<List<string>>(result).Count;
                } catch { 
                    return JsonConvert.DeserializeObject<List<string>>("[]").Count;
                }
            }
        }


        public float Volume
        {
            get
            {
                if (SaveSystem.CurrentSave == null)
                    return 0.45f;
                if (TutorialManager.IsInTutorial || SaveSystem.CurrentSave.StoryPosition < 1)
                    return 0.45f;
                try
                {
                    return SaveSystem.CurrentSave.Settings.audioVolume;
                }
                catch
                {
                    SaveSystem.CurrentSave.Settings.audioVolume = 0.45F;
                    return 0.45F;
                }
                }

            set
            {
                SaveSystem.CurrentSave.Settings.audioVolume = value;
                //SaveSystem.SaveGame();
            }
        }

        public byte[] GetTrack(int index)
        {
            System.Net.WebClient wc = new System.Net.WebClient();

            try {
                string result = wc.DownloadString("http://getshiftos.ml/api.php?q=soundtrack");

                var st = JsonConvert.DeserializeObject<List<string>>(result);

                return wc.DownloadData(st[index]);
            } catch {
                return null;
            }
        }
    }

    
}
