using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class WaterGunNodeProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private float Index
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 800;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer < 30)
            {
                Projectile.velocity.Y += 0.1f;
            }
            else
            {
                Projectile.velocity *= 0.9f;
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.1f;
            return false;
        }
    }
}
