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
                var wc = new System.Net.WebClient();
                return JsonConvert.DeserializeObject<List<string>>(wc.DownloadString("http://getshiftos.ml/api.php?q=soundtrack")).Count;
            }
        }


        public float Volume
        {
            get
            {
                if (SaveSystem.CurrentSave == null)
                    return 1.0f;
                try
                {
                    return SaveSystem.CurrentSave.Settings.audioVolume;
                }
                catch
                {
                    SaveSystem.CurrentSave.Settings.audioVolume = 1.0f;
                    return 1.0f;
                }
            }

            set
            {
                SaveSystem.CurrentSave.Settings.audioVolume = value;
            }
        }

        public byte[] GetTrack(int index)
        {
            var wc = new System.Net.WebClient();
            var st = JsonConvert.DeserializeObject<List<string>>(wc.DownloadString("http://getshiftos.ml/api.php?q=soundtrack"));

            return wc.DownloadData(st[index]);
        }
    }

    
}
