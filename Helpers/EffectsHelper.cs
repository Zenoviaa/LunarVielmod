using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    internal static class EffectsHelper
    {
        public static IgniterExplosionCircle SimpleExplosionCircle( Projectile baseProjectile, Color explosionColor, float startRadius = 4, float endRadius = 64, float width = 24)
        {
            Projectile p = Projectile.NewProjectileDirect(baseProjectile.GetSource_FromThis(), baseProjectile.Center, Vector2.Zero,
                ModContent.ProjectileType<IgniterExplosionCircle>(), 0, 0, baseProjectile.owner);
            IgniterExplosionCircle circle = p.ModProjectile as IgniterExplosionCircle;
            circle.DrawColor = explosionColor;
            circle.StartRadius = startRadius;
            circle.EndRadius = endRadius;
            circle.Width = width;
            p.netUpdate = true;
            return circle;
        }
    }
}
