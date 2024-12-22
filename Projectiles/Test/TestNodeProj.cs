using Terraria.ModLoader;

namespace Stellamod.Projectiles.Test
{
    internal class TestNodeProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }
    }
}
