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

        if (MathF.Abs(Projectile.velocity.X) > 1)
            Projectile.velocity *= 0.96f;
        Projectile.velocity.X += MathF.Sin(Timer * 0.03f) * 0.06f;
        Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, 0.1f, 0.1f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InSine(LifeRatio));
        drawer.scale *= RandScale;
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
