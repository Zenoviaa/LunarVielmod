using Stellamod.Core.Pixelation;

namespace Stellamod.Common.Particles;

public interface IParticleUpdater
{
    ParticleFrameData FrameData { get; }
    DrawLayer PixelationDrawLayer { get; }
    void Update();
    void Draw(SpriteBatch spriteBatch, Vector2 screenPos);
}
