using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;

public class NiiviCometDash : ModProjectile,
    IDrawToRenderTarget
{
    NPC Parent => Main.npc[(int)Projectile.ai[0]];
    ref float Timer => ref Projectile.ai[1];
    public override string Texture => ModType.NoTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(64);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.timeLeft = 180;
        Projectile.tileCollide = false;
        Projectile.light = 0.4f;
        Projectile.penetrate = -1;
        Projectile.width = 256;
        Projectile.height = 16;
    }

    public override bool CanHitPlayer(Player target)
    {
        if (Timer >= 60)
            return false;
        return base.CanHitPlayer(target);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return CollisionHelper.LaserCollision(projHitbox, targetHitbox, Projectile.Center, Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.width, 128);
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer < 60)
        {
            Projectile.velocity = -Parent.velocity;
            Projectile.Center = Parent.Center;
        }
    }

    void DrawPixelatedCometTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(256, 0, ratio);
        }
        Color GetTrailColor(float ratio)
        {
            return Color.White;
        }

        var pass = AssetReferences.Effects.Niivi.NiiviCometDash.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.noiseSampler = new()
        {
            Texture = AssetReferences.Assets.Noise.StarCometNoise.Asset.Value,
            Sampler = SamplerState.LinearWrap
        };
        pass.Apply();
        gDevice.Register(0, AssetReferences.Assets.LaserTextures.TexturedLaser.Asset.Value, SamplerState.LinearWrap);
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, pass.Shader, Projectile.Size * 0.5F);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedCometTrail);
    }
}
