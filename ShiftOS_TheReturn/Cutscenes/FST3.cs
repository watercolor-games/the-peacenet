using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Engine.Cutscenes
{
    public class FST3: IVideoFormat
    {
        // Memset for C# - StackOverflow saves the day again...
        private static class Util
        {
            static Util()
            {
                var dynamicMethod = new DynamicMethod("Memset", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, null, new [] { typeof(IntPtr), typeof(byte), typeof(int) }, typeof(Util), true);

                var generator = dynamicMethod.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldarg_2);
                generator.Emit(OpCodes.Initblk);
                generator.Emit(OpCodes.Ret);

                MemsetDelegate = (Action<IntPtr, byte, int>)dynamicMethod.CreateDelegate(typeof(Action<IntPtr, byte, int>));
            }

            public static void Memset(byte[] array, int offset, byte what, int length)
            {
                var gcHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                MemsetDelegate(gcHandle.AddrOfPinnedObject(), what, length);
                gcHandle.Free();
            }

            private static Action<IntPtr, byte, int> MemsetDelegate;
        }
        
        private class BitReader
        {
            private byte[] bytes;
            private byte bitpos = 0;
            private int bytepos = 0;
            public BitReader(byte[] bytes)
            {
                this.bytes = bytes;
            }
            
            public byte ReadBit()
            {
                byte ret = ((bytes[bytepos] & (1 << bitpos)) != 0) ? (byte) 1 : (byte) 0;
                bitpos++;
                if (bitpos == 8)
                {
                    bitpos = 0;
                    bytepos++;
                }
                return ret;
            }
        }
        
        private enum OpCode
        {
            AsIs,      // read  0 bytes
            OneColour, // read  1 byte
            TwoColour, // read  4 bytes
            Literal    // read 16 bytes
        }
        
        private const uint magic = 0x46535433; // "3TSF"
        
        public uint Length { get; private set; }
        public float MsPerFrame { get; private set; }
        
        private readonly ushort width, height, gridWidth, gridHeight, bitfieldSize;
        private List<byte[]> soundLzwDict;
        private byte[] imgData;
        private Color[] palette;
        private uint curFrame = 0;
        private bool[] palFrames;
        
        private byte k;
        private int prevCode;
        // The first byte of the first frame's audio component needs to
        // be treated literally so that k and prevCode will be
        // initialised to decompress the other codepoints. This flag
        // keeps track of that behaviour.
        private bool init = false;
        
        private WaveFormat fmt;
        
        private Stream fobj;
        private GraphicsContext gfx;
        
        public FST3(Stream fobj)
        {
            using (var read = new BinaryReader(fobj, Encoding.UTF8, true))
            {
                if (read.ReadUInt32() != magic)
                    throw new InvalidDataException("This is not an FST3 file.");
                
                gridWidth = read.ReadUInt16();
                gridHeight = read.ReadUInt16();
                Length = read.ReadUInt32();
                MsPerFrame = read.ReadSingle();
                ushort soundFreq = read.ReadUInt16();
                byte soundBits = read.ReadByte();
                byte soundChannels = read.ReadByte();
                fmt = new WaveFormat(soundFreq, soundBits, soundChannels);
                
                palFrames = new bool[Length];
                uint numPalFrames = read.ReadUInt32();
                for (long i = 0; i < numPalFrames; i++)
                    palFrames[read.ReadUInt32()] = true;
            }
            
            width = (ushort)(gridWidth * 4);
            height = (ushort)(gridHeight * 4);
            
            bitfieldSize = (ushort)((gridWidth * gridHeight) / 4);
            
            soundLzwDict = new List<byte[]>(256);
            for (byte i = 0; i < 256; i++)
                soundLzwDict.Add(new byte[1] { i });
            
            imgData = new byte[width * height];
            
            this.fobj = fobj;
        }
        
        public VideoFrame NextFrame(GraphicsContext gfx)
        {
            using (var read = new BinaryReader(fobj, Encoding.UTF8, true))
            {
                if (palFrames[curFrame])
                    for (short i = 0; i < 256; i++)
                        palette[i] = new Color(read.ReadByte(), read.ReadByte(), read.ReadByte());
                var bread = new BitReader(read.ReadBytes(bitfieldSize));
                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridHeight; x++)
                    {
                        byte opcode = bread.ReadBit();
                        opcode |= (byte)(bread.ReadBit() << 1);
                        switch ((OpCode) opcode)
                        {
                            case OpCode.AsIs:
                                break;
                            case OpCode.OneColour:
                                byte colour = read.ReadByte();
                                for (int gy = 0; gy < 4; gy++)
                                    Util.Memset(imgData, y * 4 * gridWidth + x * 4 + gy * width, colour, 4);
                                break;
                            case OpCode.TwoColour:
                                byte[] colours = read.ReadBytes(2);
                                var cbread = new BitReader(read.ReadBytes(2));
                                for (int gy = 0; gy < 4; gy++)
                                    for (int gx = 0; gx < 4; gx++)
                                        imgData[y * 4 * gridWidth + x * 4 + gy * width + gx] = colours[cbread.ReadBit()];
                                break;
                            case OpCode.Literal:
                                for (int gy = 0; gy < 4; gy++)
                                    Array.Copy(read.ReadBytes(4), 0, imgData, y * 4 * gridWidth + x * 4 + gy * width, 4);
                                break;
                        }
                    }
                }
                
                bread = new BitReader(read.ReadBytes(read.ReadUInt16()));
                var audioData = new byte[read.ReadUInt16()];
                int c = 0;
                if (!init)
                {
                    prevCode = k = audioData[0] = read.ReadByte();
                    init = true;
                    c = 1;
                }
                while (c < audioData.Length)
                {
                    int count = 0;
                    for (int i = 0; i < 32; i++)
                        if (((1 << i) & (soundLzwDict.Count - 1)) != 0)
                            count = i;
                    count++;
                    int currCode = 0;
                    for (int i = 0; i < count; i++)
                        currCode |= bread.ReadBit() << i;
                    byte[] pattern;
                    if (currCode == soundLzwDict.Count)
                    {
                        pattern = new byte[soundLzwDict[prevCode].Length + 1];
                        Array.Copy(soundLzwDict[prevCode], 0, pattern, 0, soundLzwDict[prevCode].Length);
                        pattern[soundLzwDict[prevCode].Length] = k;
                    }
                    else
                        pattern = soundLzwDict[currCode];
                    Array.Copy(pattern, 0, audioData, c, soundLzwDict[prevCode].Length);
                    k = pattern[0];
                    byte[] ncode = new byte[soundLzwDict[prevCode].Length + 1];
                    Array.Copy(soundLzwDict[prevCode], 0, pattern, 0, soundLzwDict[prevCode].Length);
                    pattern[soundLzwDict[prevCode].Length] = k;
                    soundLzwDict.Add(ncode);
                    prevCode = currCode;
                    c += pattern.Length;
                }
                
                var data = new Color[imgData.Length];
                for (int i = 0; i < imgData.Length; i++)
                    data[i] = palette[imgData[i]];
                
                var ret = new VideoFrame();
                ret.picture = new Texture2D(gfx.Device, width, height);
                ret.picture.SetData(data);
                ret.sound = new RawSourceWaveStream(new MemoryStream(audioData), fmt);
                return ret;
            }
        }
    }
}

