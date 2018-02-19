using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.Cutscenes;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Interfaces;

namespace Plex.Engine
{
    public class VideoPlayer : IEntity
    {
        const int flicksPerMs = 705600;
        int timer = 0, i = 0;
        IVideoFormat vid;
        VideoFrame? frame;
        bool fin = false;
        public event EventHandler Finished;
        int frames = 1;

        public void Update(GameTime time)
        {
            if (fin)
                return;
            timer += time.ElapsedGameTime.Milliseconds * flicksPerMs;
            frames += Math.DivRem(timer, vid.FlicksPerFrame, out timer);
            if (frames > 0)
            {
                if (i + frames > vid.Length)
                {
                    Finished?.Invoke(this, new EventArgs());
                    fin = true;
                    return;
                }
            }
        }

        public void Draw(GameTime time, GraphicsContext gfx)
        {
            if (!fin) while (frames > 0)
                {
                    frame = vid.NextFrame(gfx);
                    i++;
                    frames--;
                }
            gfx.BeginDraw();
            gfx.DrawRectangle(0, 0, gfx.Width, gfx.Height, ((VideoFrame)frame).picture, System.Windows.Forms.ImageLayout.Zoom);
            gfx.EndDraw();
        }

        public void OnKeyEvent(KeyboardEventArgs e)
        {
        }

        public void OnMouseUpdate(MouseState mouse)
        {
        }

        public VideoPlayer(IVideoFormat vid)
        {
            this.vid = vid;
        }
    }
}
