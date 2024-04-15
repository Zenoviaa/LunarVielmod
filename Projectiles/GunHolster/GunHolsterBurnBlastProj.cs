using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class GunHolsterBurnBlastProj : GunHolsterProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Make sure this is the width/height of the texture or it won't draw correctly
            Projectile.width = 62;
            Projectile.height = 38;

            //This number is in ticks
            AttackSpeed = 60;

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
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Orange, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, direction * 16, 
                ModContent.ProjectileType<BurnBlastProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 16f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), Projectile.position);
        }
    }
}
