using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace Stellamod.Common.Bases
{
    public class SpearSwingStyle : BaseSwingStyle
    {
        private bool _thrust;
        public float thrustSpeed;
        public float stabRange;
        public float spinRotationRange;
        public Func<float, float> spinRotationEasingFunc;
        public float spinTrailOffset;
        public override void AI()
        {
            float lerpValue = SwingProjectile.Timer / SwingProjectile.GetSwingTime(swingTime);

            float swingProgress = lerpValue;
            float targetRotation = Projectile.velocity.ToRotation();
            SwingProjectile.uneasedLerpValue = lerpValue;
            swingProgress = easingFunc(swingProgress);      
            SwingProjectile._smoothedLerpValue = swingProgress;
            PlaySwingSound(swingProgress);

            float dir2 = (int)Projectile.ai[1];

            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 swingVelocity = swingDirection * stabRange;
            if (!_thrust)
            {
                Owner.velocity += swingDirection * thrustSpeed;
                _thrust = true;
            }

            Projectile.Center = Owner.Center +
                Vector2.Lerp(Vector2.Zero, swingVelocity, swingProgress) + swingDirection * SwingProjectile.holdOffset;
            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver4;
            if(spinRotationRange != 0)
            {
                float spinRotationProgress;
                if (spinRotationEasingFunc != null)
                    spinRotationProgress = spinRotationEasingFunc(swingProgress);
                else
                    spinRotationProgress = lerpValue;
                float spinRotation = dir2 == 1 ? MathHelper.Lerp(0, spinRotationRange, spinRotationProgress) : MathHelper.Lerp(spinRotationRange, 0, spinRotationProgress);
                Projectile.rotation += spinRotation;
            }

       

            if (spinRotationRange == 0)
            {
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
                    Vector2 pos = Owner.Center +
                        Vector2.Lerp(Vector2.Zero, swingVelocity, smoothedTrailProgress) + swingDirection * SwingProjectile.holdOffset;
                    points[i] = pos - (SwingProjectile.GetFramingSize() / 2);
                };

                SwingProjectile._trailPoints = points;
            }
            else
            {
                Vector2[] points = new Vector2[ProjectileID.Sets.TrailCacheLength[Projectile.type]];
                for (int i = 0; i < points.Length; i++)
                {
                    float l = points.Length;
                    //Lerp between the points
                    float progressOnTrail = i / l;

                    //Calculate starting lerp value
                    float startTrailLerpValue = MathHelper.Clamp(lerpValue - SwingProjectile.trailStartOffset, 0, 1);
                    float startTrailProgress = startTrailLerpValue;
                    if (spinRotationEasingFunc != null)
                        startTrailProgress = spinRotationEasingFunc(startTrailLerpValue);
                    else
                        startTrailProgress = startTrailLerpValue;

                    //Calculate ending lerp value
                    float endTrailLerpValue = lerpValue;
                    float endTrailProgress = endTrailLerpValue;
                    if (spinRotationEasingFunc != null)
                        endTrailProgress = spinRotationEasingFunc(endTrailLerpValue);
                    else
                        endTrailProgress = endTrailLerpValue;

                    //Lerp in between points
                    float smoothedTrailProgress = MathHelper.Lerp(startTrailProgress, endTrailProgress, progressOnTrail);
                    float rot = dir2 == 1 ? MathHelper.Lerp(0, spinRotationRange, smoothedTrailProgress) : MathHelper.Lerp(spinRotationRange, 0, smoothedTrailProgress);

                    Vector2 pos = Projectile.Center;
                    pos += rot.ToRotationVector2() * (SwingProjectile.GetTrailOffset()*spinTrailOffset) / 2 ;
                    points[i] = pos - SwingProjectile.GetFramingSize() / 2;
                }
                SwingProjectile._trailPoints = points;
            }

        }
    }

}
