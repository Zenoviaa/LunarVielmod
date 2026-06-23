using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles;

public class IllurianSnowflakeParticle : Particle<IllurianSnowflakeParticle>
{
    public const int FRAME_WIDTH = 256;
    public const int FRAME_HEIGHT = 256;
    public const int MAX_FRAME_COUNT = 1;

    public float dampening;
    public override void OnSpawn()
    {
        Frame = new Rectangle(0, 128 * Main.rand.Next(MAX_FRAME_COUNT), 128, 128);
        dampening = 0.05f;
        // throw new NotImplementedException();
    }

    public override void Update()
    {
        Velocity *= 1.0f - dampening;
        Rotation = Velocity.ToRotation();
        Scale *= 0.97f;
        // throw new NotImplementedException();
        fadeIn++;
        if (fadeIn > 180 || Scale < 0.1f)
            active = false;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        var textureAsset = GetTexture();
        spriteBatch.Draw(textureAsset.Value, DrawPosition, Frame, color, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
    }
}
