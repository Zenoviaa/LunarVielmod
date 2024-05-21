using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class GunHolsterMintyBlastProj : GunHolsterProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Make sure this is the width/height of the texture or it won't draw correctly
            Projectile.width = 62;
            Projectile.height = 30;

            //This number is in ticks
            AttackSpeed = 2;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);

            //Right handed gun
            IsRightHand = false;
        }

        protected override void Shoot(Vector2 position, Vector2 direction)
        {
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 4; k++)
            {
                Vector2 newDirection = direction.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.LightCyan, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.White, 1);
            for(int i = 0; i < Main.rand.Next(2, 5); i++)
            {
                Vector2 velocity = direction * 16;
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity,
                    ModContent.ProjectileType<MintyBlastProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 2f);

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunLaser");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }
    }
}
