using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles;


public class ElectricZapParticle : Particle<ElectricZapParticle>
{
    public int FrameWidth = 422;
    public int FrameHeight = 127;
    public int MaxFrameCount = 1;
    public float gravity;
    public Color innerColor;
    public Color outerColor;
    public Vector2 stretchScale;
    public float dampening;
    public override void OnSpawn()
    {
        gravity = 0;
        innerColor = Color.White;
        outerColor = Color.DarkBlue;
        Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
        customShader = DustShader.Instance;
    }

    public override void Update()
    {
        Velocity.Y += gravity;
        Velocity *= 1.0f - dampening;
        Rotation = Velocity.ToRotation();
        Scale *= 0.97f;
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
        Vector2 centerPos = DrawPosition;
        DustShader shader = DustShader.Instance;
        shader.InnerColor = innerColor;
        shader.OuterColor = outerColor;
        shader.Apply();

        var textureAsset = GetTexture();
        spriteBatch.Draw(textureAsset.Value, centerPos, Frame, Color.White, Rotation, Frame.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
    }
}