using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Visual.Particles;

public class BigCrackParticle : Particle<BigCrackParticle>
{
    public int FrameWidth = 256;
    public int FrameHeight = 256;
    public int MaxFrameCount = 1;
    public override int GetPoolSize()
    {
        return 3;
    }

    public override void OnSpawn()
    {
        color = Color.White;
        Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
    }

    public override void Update()
    {
       
        fadeIn++;
        if (fadeIn > 180 || Scale < 0.1f)
            active = false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 centerPos = DrawPosition;
        var textureAsset = GetTexture();

        Color color2 = Color.Lerp(color, Color.Black, EasingFunction.InExpo(fadeIn / 180f));
        spriteBatch.Draw(textureAsset.Value, centerPos, Frame, color2, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
    }
}