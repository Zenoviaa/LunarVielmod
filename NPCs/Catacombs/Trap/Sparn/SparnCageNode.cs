using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Trap.Sparn
{
    internal class SparnCageNode : ModProjectile
    {
        public Projectile targetProjectile;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.light = 0.25f;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.98f;
        }

        private void DrawChainCurve(SpriteBatch spriteBatch, Vector2 projBottom, out Vector2[] chainPositions)
        {
            Texture2D chainTex = ModContent.Request<Texture2D>(Texture + "_Chain").Value;
            Curvature curve = new Curvature(new Vector2[] { targetProjectile.Center, projBottom });
            int numPoints = 30;
            chainPositions = curve.GetPoints(numPoints).ToArray();

            //Draw each chain segment, skipping the very first one, as it draws partially behind the player
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];
                float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points

                Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
                Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
                Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
                spriteBatch.Draw(chainTex, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if(targetProjectile != null && targetProjectile.active)
                DrawChainCurve(Main.spriteBatch, Projectile.Center, out Vector2[] chainPositions);

            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Gray, Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Curvature curve = new Curvature(new Vector2[] { Projectile.Center, targetProjectile.Center });
            int numPoints = 32;
            Vector2[] chainPositions = curve.GetPoints(numPoints).ToArray();
            float collisionPoint = 0;
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];
                Vector2 previousPosition = chainPositions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }

            return base.Colliding(projHitbox, targetHitbox);
        }
    }
}
