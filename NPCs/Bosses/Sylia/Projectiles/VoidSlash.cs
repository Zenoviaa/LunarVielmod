using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia.Projectiles
{

    public class VoidSlash : ModProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 2;
        }
    }
}
