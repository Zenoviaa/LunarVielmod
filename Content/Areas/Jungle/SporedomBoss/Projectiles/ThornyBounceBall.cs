using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;

public class ThornyBounceBall : ModProjectile
{
    private float XSpeed => 4;
    private float Tracking => 0.03f;
    private float Gravity => 0.3f;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.timeLeft = 240;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        base.AI();
        var nearestPlayer = PlayerHelper.FindClosestPlayer(Projectile.Center, 1024);
        if (nearestPlayer != null)
        {
            var direction = nearestPlayer.Center.X < Projectile.Center.X ? -1 : 1;
            var targetX = direction * XSpeed;
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, targetX, Tracking);
        }
        Projectile.rotation += Projectile.velocity.X * 0.05f;
        Projectile.velocity.Y += Gravity;
    }

    private void BounceEffects()
    {
        for (float f = 0; f < 5f; f++)
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(16, 16);
            Vector2 velocity = Main.rand.NextVector2Circular(16, 3);
            var p = Particle<ThickSmokeParticle>.Spawn(pos, velocity, Color.DarkGray);
            p.Scale *= 0.25f;
            p.color = Color.Orange * 0.4f;
        }

        var p3 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitY);
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (ProjectileHelper.Bounce(Projectile, oldVelocity))
        {
            BounceEffects();
        }
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (float f = 0; f < 7f; f++)
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(16, 16);
            Vector2 velocity = Main.rand.NextVector2Circular(16, 3);
            var p = Particle<ThickSmokeParticle>.Spawn(pos, velocity, Color.DarkGray);
            p.Scale *= 0.25f;
            p.color = Color.Orange * 0.4f;
        }
    }
}
