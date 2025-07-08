using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Core.SwingSystem
{
    internal class ThrustSwing : ISwing
    {
        private int _dir;
        public ThrustSwing()
        {
            //Set some default values
            Duration = 30;
            HitCount = 1;
            ThrowDistance = 64;
            Easing = EasingFunction.InOutExpo;
            TrailOffset = 1.5f;
        }
        public const float TRAIL_START_OFFSET = 0.2f;
        public float Duration { get; set; }
        public int HitCount { get; set; }
        public float ThrowDistance { get; set; }
        public float TrailOffset { get; set; }
        public Easer Easing { get; set; }
        public SoundStyle? Sound { get; set; }
        public float GetDuration()
        {
            return Duration;
        }
        public int GetHitCount()
        {
            return HitCount;
        }

        public void SetDirection(int direction)
        {
            _dir = direction;
        }

        private void CalculateOffset(float time, Vector2 velocity, out Vector2 offset)
        {
            float start = 0;
            float end = ThrowDistance;
            float interpolant = Easing(time);
            float distance = MathHelper.Lerp(start, end, interpolant);
            offset = velocity.SafeNormalize(Vector2.Zero) * distance;
        }
        public void UpdateSwing(float time, Vector2 position, Vector2 velocity, out Vector2 offset)
        {
            CalculateOffset(time, velocity, out offset);
        }

        public void CalculateTrailingPoints(float time, Vector2 velocity, ref Vector2[] trailCache)
        {
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
