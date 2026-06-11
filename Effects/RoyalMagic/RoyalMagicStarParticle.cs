using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Effects.RoyalMagic;

public class RoyalMagicStarParticle : 
    Particle<RoyalMagicStarParticle>
{
    private float _flickerOffset;
    public const int FRAME_WIDTH = 128;
    public const int FRAME_HEIGHT = 128;
    public float rotationRadians;
    public override void OnSpawn()
    {
        _flickerOffset = Main.rand.NextFloat(0f, 10f);
        rotationRadians = Main.rand.NextFloat(-0.05f, 0.05f);
        Frame = new Rectangle(0, FRAME_HEIGHT * Main.rand.Next(4), FRAME_WIDTH, FRAME_HEIGHT);
    }

    public override void Update()
    {
        Velocity *= 0.95f;
        Velocity = Velocity.RotatedBy(rotationRadians);
        Scale *= 0.999f;
        fadeIn++;
        if (fadeIn >= 90)
            active = false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        var textureAsset = GetTexture();

        Color starColor = color * 2;
        starColor = Color.Lerp(starColor, Color.Black, ExtraMath.Osc(0f, 1f, speed: 12, _flickerOffset));
        starColor = Color.Lerp(starColor, Color.Black, fadeIn / 90f);
        spriteBatch.Draw(textureAsset.Value, DrawPosition, Frame, starColor, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);

        Rectangle glowFrame = new Rectangle(0, FRAME_HEIGHT * 4, FRAME_WIDTH, FRAME_HEIGHT);
        Color glowColor = Color.Lerp(Color.SkyBlue, Color.Black, 0.9f);
        glowColor = Color.Lerp(glowColor, Color.Black, fadeIn / 90f);
        spriteBatch.Draw(textureAsset.Value, DrawPosition, glowFrame, glowColor, Rotation, Frame.Size() / 2f, Scale * 1.15f, SpriteEffects.None, 0);
    }
}
