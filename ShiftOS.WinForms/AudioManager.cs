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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShiftOS.WinForms
{
    public static class AudioManager
    {

        public static void Test()
        {
            
        }

        internal static byte[] GetRandomSong()
        {
            var r = new Random().Next(1, 10);
            switch (r)
            {
                case 1:
                    return Properties.Resources.Ambient1;
                case 2:
                    return Properties.Resources.Ambient2;
                case 3:
                    return Properties.Resources.Ambient3;
                case 4:
                    return Properties.Resources.Ambient4;
                case 5:
                    return Properties.Resources.Ambient5;
                case 6:
                    return Properties.Resources.Ambient6;
                case 7:
                    return Properties.Resources.Ambient7;
                case 8:
                    return Properties.Resources.Ambient8;
                default:
                    return Properties.Resources.Ambient9;

            }
        }

        internal static void StartAmbientLoop()
        {
            var athread = new Thread(() =>
            {
                MemoryStream str = null;
                NAudio.Wave.Mp3FileReader mp3 = null;
                NAudio.Wave.WaveOut o = null;
                bool shuttingDown = false;

                Engine.AppearanceManager.OnExit += () =>
                {
                    shuttingDown = true;
                    o?.Stop();
                    o?.Dispose();
                    mp3?.Close();
                    mp3?.Dispose();
                    str?.Close();
                    str?.Dispose();
                };
                while (shuttingDown == false)
                {
                    if (Engine.SaveSystem.CurrentSave != null)
                    {
                        if (Engine.SaveSystem.CurrentSave.MusicEnabled)
                        {
                            str = new MemoryStream(GetRandomSong());
                            mp3 = new NAudio.Wave.Mp3FileReader(str);
                            o = new NAudio.Wave.WaveOut();
                            o.Init(mp3);
                            bool c = false;
                            o.Play();
                            o.PlaybackStopped += (s, a) =>
                            {
                                c = true;
                            };

                            while (!c)
                            {
                                if (Engine.SaveSystem.CurrentSave.MusicEnabled)
                                {
                                    try
                                    {
                                        o.Volume = (float)Engine.SaveSystem.CurrentSave.MusicVolume / 100;
                                    }
                                    catch { }
                                }
                                else
                                {
                                    o.Stop();
                                    c = true;
                                }
                                Thread.Sleep(10);
                            }
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
            });
            athread.IsBackground = true;
            athread.Start();
        }
    }
}
