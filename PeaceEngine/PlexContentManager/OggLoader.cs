using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace Plex.Engine.PlexContentManager
{
    public class OggLoader : ILoader<SoundEffect>
    {
        public IEnumerable<string> Extensions => new[] { ".OGG" };

        const double sc16 = 0x7FFF + 0.4999999999999999;

        public SoundEffect Load(Stream fobj)
        {
            using (var stream = new NVorbis.VorbisReader(fobj, false))
            {
                AudioChannels channels;
                switch (stream.Channels)
                {
                    case 1:
                        channels = AudioChannels.Mono;
                        break;
                    case 2:
                        channels = AudioChannels.Stereo;
                        break;
                    default:
                        throw new InvalidDataException($"Unsupported channel count {stream.Channels} (must be mono or stereo).");
                }
                var samps = new float[stream.TotalSamples * stream.Channels];
                Logger.Log($"TotalSamples = {stream.TotalSamples}");
                var data = new byte[samps.Length * 2];
                stream.ReadSamples(samps, 0, samps.Length);
                using (var ms = new MemoryStream(data))
                using (var write = new BinaryWriter(ms))
                    foreach (var samp in samps)
                        write.Write((short)(samp * sc16)); // convert to S16 int PCM
                return new SoundEffect(data, stream.SampleRate, channels);
            }
        }
    }
}
