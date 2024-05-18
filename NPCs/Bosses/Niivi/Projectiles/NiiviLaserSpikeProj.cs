using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviLaserSpikeProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        //Visuals
        public override string Texture => TextureRegistry.EmptyTexture;
        internal PrimitiveTrail BeamDrawer;
        private List<Vector2> LaserSpikePos;

        //AI
        private float LifeTime => 180;
        private ref float Timer => ref Projectile.ai[0];
        private float HitboxTime => 90;
        private float LaserScale = 0.1f;
        private float Progress;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.scale = 1f;
            LaserSpikePos = new List<Vector2>();
        }

        public override void AI()
        {
            Timer++;
            float easeInLength = 30;
            if(Timer > HitboxTime - easeInLength)
            {
                float progress2 = (Timer - HitboxTime) / easeInLength;
                float easedProgress = Easing.InOutCubic(progress2);
                LaserScale = MathHelper.Lerp(0.1f, 1f, easedProgress);
            }

            Progress += 0.02f;
            if (Progress >= 2f)
                Progress = 2;
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver4 / 60f);

            Vector2 velocity = Projectile.velocity * Progress;
            LaserSpikePos.Clear();
            LaserSpikePos.Add(Projectile.Center);

            float numPoints = 24f;
            for(int i = 0; i < numPoints; i++)
            {
                float progress = (float)i / numPoints;
                progress *= Progress;
                Vector2 start = Projectile.Center;
                Vector2 end = Projectile.Center + velocity.RotatedBy(MathHelper.PiOver2 * progress);
                LaserSpikePos.Add(Vector2.Lerp(start, end, progress));
            }


        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer < HitboxTime || Timer > LifeTime - HitboxTime / 4)
                return false;
            //This damages everything in the trail
            float collisionPoint = 0;
            for (int i = 1; i < LaserSpikePos.Count; i++)
            {
                Vector2 position = LaserSpikePos[i];
                Vector2 previousPosition = LaserSpikePos[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 1, ref collisionPoint))
                    return true;
            }
            return false;
        }


        public float WidthFunction(float completionRatio)
        {
            float mult = 1;
            if (Projectile.timeLeft < 60)
            {
                mult = Projectile.timeLeft / (float)60;
            }
            return Projectile.width * Projectile.scale * 1.3f * mult * LaserScale * (0.5f + (1f-completionRatio));
        }

        public Color ColorFunction(float completionRatio)
        {
            return Main.DiscoColor;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.White);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.FadedStreak);

            BeamDrawer.DrawPixelated(LaserSpikePos, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
