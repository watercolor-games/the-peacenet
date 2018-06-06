using System;
using Plex.Engine.GUI;
using Plex.Engine;
using System.Threading;
namespace Peacenet
{
#if DEBUG 
    [AppLauncher("Frame Watch Test", "System", "Makes sure the Frame Watch works, duh")]
    public class FrameWatchTest : Window
    {
        [Dependency]
        FrameWatch fw;

        Button start;
        Label testa;
        Label testb;

        enum Frame
        {
            Dormant,
            TestA,
            TestB,
            Apply
        }

        Frame frame;

        public FrameWatchTest(WindowSystem _winsys) : base(_winsys)
        {
            frame = Frame.Dormant;
            Width = 800;
            Height = 600;
            start = new Button { Text = "Start", Width = 170, Height = 100, X = 2, Y = 2 };
            AddChild(start);
            testa = new Label { Text = "Click", AutoSize = true, X = 2, Y = start.Y + start.Height + 2 };
            AddChild(testa);
            testb = new Label { Text = "Start", AutoSize = true, X = 2, Y = testa.Y + 16 };
            AddChild(testb);
        }

        string resulta, resultb;

        protected override void OnPaint(Microsoft.Xna.Framework.GameTime time, Plex.Engine.GraphicsSubsystem.GraphicsContext gfx)
        {
            base.OnPaint(time, gfx);
            switch (frame)
            {
                case Frame.TestA:
                    resulta = "False Negative";
                    fw.Alert(TimeSpan.FromMilliseconds(350), () => resulta = "OK");
                    Thread.Sleep(500);
                    frame = Frame.TestB;
                    break;
                case Frame.TestB:
                    resultb = "OK";
                    fw.Alert(TimeSpan.FromMilliseconds(350), () => resultb = "False Positive");
                    frame = Frame.Apply;
                    break;
                case Frame.Apply:
                    if (resulta != null)
                        testa.Text = resulta;
                    if (resultb != null)
                        testb.Text = resultb;
                    resulta = resultb = null;
                    frame = Frame.Dormant;
                    break;
            }
        }
    }
#endif
}
