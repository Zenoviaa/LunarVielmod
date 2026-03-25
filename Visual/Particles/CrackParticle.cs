using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles;

public class CrackParticle : Particle<CrackParticle>
{
    public int FrameWidth = 128;
    public int FrameHeight = 128;
    public int MaxFrameCount = 1;
    public float gravity;
    public Vector2 stretchScale;
    public float dampening;
    public bool fast;
    public bool noTileCollide;
    public float offsetRot;

    public override void OnSpawn()
    {
        gravity = 0f;
        Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
        customShader = null;
    }

    public override void Update()
    {
        Velocity.Y += gravity;
        Velocity *= 1.0f - dampening;
        Rotation = Velocity.ToRotation() + fadeIn * 0.05f + offsetRot;
        Scale *= 0.97f;
        color *= 0.88f;

        float stretchInterp = Velocity.Length() / 5f;
        stretchScale.X = MathHelper.Lerp(1f, 2f, stretchInterp);
        stretchScale.Y = 1f;
        fadeIn++;
        if (fadeIn > 180 || Scale < 0.1f)
            active = false;

        //Bouncing
        if (noTileCollide)
            return;

        Vector2 collisionVelocity = Collision.TileCollision(Center, Velocity, 2, 2);
        if (Velocity.X != collisionVelocity.X)
            Velocity.X = -collisionVelocity.X;
        if (Velocity.Y != collisionVelocity.Y)
            Velocity.Y = -collisionVelocity.Y;

    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 centerPos = DrawPosition;
        var textureAsset = GetTexture();

        Color color2 = color;
        color2.A = 0;
        spriteBatch.Draw(textureAsset.Value, centerPos, Frame, color2, Rotation, Frame.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
    }
}