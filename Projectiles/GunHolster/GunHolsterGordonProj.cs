using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class GunHolsterGordonRightProj : GunHolsterProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Make sure this is the width/height of the texture or it won't draw correctly
            Projectile.width = 68;
            Projectile.height = 44;

            //Higher is faster
            AttackSpeed = 10;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);
          //  ShootCount = 1;
            //Recoil
            RecoilDistance = 3;

            IsRightHand = true;
        }

        protected override void Shoot(Vector2 position, Vector2 direction)
        {
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = direction.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Turquoise, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Turquoise, 1);

            Player player = Main.player[Projectile.owner];
            player.PickAmmo(player.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId, true);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, direction * 8, ModContent.ProjectileType<NLUX>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunBlasting");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle);
        }
    }
    internal class GunHolsterGordonLeftProj : GunHolsterProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Make sure this is the width/height of the texture or it won't draw correctly
            Projectile.width = 68;
            Projectile.height = 44;

            //Higher is slower
            AttackSpeed = 10;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);
            //  ShootCount = 1;
            //Recoil
            RecoilDistance = 3;

            IsRightHand = false;
        }

        protected override void Shoot(Vector2 position, Vector2 direction)
        {
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = direction.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Turquoise, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Turquoise, 1);

            Player player = Main.player[Projectile.owner];
            player.PickAmmo(player.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId, true);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, direction * 8, ModContent.ProjectileType<NLUX>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunBlasting");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle);
        }
    }


}
