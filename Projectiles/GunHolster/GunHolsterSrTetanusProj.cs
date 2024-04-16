using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Stellamod.Projectiles.GunHolster
{
    internal class GunHolsterSrTetanusProj : GunHolsterProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Make sure this is the width/height of the texture or it won't draw correctly
            Projectile.width = 48;
            Projectile.height = 32;

            //This number is in ticks
            AttackSpeed = 20;
            ShootCount = 2;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);

            //Right handed gun
            IsRightHand = true;
        }

        protected override void Shoot(Vector2 position, Vector2 direction)
        {
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 14; k++)
            {
                Vector2 newDirection = direction.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.DarkGreen, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkGreen, 1);
            Vector2 velocity = direction * 16;
            velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 15);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity,
                ModContent.ProjectileType<SrTetanusProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), Projectile.position);
        }
    }
}
