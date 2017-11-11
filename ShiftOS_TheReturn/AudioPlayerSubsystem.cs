using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine
{
    [Obsolete("You do know MonoGame has support for audio, right Michael?")]
    public class AudioPlayerSubsystem
    {
        private static IAudioPlayer _player = null;

        public static void Init(IAudioPlayer player)
        {
            _player = player;
        }

        public static void Startup()
        {
            _player?.Startup();
        }

        public static void Shutdown()
        {
            _player?.Shutdown();
        }

        public static void Infobox()
        {
            _player?.Infobox();
        }

        public static void Notification()
        {
            _player?.Notification();
        }

    }

    public interface IAudioPlayer
    {
        void Startup();
        void Shutdown();
        void Infobox();
        void Notification();
    }
}
