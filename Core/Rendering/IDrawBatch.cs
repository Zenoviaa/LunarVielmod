using Stellamod.Core.Pixelation;

namespace Stellamod.Core.Rendering;

public interface IDrawBatch
{
    public DrawLayer DrawLayer { get; set; }
    bool NeedsFlushing();
    void Flush(GraphicsDevice gDevice);
}
