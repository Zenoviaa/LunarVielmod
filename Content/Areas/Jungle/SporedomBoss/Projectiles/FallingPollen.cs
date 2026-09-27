using Stellamod.Common.Particles;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;

public class FallingPollen : ModProjectile
{
    private float Lifetime => 300;
    private float LifeRatio => Timer / Lifetime;
    private ref float Timer => ref Projectile.ai[0];
    private ref float RandScale => ref Projectile.ai[1];
    private float OutScale => EasingFunction.InOutSine(Projectile.timeLeft / 60f);
    private Color PollenLightColor => Color.Lerp(Color.Gold, Color.Black, 0.8f);
    private Color PollenDarkColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.8f);
    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        RandScale = Main.rand.NextFloat(0.6f, 1f);
    }
    
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = (int)Lifetime;
        Projectile.penetrate = -1;
        Projectile.light = 0.78f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Main.rand.NextBool(16))
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(16, 16);
            Vector2 velocity = Main.rand.NextVector2Circular(16, 3);
            var p = Particle<ThickSmokeParticle>.Spawn(pos, velocity, Color.DarkGray);
            p.Scale *= 0.125f;
            p.color = Color.Orange * 0.4f;
        }
        if(Main.rand.NextBool(8))
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                velocity = -Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1f),
                innerColor = PollenLightColor.ToVector4(),
                outerColor = PollenDarkColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1f)),
                timeLeft = 120
            });
        }

        if (Timer % 16 == 0)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0, 32, Main.rand.NextFloat(0f, 1f));
            var sp = FaintSmokeParticle.Spawn(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            sp.behindLayer = true;
            sp.fadeToColor = Color.Black;
            sp.color = PollenDarkColor;

            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.Scale *= 0.3f;
            sp.dampening = 0.05f;
        }
        if (MathF.Abs(Projectile.velocity.X) > 1)
            Projectile.velocity *= 0.96f;
        Projectile.velocity.X += MathF.Sin(Timer * 0.03f) * 0.1f;
        var target = PlayerHelper.FindClosestPlayer(Projectile.position, 1024);
        if(target != null)
        {
            var xDir = target.Center.X < Projectile.Center.X ? -1 : 1;
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, xDir * 4, 0.02f);
        }
        Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, 3 * RandScale, 0.1f);
        if (OutScale < 1f)
            Projectile.hostile = false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.scale *= RandScale * MathHelper.Lerp(1f, 0f, EasingFunction.InQuad(LifeRatio));
        drawer.scale *= OutScale;
        Main.spriteBatch.Draw(drawer);

        drawer.VerticalFrame(1, 2);
        drawer.color = Color.Gold * ExtraMath.Osc(0.5f, 1f, speed: 2, Projectile.whoAmI) * 0.3f;
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
