using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles;

public class MoonSpiralParticle : Particle<MoonSpiralParticle>
{
    public const int FRAME_WIDTH = 128;
    public const int FRAME_HEIGHT = 128;
    public const int MAX_FRAME_COUNT = 1;

    public Vector2 stretchScale;
    public float gravity;
    public float dampening;
    public bool fast;
    public override void OnSpawn()
    {
        gravity = 0.2f;
        Frame = new Rectangle(0,
            FRAME_HEIGHT * Main.rand.Next(MAX_FRAME_COUNT),
            FRAME_WIDTH,
            FRAME_HEIGHT);
        fast = false;
    }

    public override void Update()
    {
        Velocity.Y += gravity;
        Velocity *= 1.0f - dampening;
        Rotation = Velocity.ToRotation();
        Scale *= 0.97f;
        if (fast)
            Scale *= 0.98f;
        color *= 0.99f;

        float stretchInterp = Velocity.Length() / 5f;
        stretchScale.X = MathHelper.Lerp(1f, 2f, stretchInterp);
        stretchScale.Y = 1f;
        fadeIn++;
        if (fadeIn > 180 || Scale < 0.1f)
            active = false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var textureAsset = GetTexture();
        Color drawColor = color;
        drawColor.A = 0;
        spriteBatch.Draw(textureAsset.Value, DrawPosition, Frame, drawColor, Rotation + MathHelper.PiOver2, Frame.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
    }
}
public class CrescentMoonParticle : Particle<CrescentMoonParticle>
{
    public const int FRAME_WIDTH = 128;
    public const int FRAME_HEIGHT = 128;
    public const int MAX_FRAME_COUNT = 1;

    public Vector2 stretchScale;
    public float gravity;
    public float dampening;
    public bool fast;
    public override void OnSpawn()
    {
        gravity = 0.2f;
        Frame = new Rectangle(0, 
            FRAME_HEIGHT * Main.rand.Next(MAX_FRAME_COUNT),
            FRAME_WIDTH,
            FRAME_HEIGHT);
        fast = false;
    }

    public override void Update()
    {
        Velocity.Y += gravity;
        Velocity *= 1.0f - dampening;
        Rotation = Velocity.ToRotation();
        Scale *= 0.97f;
        if (fast)
            Scale *= 0.98f;
        color *= 0.99f;

        float stretchInterp = Velocity.Length() / 5f;
        stretchScale.X = MathHelper.Lerp(1f, 2f, stretchInterp);
        stretchScale.Y = 1f;
        fadeIn++;
        if (fadeIn > 180 || Scale < 0.1f)
            active = false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var textureAsset = GetTexture();
        Color drawColor = color;
        drawColor.A = 0;
        spriteBatch.Draw(textureAsset.Value, DrawPosition, Frame, drawColor, Rotation + MathHelper.PiOver2, Frame.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
    }
}