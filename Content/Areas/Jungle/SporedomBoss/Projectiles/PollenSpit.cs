using Stellamod.Common.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;
public class PollenSpit : ModProjectile,
    IDrawToRenderTarget
{
    private float XSlowing => 0.99f;
    private float MaxFallSpeed => 15;
    private float Gravity => 0.3f;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 4;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.velocity.X *= XSlowing;
        if (Projectile.velocity.Y < MaxFallSpeed)
            Projectile.velocity.Y += Gravity;
        if (Timer % 16 == 0)
        {
            var offset = Main.rand.NextVector2Circular(16, 16);
            var pos = Projectile.Center + offset;
            var vel = Main.rand.NextVector2Circular(1, 1);
            Dust.NewDustPerfect(pos, DustID.GemTopaz, vel, Scale: Main.rand.NextFloat(0.2f, 0.6f));
        }

        if (Timer % 8 == 0)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0, 384f, Main.rand.NextFloat(0f, 1f));
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            sp.behindLayer = true;
            sp.fadeToColor = Color.Black;
            sp.color = Color.Lerp(Color.Orange, Color.Gold, Main.rand.NextFloat(0f, 1f));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (var f = 0; f < 4; f++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                velocity = -Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1f),
                innerColor = Color.Gold.ToVector4(),
                outerColor = Color.DarkOrange.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1f)),
                timeLeft = 120
            });
        }

        for (var f = 0; f < 8; f++)
        {
            DustParticle.Spawn(Projectile.Center,
                -Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1f),
                DustParticleSpawnParams.Default with
                {
                    innerColor = Color.Gold,
                    outerColor = Color.DarkOrange,
                    gravity = 0.2f,
                    scaleRange = new Vector2(0.3f, 0.7f)
                });
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    private void PollenDraw(SpriteBatch spriteBatch)
    {
        var drawer = Projectile.Drawer;
        var startScale = new Vector2(1.1f, 0.9f);
        var endScale = new Vector2(0.9f, 1.1f);
        var scale = Vector2.One;
        scale.X = MathHelper.Lerp(startScale.X, endScale.X, ExtraMath.Osc(0f, 1f, speed: 2));
        scale.Y = MathHelper.Lerp(startScale.Y, endScale.Y, ExtraMath.Osc(0f, 1f, speed: 2, offset: 3.14f));
        drawer.scale *= scale;
        spriteBatch.Draw(drawer);
    }

    public void DrawToRenderTargets()
    {
        PollenSpitRenderer.DrawActionQueue.Enqueue(PollenDraw);
        OutlineRenderer.Queue(PollenDraw);
    }
}
