using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    /// <summary>
    /// Base class for all projectiles in the mod that has a few extra variables and functions
    /// </summary>
    public abstract class ScarletProjectile : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
    }
}
