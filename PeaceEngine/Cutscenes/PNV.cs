using System;
using Plex.Engine.GraphicsSubsystem;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Plex.Engine.Cutscenes
{
    public class PNV : IVideoFormat, IDisposable
    {
        private Texture2D _frameTexture = null;
        Stream fobj;
        BinaryReader read;
        Color[] frame;
        public PNV(Stream fobj)
        {
            this.fobj = new GZipStream(fobj, CompressionMode.Decompress, true);
            this.read = new BinaryReader(this.fobj, Encoding.UTF8, true);
            if (read.ReadUInt32() != 0x56654E50)
                throw new InvalidDataException("This is not a PNV file.");
            Length = read.ReadInt32();
            FlicksPerFrame = read.ReadInt32();
            w = read.ReadInt32();
            h = read.ReadInt32();
            frame = new Color[w * h];
        }

        public int Length { get; private set; }
        public int FlicksPerFrame { get; private set; }

        int w, h;

        public void Dispose()
        {
            read?.Dispose();
            fobj?.Dispose();
            frame = null;
            _frameTexture?.Dispose();
        }

        public VideoFrame NextFrame(GraphicsContext gfx)
        {
            if (_frameTexture == null)
                _frameTexture = new Texture2D(gfx.Device, w, h);
            VideoFrame ret;
            int p = 0;
            while (p < frame.Length)
            {
                uint inst = read.ReadUInt32();
                uint l = inst >> 24;
                for (uint i = 0; i < l; i++)
                {
                    frame[p].R ^= (byte)inst;
                    frame[p].G ^= (byte)(inst >> 8);
                    frame[p].B ^= (byte)(inst >> 16);
                    p++;
                }
            }
            ret.sound = null;
            ret.picture = _frameTexture;
            ret.picture.SetData(frame);
            return ret;
        }
    }
}
