using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Test
{
    internal class FigureEightTestProj : ModProjectile
    {
        private float Timer;
        private Vector2 FigureEightStartCenter;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                //Starting position of the figure 8
                FigureEightStartCenter = Projectile.Center;
            }

            //Calculate the position
            float movementSpeed = 32;
            float size = 384;
            float figureEightSpeed = 0.05f;

            float t = Timer * figureEightSpeed;
            float scale = 2 / (3 - MathF.Cos(2 * t));

            scale *= size;
            float x = scale * MathF.Cos(t);
            float y = scale * MathF.Sin(2 * t) / 2;

            Vector2 targetCenter = FigureEightStartCenter + new Vector2(x, y);
            Vector2 targetVelocity = Projectile.Center.DirectionTo(targetCenter) * movementSpeed;
            float distance = Vector2.Distance(Projectile.Center, targetCenter);
            if (distance < movementSpeed)
            {
                targetVelocity = Projectile.Center.DirectionTo(targetCenter) * distance;
            }
            Projectile.velocity = targetVelocity;
        }
    }
}
