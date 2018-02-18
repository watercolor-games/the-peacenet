using System;
using Microsoft.Xna.Framework;
using Plex.Engine.Cutscenes;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class VideoPlayer : Control
    {
        const int flicksPerMs = 705600;
        int timer = 0, i = 0;
        IVideoFormat vid;
        VideoFrame? frame;
        bool fin = false;
        public event EventHandler Finished;
        int frames = 1;
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            if (!fin) while (frames > 0)
            {
                frame = vid.NextFrame(gfx);
                i++;
                frames--;
            }
            gfx.DrawRectangle(0, 0, Width, Height, ((VideoFrame)frame).picture, System.Windows.Forms.ImageLayout.Zoom);
        }
        protected override void OnUpdate(GameTime time)
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
                Invalidate(true);
            }
        }
        public VideoPlayer(IVideoFormat vid)
        {
            this.vid = vid;
        }
    }
}
