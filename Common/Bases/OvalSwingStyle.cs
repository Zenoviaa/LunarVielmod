using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace Stellamod.Common.Bases
{
    public class OvalSwingStyle : BaseSwingStyle
    {

        public OvalSwingStyle() : base()
        {

        }

        public float swingXRadius;
        public float swingYRadius;
        public float swingRange;
        public float ovalRotOffset;

        public override void AI()
        {
            float lerpValue = SwingProjectile.Timer / SwingProjectile.GetSwingTime(swingTime);
            float swingProgress = lerpValue;
            float targetRotation = Projectile.velocity.ToRotation();

            SwingProjectile.uneasedLerpValue = lerpValue;
            swingProgress = easingFunc(swingProgress);
            SwingProjectile._smoothedLerpValue = swingProgress;
            PlaySwingSound(swingProgress);

            int dir2 = (int)Projectile.ai[1];
            float xOffset;
            float yOffset;
            if (dir2 == -1)
            {
                xOffset = swingXRadius * MathF.Sin(swingProgress * swingRange + swingRange + ovalRotOffset);
                yOffset = swingYRadius * MathF.Cos(swingProgress * swingRange + swingRange + ovalRotOffset);
            }
            else
            {
                xOffset = swingXRadius * MathF.Sin((1f - swingProgress) * swingRange + swingRange + ovalRotOffset);
                yOffset = swingYRadius * MathF.Cos((1f - swingProgress) * swingRange + swingRange + ovalRotOffset);
            }

            Projectile.Center = Owner.Center + new Vector2(xOffset, yOffset).RotatedBy(targetRotation);
            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver4;

            Vector2[] points = new Vector2[ProjectileID.Sets.TrailCacheLength[Projectile.type]];
            for (int i = 0; i < points.Length; i++)
            {
                float l = points.Length;
                //Lerp between the points
                float progressOnTrail = i / l;

                //Calculate starting lerp value
                float startTrailLerpValue = MathHelper.Clamp(lerpValue - SwingProjectile.trailStartOffset, 0, 1);
                float startTrailProgress = startTrailLerpValue;
                startTrailProgress = easingFunc(startTrailLerpValue);


                //Calculate ending lerp value
                float endTrailLerpValue = lerpValue;
                float endTrailProgress = endTrailLerpValue;
                endTrailProgress = easingFunc(endTrailLerpValue);

                //Lerp in between points
                float smoothedTrailProgress = MathHelper.Lerp(startTrailProgress, endTrailProgress, progressOnTrail);
                float xOffset2;
                float yOffset2;
                if (dir2 == -1)
                {
                    xOffset2 = swingXRadius * MathF.Sin(smoothedTrailProgress * swingRange + swingRange + ovalRotOffset);
                    yOffset2 = swingYRadius * MathF.Cos(smoothedTrailProgress * swingRange + swingRange + ovalRotOffset);
                }
                else
                {
                    xOffset2 = swingXRadius * MathF.Sin((1f - smoothedTrailProgress) * swingRange + swingRange + ovalRotOffset);
                    yOffset2 = swingYRadius * MathF.Cos((1f - smoothedTrailProgress) * swingRange + swingRange + ovalRotOffset);
                }


                Vector2 pos = Owner.Center + new Vector2(xOffset2, yOffset2).RotatedBy(targetRotation);
                points[i] = pos - (SwingProjectile.GetFramingSize() / 2);// + GetTrailOffset().RotatedBy(targetRotation);
            }
            SwingProjectile._trailPoints = points;
        }
    }
}
