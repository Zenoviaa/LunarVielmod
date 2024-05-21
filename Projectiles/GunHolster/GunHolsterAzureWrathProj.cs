using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class GunHolsterAzureWrathProj : GunHolsterProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Make sure this is the width/height of the texture or it won't draw correctly
            Projectile.width = 68;
            Projectile.height = 44;

            //Higher is faster
            AttackSpeed = 1;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 1;

            IsRightHand = true;
        }

        protected override void Shoot(Vector2 position, Vector2 direction)
        {
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = direction.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.MediumPurple, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.BlueViolet, 1);

            Player player = Main.player[Projectile.owner];
            Vector2 velocity = direction * 16;
            velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 15);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity,
            ModContent.ProjectileType<AzurewrathProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/TON618");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }
    }
}
