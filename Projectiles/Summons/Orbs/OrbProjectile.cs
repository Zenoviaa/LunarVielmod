using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Orbs
{
    internal abstract class OrbProjectile : ModProjectile
    {
        public virtual float MaxThrowDistance { get; }

        protected Player Owner => Main.player[Projectile.owner];

        protected Vector2 GetSwingTarget()
        {
            Vector2 targetMouseWorld = Main.MouseWorld;
            float maxThrowDistance = MaxThrowDistance;
            float distance = Vector2.Distance(Owner.Center, targetMouseWorld);
            if (distance < maxThrowDistance)
                return targetMouseWorld;
            else
            {
                return Owner.Center + (Owner.Center.DirectionTo(Main.MouseWorld) * maxThrowDistance);
            }
        }
    }
}
