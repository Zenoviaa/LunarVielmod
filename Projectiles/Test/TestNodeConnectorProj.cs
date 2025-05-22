using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Test
{
    internal class TestNodeConnectorProj : ModProjectile
    {
        Vector2[] ConnectorPos;
        int FrameTick;
        int FrameCounter;
        
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            //Get the points to connect
            List<Vector2> points = new List<Vector2>();
            for(int i = 0; i < Main.maxProjectiles; i++)
            {
                if (!Main.projectile[i].active)
                    continue;

                if (Main.projectile[i].owner != Projectile.owner)
                    continue;

                if (Main.projectile[i].type == ModContent.ProjectileType<TestNodeProj>())
                {
                    points.Add(Main.projectile[i].Center);
                }
            }

            ConnectorPos = points.ToArray();
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 8;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Yellow;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = ConnectorPos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (ConnectorPos.Length == 0)
                return false;

            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture).Value;
            int frameCount = 8;
            int frameTime = 2;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref FrameCounter, ref FrameTick, frameTime, frameCount, true);
            DrawHelper.DrawSupernovaChains(chainTexture, ConnectorPos, animationFrame, (float)Projectile.alpha / 255f);
            return false;
        }
    }
}
