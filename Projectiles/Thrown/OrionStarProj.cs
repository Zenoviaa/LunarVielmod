using Microsoft.Xna.Framework;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    internal class OrionStarProj : ModProjectile
    {
        private float _sparkleSize = 0.5f;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            _sparkleSize += 0.02f;

            Timer++;
            if (Timer % 4 == 0)
            {

            }

            if (Timer >= 30)
            {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<OrionStarBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 3);
                Projectile.Kill();
                Timer = 0;
            }
        }
    }
}
