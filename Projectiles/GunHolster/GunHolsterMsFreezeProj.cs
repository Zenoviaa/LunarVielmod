using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class GunHolsterMsFreezeProj : GunHolsterProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Make sure this is the width/height of the texture or it won't draw correctly
            Projectile.width = 68;
            Projectile.height = 30;

            //Higher is slower
            AttackSpeed = 3;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            IsRightHand = true;

            //Recoil
            RecoilDistance = 0;
            RecoilRotation = 0;
            RecoilRotationMini = 0;
        }

        protected override void Shoot(Vector2 position, Vector2 direction)
        {
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = direction.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.DarkBlue, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Blue, 1);

            Player player = Main.player[Projectile.owner];
            player.PickAmmo(player.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId, true);

            projToShoot = Main.rand.Next(new int[] { ModContent.ProjectileType<FroBall2>(), ModContent.ProjectileType<FroBall1>() });

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, direction * 8, projToShoot, Projectile.damage, Projectile.knockBack, Projectile.owner);


            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/HarmonicBlasphemy1");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);

        }


      

    }
}
