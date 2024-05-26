using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Projectiles.Magic
{
    internal class MarksmanLightningProj : ModProjectile, IPixelPrimitiveDrawer
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Seed => ref Projectile.ai[1];
        private float Lifetime => 60;
        private Vector2[] LightningPos;

        internal PrimitiveTrail BeamDrawer;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = (int)Lifetime;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Timer++;
            if(Seed != 0)
            {                
                //Calculate
                List<Vector2> points = new List<Vector2>();
                Vector2 currentPoint = Projectile.Center;
                points.Add(currentPoint);

                int numPoints = 80;
                UnifiedRandom random = new UnifiedRandom((int)Seed);
                for (int i = 0; i < numPoints; i++)
                {
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    float maxRadians = MathHelper.ToRadians(30);
                    double radians = random.NextDouble() * maxRadians - random.NextDouble() * maxRadians;
                    direction = direction.RotatedByRandom(radians);
                    float distance = random.NextFloat(2, 64);
                    currentPoint = currentPoint + direction * distance;
                    points.Add(currentPoint);
                }

                LightningPos = points.ToArray();
                Seed = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Electrifying!!!! nEMIES!!!
            target.AddBuff(BuffID.Electrified, 120);
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);

            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1, 1);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed, Scale: 1.5f);
                d.noGravity = true;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 194;
            float timeLeft = Projectile.timeLeft;
            float progress = timeLeft / Lifetime;
            float easedProgress = Easing.SpikeInOutCirc(1f - progress);
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio) * easedProgress;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.DarkGoldenrod;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (LightningPos == null)
                return false;
            //This damages everything in the trail
            Vector2[] positions = LightningPos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 80, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            if (LightningPos == null || LightningPos.Length == 0)
                return;
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.LightGoldenrodYellow);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.FadedStreak);

            BeamDrawer.DrawPixelated(LightningPos, -Main.screenPosition, LightningPos.Length);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
