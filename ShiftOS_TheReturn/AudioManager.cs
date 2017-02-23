#define NOSOUND

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace ShiftOS.Engine
{
    public static class AudioManager
    {
        private static WaveOut _out = null;
        private static AudioFileReader _reader = null;
        private static IAudioProvider _provider = null;
        private static bool _running = true;

        public static void Init(IAudioProvider _p)
        {
#if !NOSOUND
            _provider = _p;
            AppearanceManager.OnExit += () =>
            {
                _running = false;
                _out?.Stop();
                _reader?.Dispose();
                _out?.Dispose();
                System.IO.File.Delete("temp.mp3");
            };
            var t = new Thread(() =>
            {
                SaveSystem.GameReady += () =>
                {
                    while(_out == null)
                    {

                    }
                    _out.Volume = _provider.Volume;
                };
                Random rnd = new Random();
                while(_running == true)
                {
                    int track = rnd.Next(0, _provider.Count);
                    byte[] mp3 = _provider.GetTrack(track);
                    System.IO.File.WriteAllBytes("temp.mp3", mp3);
                    _reader = new AudioFileReader("temp.mp3");
                    _out = new WaveOut();
                    _out.Init(_reader);
                    _out.Volume = _provider.Volume;

                    _out.Play();
                    while(_out.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(5000); //even when the player isn't playing, this will give a good delay between songs.
                    }
                    _reader.Dispose();
                    _out.Dispose();
                }
            });
            t.IsBackground = true;
            t.Start();
#endif
        }

        public static void SetVolume(float volume)
        {
            _provider.Volume = volume; //persist between songs
            _out.Volume = volume;
        }

        internal static void Kill()
        {
            _running = false;
            _out.Stop();
            _out.Dispose();
            _reader.Dispose();
        }
    }

    public interface IAudioProvider
    {
        /// <summary>
        /// Gets a byte[] array corresponding to an MP3 track given an index.
        /// </summary>
        /// <param name="index">A track index to use when finding the right track.</param>
        /// <returns>The MP3 byte[] array.</returns>
        byte[] GetTrack(int index);

        /// <summary>
        /// Gets the 1-based count of all available tracks.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets or sets the track player's volume.
        /// </summary>
        float Volume { get; set; }
    }
}
