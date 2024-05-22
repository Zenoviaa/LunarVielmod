using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class GunHolsterBubbleBussyProj : GunHolsterProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Make sure this is the width/height of the texture or it won't draw correctly
            Projectile.width = 62;
            Projectile.height = 34;

            //This number is in ticks
            AttackSpeed = 8;

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
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.DarkBlue, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkBlue, 1);
            Vector2 velocity = direction * 12;
            velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 15);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity,
                ModContent.ProjectileType<BubbleBussyProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/TentacleBubbleOut");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }
    }
}
