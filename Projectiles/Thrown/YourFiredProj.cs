using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown;

public class YourFiredProj : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 38;
        Projectile.height = 40;
        Projectile.friendly = true;
        Projectile.timeLeft = 300;
    }

    public override void AI()
    {
        Timer++;
        if (Timer == 1)
        {
            //Effects
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
        }


        Projectile.velocity.Y += 0.3f;
        Projectile.rotation = Projectile.velocity.ToRotation();
        // And create bright light.
        Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 0.78f * MathF.Sin(Timer * 0.5f));
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekFireballDeath"), Projectile.position);
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"), Projectile.position);
        float num = 8;
        float maxDelay = 30;
        for (int i = 0; i < num; i++)
        {
            float clusterRadius = 256;
            float progress = i / (float)num;
            float delay = progress * maxDelay;
            Vector2 randPosition = Projectile.Center + Main.rand.NextVector2Circular(clusterRadius, clusterRadius);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), randPosition, Vector2.Zero,
                ModContent.ProjectileType<YourFiredExplosionProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: delay);
        }
    }
}
