using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Projectiles.Magic;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class WaterGunConnectorProj : ModProjectile
    {
        Vector2[] TargetConnectorPos;
        Vector2[] ConnectorPos;
        List<Vector2> Connector;
        List<Vector2> Target;
        int FrameTick;
        int FrameCounter;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = int.MaxValue;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            AI_Channel();
            AI_FillPoints();
        }

        private void AI_Channel()
        {
            //Channeling
            bool isReal = false;
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            foreach(var proj in Main.ActiveProjectiles)
            {
                if(proj.type == ModContent.ProjectileType<WaterGunNodeProj>() && proj.owner == Projectile.owner)
                {
                    isReal = true;
                }
            }

            if (!isReal)
            {
                Projectile.Kill();
            }
        }

        private void AI_FillPoints()
        {
            //Get the points to connect
            List<Vector2> points = new List<Vector2>();
            List<Projectile> projectiles = new List<Projectile>();
            int nodeType = ModContent.ProjectileType<WaterGunNodeProj>();
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.owner != Projectile.owner)
                    continue;
                if (proj.type != nodeType)
                    continue;
                projectiles.Add(proj);
            }

            projectiles.Sort((x, y) => y.timeLeft.CompareTo(x.timeLeft));
            for(int i = 0; i < projectiles.Count; i++)
            {
                points.Add(projectiles[i].Center);
            }

            Connector = points;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = Connector.ToArray();
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
        internal PrimitiveTrail BeamDrawer;


        public float WidthFunction(float completionRatio)
        {
            return Projectile.width * Projectile.scale * 1.3f;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.White;
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var oldPos = Connector;
            if (oldPos.Count == 0)
                return false;

         
            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture).Value;
            int frameCount = 8;
            int frameTime = 2;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref FrameCounter, ref FrameTick, frameTime, frameCount, true);
            SpriteBatch spriteBatch = Main.spriteBatch;



            for (int i = 1; i < oldPos.Count; i++)
            {
                
                //Draw from center bottom of texture
                Vector2 frameSize = animationFrame.Size();
                Vector2 origin = new Vector2(frameSize.X / 2, frameSize.Y);

                Vector2 start = oldPos[i];
                Vector2 end = oldPos[i - 1];
                Vector2 position = start;

                float rotation = (start - end).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float distance = Vector2.Distance(start, end);
                float yScale = Vector2.Distance(oldPos[i], oldPos[i - 1]) / frameSize.Y; //Calculate how much to squash/stretch for smooth chain based on distance between points

                Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
                spriteBatch.Draw(chainTexture, position - Main.screenPosition, animationFrame,
                    Color.White, rotation, origin, scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
