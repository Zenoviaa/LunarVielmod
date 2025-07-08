using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Stellamod.Core.Helpers.Math;
using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Core.SwingSystem
{
    internal class OvalSwing : ISwing
    {
        private int _dir;
        private float _swingRadians;
        private bool _hasPlayedSound;
        public OvalSwing()
        {
            //Set some default values
            Duration = 30;
            XSwingRadius = 32;
            YSwingRadius = 24;
            SwingDegrees = 270;
            Easing = EasingFunction.InOutExpo;
            TrailOffset = 1.5f;
        }

        public const float TRAIL_START_OFFSET = 0.2f;
        public float Duration { get; set; }
        public float XSwingRadius { get; set; }
        public float YSwingRadius { get; set; }
        public float SwingDegrees
        {
            get => MathHelper.ToDegrees(_swingRadians);
            set => _swingRadians = MathHelper.ToRadians(value);
        }

        public float TrailOffset { get; set; }
        public Easer Easing { get; set; }
        public SoundStyle? Sound { get; set; }

        public float GetDuration()
        {
            return Duration;
        }

        public void SetDirection(int direction)
        {
            _dir = direction;
        }

        public void UpdateSwing(float time, Vector2 position, Vector2 velocity,
            out Vector2 offset)
        {
            //Calculate easing
            float easedInterpolant = Easing(time);
            if (!_hasPlayedSound && easedInterpolant >= 0.35f && Sound != null)
            {
                SoundEngine.PlaySound(Sound, position);
                _hasPlayedSound = true;
            }
            //Calculate the offset at this time
            float xOffset;
            float yOffset;
            if (_dir == -1)
            {
                xOffset = XSwingRadius * MathF.Sin(easedInterpolant * _swingRadians + _swingRadians / 2f);
                yOffset = YSwingRadius * MathF.Cos(easedInterpolant * _swingRadians + _swingRadians / 2f);
            }
            else
            {
                xOffset = XSwingRadius * MathF.Sin((1f - easedInterpolant) * _swingRadians + _swingRadians / 2f);
                yOffset = YSwingRadius * MathF.Cos((1f - easedInterpolant) * _swingRadians + _swingRadians / 2f);
            }

            float targetRotation = velocity.ToRotation();

            //Set Offset
            offset = new Vector2(xOffset, yOffset).RotatedBy(targetRotation + MathHelper.Pi);
        }

        public void CalculateTrailingPoints(float time, Vector2 velocity, ref Vector2[] trailCache)
        {
            //Alright, calculating trail points
            //The points will be offset by the position matrix
            //So we just calculate the local points here
            for (int t = 0; t < trailCache.Length; t++)
            {
                float l = trailCache.Length;
                //Lerp between the points
                float progressOnTrail = t / l;

                //Calculate starting lerp value
                float startTrailLerpValue = MathHelper.Clamp(time - TRAIL_START_OFFSET, 0, 1);
                float startTrailProgress = startTrailLerpValue;
                startTrailProgress = Easing(startTrailLerpValue);


                //Calculate ending lerp value
                float endTrailLerpValue = time;
                float endTrailProgress = endTrailLerpValue;
                endTrailProgress = Easing(endTrailLerpValue);

                //Smoothing lerp in between points
                float interpolant = MathHelper.SmoothStep(startTrailProgress, endTrailProgress, progressOnTrail);

                float xOffset;
                float yOffset;
                if (_dir == -1)
                {
                    xOffset = XSwingRadius * MathF.Sin(interpolant * _swingRadians + _swingRadians / 2f);
                    yOffset = YSwingRadius * MathF.Cos(interpolant * _swingRadians + _swingRadians / 2f);
                }
                else
                {
                    xOffset = XSwingRadius * MathF.Sin((1f - interpolant) * _swingRadians + _swingRadians / 2f);
                    yOffset = YSwingRadius * MathF.Cos((1f - interpolant) * _swingRadians + _swingRadians / 2f);
                }

                float targetRotation = velocity.ToRotation();

                //Set Offset, now we can take this and offset it more in the projectile
                trailCache[t] = new Vector2(xOffset * TrailOffset, yOffset * TrailOffset).RotatedBy(targetRotation + MathHelper.Pi);
            }
        }
    }
}
