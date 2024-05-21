using Microsoft.Xna.Framework;
using Mono.Cecil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class GunHolsterElectrifyingProj : GunHolsterProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Make sure this is the width/height of the texture or it won't draw correctly
            Projectile.width = 60;
            Projectile.height = 32;

            //Higher is faster
            AttackSpeed = 4;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        protected override void Shoot(Vector2 position, Vector2 direction)
        {
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 4; k++)
            {
                Vector2 newDirection = direction.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.LightCyan, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.LightCyan, 1);

            Player player = Main.player[Projectile.owner];
            player.PickAmmo(player.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId, true);

            int numProjectiles = Main.rand.Next(1, 2);
            for (int p = 0; p < numProjectiles; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 velocity = direction * 8;
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(6));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), position, newVelocity, 
                    ModContent.ProjectileType<ElectrifyingProj>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
            }

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunElectric");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }
    }
}
