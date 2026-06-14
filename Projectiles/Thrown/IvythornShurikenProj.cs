using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown;

public class IvythornShurikenProj : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.Shuriken);
        AIType = ProjectileID.Shuriken;
        Projectile.scale = 1.2f;
    }


    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        for (int i = 0; i < 5; i++)
        {
            Dust.NewDustPerfect(base.Projectile.Center, DustID.JunglePlants, (Vector2.One).RotatedByRandom(25.0), 0, default(Color), 1f).noGravity = false;
        }
        for (int i = 0; i < 5; i++)
        {
            int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.JunglePlants, 0f, -2f, 0, default(Color), .8f);
            Main.dust[num1].noGravity = true;
            Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
            if (Main.dust[num1].position != Projectile.Center)
                Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
            int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default(Color), .8f);
            Main.dust[num].noGravity = true;
            Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
            if (Main.dust[num].position != Projectile.Center)
                Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
        }

    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }


}


