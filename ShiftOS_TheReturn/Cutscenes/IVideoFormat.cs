using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.Cutscenes
{
    public interface IVideoFormat
    {
        int Length { get; }
        int FlicksPerFrame { get; }
        
        VideoFrame NextFrame(GraphicsContext gfx);
    }
}

