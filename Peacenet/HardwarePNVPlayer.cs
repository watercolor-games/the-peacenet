using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Plex.Engine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO.Compression;

namespace Peacenet
{
    public class HardwarePNVPlayer : IEntity, ILoadable
    {
        private Effect _shader = null;
        private Stream _stream = null;
        private BinaryReader _reader = null;
        private int _fwidth = 0;
        private int _fheight = 0;
        private int _frames = 0;
        private int _i = 0;
        private int _count = 0;
        private int _flicksPerFrame = 0;
        private bool _finished = false;
        private int _timer = 0;
        private const int _flicksPerMs = 705600;

        public event EventHandler Finished;

        public HardwarePNVPlayer(Stream stream)
        {
            _stream = new GZipStream(stream, CompressionMode.Decompress, true);
            _reader = new BinaryReader(this._stream, Encoding.UTF8, true);
            if (_reader.ReadUInt32() != 0x56654E50)
                throw new InvalidDataException("This is not a PNV file.");
            _count = _reader.ReadInt32();
            _flicksPerFrame = _reader.ReadInt32();
            _fwidth = _reader.ReadInt32();
            _fheight = _reader.ReadInt32();
        }

        private void _updateShader()
        {
            List<float> _counts = new List<float>();
            List<Vector4> _colors = new List<Vector4>();

            int p = 0;
            while (p < _fwidth*_fheight)
            {
                uint inst = _reader.ReadUInt32();
                uint l = inst >> 24;
                _counts.Add(l);
                _colors.Add(new Vector4((byte)inst, (byte)(inst >> 8), (byte)(inst >> 16), 1.0f));
                p += (int)l;
            }
            _shader.Parameters["PixelCounts"].SetValue(_counts.ToArray());
            _shader.Parameters["CurrentFrame"].SetValue(_colors.ToArray());
        }

        public void Update(GameTime time)
        {
            if (_finished)
                return;
            _timer += time.ElapsedGameTime.Milliseconds * _flicksPerMs;
            _frames += Math.DivRem(_timer, _flicksPerFrame, out _timer);
            if (_frames > 0)
            {
                if (_i + _frames > _count)
                {
                    Finished?.Invoke(this, new EventArgs());
                    _finished = true;
                    return;
                }
            }

        }

        public void Draw(GameTime time, GraphicsContext gfx)
        {
            if (_finished)
                return;
            while (_frames > 0)
            {
                _updateShader();
                _i++;
                _frames--;
            }
            gfx.BeginDraw(_shader);
            gfx.DrawRectangle(0, 0, gfx.Width, gfx.Height, Color.Black);
            gfx.EndDraw();
        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
        }

        public void OnMouseUpdate(MouseState mouse)
        {
        }

        public void OnGameExit()
        {
        }

        public void Load(ContentManager content)
        {
            _shader = content.Load<Effect>("Shaders/PNVDecoder");
            _shader.Parameters["FrameWidth"].SetValue(_fwidth);
            _shader.Parameters["FrameHeight"].SetValue(_fheight);
            _shader.Parameters["PreviousFrame"].SetValue(new Vector4[_fwidth * _fheight]);
        }
    }
}
