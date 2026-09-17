using Terraria;

namespace Stellamod.Common;

public static class ProjectileExtensions
{
    extension(Projectile projectile)
    {
        public Player PlayerOwner => Main.player[projectile.owner];
    }
}
