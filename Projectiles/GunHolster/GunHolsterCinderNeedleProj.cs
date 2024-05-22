using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class GunHolsterCinderNeedleProj : GunHolsterProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Make sure this is the width/height of the texture or it won't draw correctly
            Projectile.width = 62;
            Projectile.height = 32;

            //Higher is faster
            AttackSpeed = 6;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        protected override void Shoot(Vector2 position, Vector2 direction)
        {
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = direction.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            for(int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Vector2 velocity = direction * 16;
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 15);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity,
                    ModContent.ProjectileType<CinderNeedleProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }


            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Gunsotp");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }
    }
}
