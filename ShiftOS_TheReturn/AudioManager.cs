using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WMPLib;

namespace ShiftOS.Engine
{
    public static class AudioManager
    {
        public static WindowsMediaPlayer player = null;

        public static void Init()
        {
            player = new WindowsMediaPlayer();
            player.PlayStateChange += (o) =>
            {
                switch ((WMPPlayState)o)
                {
                    case WMPPlayState.wmppsPlaying:
                    case WMPPlayState.wmppsBuffering:
                    case WMPPlayState.wmppsReconnecting:
                        
                        break;
                    case WMPPlayState.wmppsReady:
                        PickRandomSong();
                        break;
                }
            };
            PickRandomSong();
        }

        public static void PickRandomSong()
        {
            var lst = JsonConvert.DeserializeObject<List<string>>(Properties.Resources.Songs);

            player.URL = lst[new Random().Next(0, lst.Count)];
        }
    }
}
