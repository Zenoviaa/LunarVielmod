using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles;

public struct DustParticleSpawnParams
{
    public DustParticleSpawnParams()
    {
        innerColor = Color.White;
        outerColor = Color.Yellow;
        scaleRange = new Vector2(0.5f, 2f);
        gravity = 0.2f;
    }
    public Color innerColor;
    public Color outerColor;
    public Vector2 scaleRange;
    public float gravity;
    public static DustParticleSpawnParams Default = new DustParticleSpawnParams();
}

public class DustParticle : Particle<DustParticle>
{
    public int FrameWidth = 64;
    public int FrameHeight = 64;
    public int MaxFrameCount = 3;
    public float gravity;
    public Color innerColor;
    public Color outerColor;
    public Vector2 stretchScale;
    public float dampening;
    public bool fast;
    public bool noTileCollide;
    public bool superFast;

    public static DustParticle Spawn(Vector2 position, Vector2 velocity, DustParticleSpawnParams? spawnParams = null)
    {
        if (!spawnParams.HasValue)
            spawnParams = new DustParticleSpawnParams();
        DustParticleSpawnParams settings = spawnParams.Value;
        float scale = Main.rand.NextFloat(settings.scaleRange.X, settings.scaleRange.Y);
        DustParticle dp = Spawn(position, velocity, Color.White, scale);
        dp.innerColor = settings.innerColor;
        dp.outerColor = settings.outerColor;
        dp.gravity = settings.gravity;
        return dp;
    }

    public override void OnSpawn()
    {
        gravity = 0.2f;
        innerColor = Color.White;
        outerColor = Color.Red;
        Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
        customShader = DustShader.Instance;
        superFast = false;
        fast = false;
        noTileCollide = false;
        dampening = 0f;
    }

    public override void Update()
    {
        Velocity.Y += gravity;
        Velocity *= 1.0f - dampening;
        Rotation = Velocity.ToRotation();
        Scale *= 0.97f;
        if (fast)
            Scale *= 0.98f;
        if (superFast)
        {
            Velocity *= 0.9f;
            Scale *= 0.94f;
        }
        color *= 0.99f;

        float stretchInterp = Velocity.Length() / 5f;
        stretchScale.X = MathHelper.Lerp(1f, 2f, stretchInterp);
        stretchScale.Y = 1f;
        fadeIn++;
        if (fadeIn > 180 || Scale < 0.1f)
            active = false;
        Lighting.AddLight(Center, color.ToVector3());
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
        DustShader shader = DustShader.Instance;
        shader.InnerColor = innerColor;
        shader.OuterColor = outerColor;
        shader.Apply();

        var textureAsset = GetTexture();
        spriteBatch.Draw(textureAsset.Value, centerPos, Frame, Color.White, Rotation, Frame.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
    }
}
