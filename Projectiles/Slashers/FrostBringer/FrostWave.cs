using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Projectiles.Bow;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Swords
{
    internal class FrostWave : BaseFlyingSlashProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            base.AI();
            Projectile.velocity *= 1.01f;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 12; i++)
            {
                float rot = Main.rand.NextFloat(0f, 3.14f);
                Vector2 vel = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 2f);
                Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, vel, Scale: 0.75f);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel,
                    ModContent.ProjectileType<WinterboundArrowFlake>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/WinterStorm");
            soundStyle.PitchVariance = 0.2f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }
    }
}


