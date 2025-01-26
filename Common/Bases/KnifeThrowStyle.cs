using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    public class KnifeDashPlayer : ModPlayer
    {
        public Vector2? DashToPosition = null;
        public float DashSlowdownTimer;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (DashToPosition != null)
            {
                Vector2 positionToDashTo = DashToPosition.Value;
                Vector2 dashDirection = (positionToDashTo - Player.Center).SafeNormalize(Vector2.Zero);
                Vector2 dashVelocity = dashDirection * 18;
                float distanceToPosition = Vector2.Distance(Player.Center, positionToDashTo);
                float speed = 18;
                if (distanceToPosition < speed || distanceToPosition > 500 || !Collision.CanHitLine(Player.position, 1, 1, positionToDashTo, 1, 1))
                {
                    DashSlowdownTimer = 15;
                    DashToPosition = null;
                }
                Player.velocity = dashDirection * speed;


            }

            if (DashSlowdownTimer > 0)
            {
                DashSlowdownTimer--;
                Player.velocity *= 0.97f;
            }
        }
    }

    public class KnifeThrowStyle : BaseSwingStyle
    {
        private bool _threw;
        private bool _hasDashed;
        private Vector2 _throwPos;
        private Vector2 _startOwnerPos;
        public float thrustSpeed;
        public float throwRange;
        public float spinRotationRange;
        public Func<float, float> spinRotationEasingFunc;
        public Func<float, float> dashEasingFunc;
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
            Vector2 swingVelocity = swingDirection * throwRange;
            if (!_threw)
            {
                _throwPos = Owner.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * throwRange;
                _threw = true;
            }

            float increments = 4f;
            float halfTime = SwingProjectile.GetSwingTime(swingTime) / increments;
            float div = 1 / increments;
            float throwTime = (div * 3);
            float remainingTime = halfTime;
            if (SwingProjectile.uneasedLerpValue < throwTime)
            {
                float newLerpValue = SwingProjectile.Timer / halfTime;
                float newSwingProgress = easingFunc(newLerpValue);
                Projectile.Center = Vector2.Lerp(Owner.Center, _throwPos, newSwingProgress) + swingDirection * SwingProjectile.holdOffset;
                Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver4;
                _startOwnerPos = Owner.Center;
            }
            else
            {
                float newLerpValue = (SwingProjectile.Timer - halfTime * 3f) / remainingTime;
                float newSwingProgress = newLerpValue;
                newLerpValue = MathHelper.Clamp(newLerpValue, 0f, 1f);
                if (dashEasingFunc != null)
                {
                    newSwingProgress = dashEasingFunc(newLerpValue);
                }
                // Owner.Center = Vector2.Lerp(_startOwnerPos, _throwPos, newSwingProgress);
                if (newLerpValue >= 0.75f && !_hasDashed)
                {
                    Owner.GetModPlayer<KnifeDashPlayer>().DashToPosition = _throwPos;
                    _hasDashed = true;
                }
            }


            if (spinRotationRange != 0)
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
                    pos += rot.ToRotationVector2() * (SwingProjectile.GetTrailOffset() * spinTrailOffset) / 2;
                    points[i] = pos - SwingProjectile.GetFramingSize() / 2;
                }
                SwingProjectile._trailPoints = points;
            }

        }
    }

}
