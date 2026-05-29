using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Effects.RoyalMagic;

public class RoyalMagicSwordParticle :
    Particle<RoyalMagicSwordParticle>
{
    private float _flickerOffset;
    public const int FRAME_WIDTH = 128;
    public const int FRAME_HEIGHT = 128;
    public override void OnSpawn()
    {
        _flickerOffset = Main.rand.NextFloat(0f, 10f);
        Frame = new Rectangle(0, FRAME_HEIGHT * Main.rand.Next(3), FRAME_WIDTH, FRAME_HEIGHT);
    }

    public override void Update()
    {
        Velocity *= 0.94f;
        Rotation = Velocity.ToRotation();
        Scale *= 0.999f;
        fadeIn++;
        if (fadeIn >= 45)
            active = false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        var textureAsset = GetTexture();

        Color starColor = color;
        starColor = Color.Lerp(starColor, Color.Black, fadeIn / 45f);
        Vector2 scale = Vector2.One * Scale;
        scale.X *= Velocity.Length() / 3f;
        scale.X = MathHelper.Clamp(scale.X, 0f, 2f);
        scale.Y *= 0.5f;
       // starColor *= ExtraMath.Osc(0.4f, 1f, speed: 3, _flickerOffset);
        spriteBatch.Draw(textureAsset.Value, DrawPosition, Frame, starColor, Rotation, Frame.Size() / 2f, scale, SpriteEffects.None, 0);
    }
}
