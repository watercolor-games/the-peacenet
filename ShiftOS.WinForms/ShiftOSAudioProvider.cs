using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private float _vol = 1.0f;

        public float Volume
        {
            get
            {
                return _vol;
            }

            set
            {
                _vol = value;
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
