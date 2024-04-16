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
    internal class GunHolsterTheReavingProj : GunHolsterProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Make sure this is the width/height of the texture or it won't draw correctly
            Projectile.width = 66;
            Projectile.height = 40;

            //This number is in ticks
            AttackSpeed = 120;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        protected override void Shoot(Vector2 position, Vector2 direction)
        {

            Player player = Main.player[Projectile.owner];
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 14; k++)
            {
                Vector2 newDirection = direction.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.LightGoldenrodYellow, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            player.PickAmmo(player.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId, true);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, direction * 8, projToShoot, Projectile.damage, Projectile.knockBack, Projectile.owner);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 16f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), Projectile.position);


            float rot = direction.ToRotation();
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"));

            Vector2 offset = new Vector2(2, -0.1f * player.direction).RotatedBy(rot);
            for (int k = 0; k < 15; k++)
            {
                Vector2 direction2 = offset.RotatedByRandom(spread);

                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction2 * Main.rand.NextFloat(8), 125, new Color(150, 80, 40), Main.rand.NextFloat(0.2f, 0.5f));
            }


            int numProjectiles = Main.rand.Next(10, 30);
            for (int p = 0; p < numProjectiles; p++)
            {


                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
                Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));



                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = direction.RotatedByRandom(MathHelper.ToRadians(25));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);

  
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, newVelocity * 12, projToShoot, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

        }
    }
}
