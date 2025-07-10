using Microsoft.Xna.Framework;
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
        public Vector2[] OldCenterPos;
        public int TrailCacheLength;
        public override void AI()
        {
            base.AI();
            if (OldCenterPos == null && TrailCacheLength > 0)
                OldCenterPos = new Vector2[TrailCacheLength];
            if(OldCenterPos != null)
            {
                for(int i = TrailCacheLength - 1; i > 0; i--)
                {
                    OldCenterPos[i] = OldCenterPos[i - 1];
                }
                OldCenterPos[0] = Projectile.Center;
            }
        }
    }
}
