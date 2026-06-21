using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Core.SwingSystem
{
    public class ThrustSwing : ISwing
    {
        private int _dir;
        private bool _hasThrust;
        private bool _hasPlayedSound;
        public ThrustSwing()
        {
            //Set some default values
            Duration = 30;
            HitCount = 1;
            ThrowDistance = 90;
            Easing = EasingFunction.InOutExpo;
            TrailOffset = 1.5f;
            DrawTrail = true;
        }
        public const float TRAIL_START_OFFSET = 0.2f;
        public float Duration { get; set; }
        public int HitCount { get; set; }
        public float ThrowDistance { get; set; }
        public float TrailOffset { get; set; }
        public float ThrustParticleOffset { get; set; }

        public float SpinDegrees { get; set; }
        public bool DrawTrail { get; set; }
        public Easer Easing { get; set; }
        public SoundStyle? Sound { get; set; }
        public Vector2? OverrideVelocity { get; set; }
        public float GetDuration(float attackSpeedMultiplier)
        {
            return Duration * attackSpeedMultiplier;
        }
        public int GetHitCount()
        {
            return HitCount;
        }

        public void SetDirection(int direction)
        {
            _dir = direction;
        }

        public bool CanHurt(BaseSwingProjectileV2 swingProjectile)
        {
            float time = swingProjectile.Interpolant;
            float ease = Easing(time);
            return time >= 0.35f;
        }
        private void CalculateOffset(float time, Vector2 velocity, out Vector2 offset)
        {
            if (OverrideVelocity.HasValue)
            {
                velocity = OverrideVelocity.Value;
            }
            float start = 0;
            float end = ThrowDistance;
            float interpolant = Easing(time);
            float distance = MathHelper.Lerp(start, end, interpolant);
            offset = velocity.SafeNormalize(Vector2.Zero) * distance;
        }
        public void UpdateSwing(BaseSwingProjectileV2 swingProjectile)
        {
            float time = swingProjectile.Interpolant;
            Vector2 position = swingProjectile.Projectile.Center;
            Vector2 velocity = swingProjectile.Projectile.velocity;

            if (!_hasThrust && time >= 0.1f)
            {
                ThrustParticleOffset = ThrowDistance / 4;
                if (OverrideVelocity.HasValue)
                {
                    velocity = OverrideVelocity.Value;
                }
                //FXUtil.SimpleImpactEffect(position + ThrustParticleOffset * velocity.SafeNormalize(Vector2.Zero), velocity, Main.rand.Next(4, 8), Color.White, Color.LightGray, Color.Black);

                float numDust = 3;
                for(int n = 0; n < numDust; n++)
                {
                    DustParticleSpawnParams spawnparams = new DustParticleSpawnParams
                    {
                        outerColor = Color.LightSkyBlue,
                        gravity = 0,
                        scaleRange = new Vector2(0.2f, 2f)

                    };
                    
                    var dp = DustParticle.Spawn(position + Main.rand.NextVector2Circular(16, 16) + ThrustParticleOffset * velocity.SafeNormalize(Vector2.Zero), velocity * Main.rand.NextFloat(0.5f, 1f) * 4, spawnparams);
                    dp.dampening = 0.1f;
                }

                ThrustParticle thrustParticle = ThrustParticle.Spawn(position , velocity * 2, Color.White, Scale: 1f);
                thrustParticle.bloomColor = Color.LightSkyBlue;
                if(swingProjectile.swordBeamLength > 0)
                {
                    thrustParticle = ThrustParticle.Spawn(position, velocity, swingProjectile.outlineColor, Scale: 1);
                    thrustParticle.scale2 *= 2;
                    thrustParticle.time = 60;
                    thrustParticle.color = swingProjectile.outlineColor;
                    thrustParticle.innerColor = swingProjectile.outlineColor;
                    thrustParticle.bloomColor = swingProjectile.outlineColor;
                }
                _hasThrust = true;
            }
            if (!_hasPlayedSound && time >= 0.35f && Sound != null)
            {
                SoundEngine.PlaySound(Sound, position);
                _hasPlayedSound = true;
            }

            CalculateOffset(time, velocity, out Vector2 offset);
            swingProjectile.EasedInterpolant = Easing(time);
            var projectile = swingProjectile.Projectile;
            projectile.Center = swingProjectile.Owner.Center + offset;
            projectile.rotation = (projectile.Center - swingProjectile.Owner.Center).ToRotation() + MathHelper.PiOver4;
            swingProjectile.extraLength = 128;
        }

        public void CalculateAfterImagePoints(BaseSwingProjectileV2 swingProjectile)
        {

            ref Vector2[] trailCache = ref swingProjectile.afterImageCache;
            ref float[] trailRotationCache = ref swingProjectile.swingRotationCache;
            Vector2 velocity = swingProjectile.Projectile.velocity;
            float time = swingProjectile.Interpolant;
            for (int t = 0; t < trailCache.Length; t++)
            {
                float l = trailCache.Length;
                float e = swingProjectile.oldTime[t * 5];

                CalculateOffset(e, velocity, out Vector2 offset);
                //Set Offset, now we can take this and offset it more in the projectile
                trailCache[t] = swingProjectile.Owner.Center + offset;
                trailRotationCache[t] = (trailCache[t] - swingProjectile.Owner.Center).ToRotation() + MathHelper.PiOver4;


            }
        }

        public void CalculateTrailingPoints(BaseSwingProjectileV2 swingProjectile)
        {
            float time = swingProjectile.Interpolant;
            ref Vector2[] trailCache = ref swingProjectile.swingTrailCache;
            Vector2 velocity = swingProjectile.Projectile.velocity;
            if (!DrawTrail)
                return;

            for (int t = 0; t < trailCache.Length; t++)
            {
                float l = trailCache.Length;
                //Lerp between the points
                float progressOnTrail = t / l;

                //Calculate starting lerp value
                float startTrailLerpValue = MathHelper.Clamp(time - TRAIL_START_OFFSET, 0, 1);

                //Calculate ending lerp value
                float endTrailLerpValue = time;

                //Smoothing lerp in between points
                float interpolant = MathHelper.SmoothStep(startTrailLerpValue, endTrailLerpValue, progressOnTrail);

                CalculateOffset(interpolant, velocity, out Vector2 offset);
                //Set Offset, now we can take this and offset it more in the projectile
                trailCache[t] = offset;
            }
        }
    }
}
