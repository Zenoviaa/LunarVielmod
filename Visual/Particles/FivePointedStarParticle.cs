using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles;

public class FivePointedStarParticle : Particle<FivePointedStarParticle>
{
    public int FrameWidth = 128;
    public int FrameHeight = 128;
    public int MaxFrameCount = 1;
    public float gravity;
    public float dampening;

    public override void OnSpawn()
    {
        gravity = 0.2f;
        Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
        gravity = 0;
    }

    public override void Update()
    {
        Velocity.Y += gravity;
        Velocity *= 1.0f - dampening;
        Rotation = Velocity.ToRotation();
        Scale *= 0.97f;
        color *= 0.99f;

        fadeIn++;
        if (fadeIn > 180 || Scale < 0.1f)
            active = false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 centerPos = DrawPosition;
        /*
        DustShader shader = DustShader.Instance;
        shader.InnerColor = innerColor;
        shader.OuterColor = outerColor;
        shader.Apply();
        */
        var textureAsset = GetTexture();
        spriteBatch.Draw(textureAsset.Value, centerPos, Frame, color, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
    }
}
