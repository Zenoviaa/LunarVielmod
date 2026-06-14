using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Visual;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public static class EffectsHelper
    {
        public struct Nothin
        {

        }
        public static Nothin SimpleExplosionCircle(Projectile baseProjectile, Color explosionColor, float startRadius = 4, float endRadius = 64, float width = 24)
        {

            return new Nothin();
        }
    }
}
