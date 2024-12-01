using Microsoft.Xna.Framework;
using Stellamod.Common.ScorpionMountSystem;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Scorpions.RoyalPalace
{
    internal class RoyalPalaceScorpionGun : BaseScorpionGun
    {
        protected override void Shoot()
        {
            base.Shoot();
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 fireVelocity = Projectile.velocity * 10f;
                fireVelocity = fireVelocity.RotatedByRandom(MathHelper.ToRadians(8));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireVelocity,
                ProjectileID.Bullet, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"), Projectile.position);
            }

            Vector2 velocity = Projectile.velocity;
            float rot = velocity.ToRotation();
            float spread = 0.4f;

            for (int k = 0; k < 7; k++)
            {
                Vector2 direction = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(Projectile.position + velocity * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(Projectile.position + velocity * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
        }
    }
}
