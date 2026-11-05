using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Terraria;
using Terraria.ID;
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
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4800;
        Projectile.SetTrailCacheLength(64);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.timeLeft = 480;
        Projectile.tileCollide = false;
        Projectile.light = 0.4f;
        Projectile.penetrate = -1;
        Projectile.width = 256;
        Projectile.height = 16;
    }

    public override bool CanHitPlayer(Player target)
    {
        if (Timer >= 170)
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

        if (Timer < 180)
        {
            Projectile.velocity = -Parent.velocity;
            Projectile.Center = Parent.Center + Parent.velocity.SafeNormalize(Vector2.Zero) * 128;
        }
    }

    void DrawPixelatedCometTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(256, 128, ratio)  * MathHelper.Lerp(1f, 0f, EasingFunction.InQuad(Timer / 240F));
        }
        Color GetTrailColor(float ratio)
        {
            return  Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(ratio)) with { A = 0 };
        }

        var pass = AssetReferences.Effects.Niivi.NiiviCometDash.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.noiseSampler = new()
        {
            Texture = AssetReferences.Assets.Noise.StarCometNoise.Asset.Value,
            Sampler = SamplerState.LinearWrap
        };
        pass.Parameters.spriteSampler = new()
        {
            Texture = AssetReferences.Assets.LaserTextures.TexturedLaser.Asset.Value,
            Sampler = SamplerState.LinearWrap
        };
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Apply();

        TrailDrawer.ClearPrimitives();
        TrailDrawer.PreparePrimitives(Projectile.oldPos, GetTrailColor, GetTrailWidth, Projectile.Size * 0.5F);
        TrailDrawer.DrawCached(pass.Shader);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedCometTrail, DrawLayer.OverNPCs);
        return false;
    }

    public void DrawToRenderTargets()
    {
      //  
    }
}
