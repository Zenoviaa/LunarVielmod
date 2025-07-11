﻿using Microsoft.Xna.Framework;
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
            HitCount = 1;
        }

        public const float TRAIL_START_OFFSET = 0.2f;
        public float Duration { get; set; }
        public int HitCount { get; set; }
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

        public float RadOffset => 0;
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

        private void CalculateXY(float interpolant, Vector2 velocity, out float xOffset, out float yOffset)
        {
            float range = _swingRadians;
            float startRads = -range / 2;
            float endRads = range / 2;

            startRads *= _dir;
            endRads *= _dir;

            float rads = MathHelper.Lerp(startRads, endRads, interpolant);
            rads += MathHelper.PiOver2;

            // rads += targetRotation;
            xOffset = XSwingRadius * MathF.Sin(rads);
            yOffset = YSwingRadius * MathF.Cos(rads);
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
            float radOffset = _swingRadians;
            float targetRotation = velocity.ToRotation();
            CalculateXY(easedInterpolant, velocity, out xOffset, out yOffset);

            //Set Offset
            offset = new Vector2(xOffset, yOffset).RotatedBy(targetRotation);
        }

        public void CalculateAfterImagePoints(float time, Vector2 velocity, ref Vector2[] trailCache)
        {
            //Alright, calculating trail points
            //The points will be offset by the position matrix
            //So we just calculate the local points here
            for (int t = 0; t < trailCache.Length; t++)
            {
                float l = trailCache.Length;
                //Lerp between the points
                float progressOnTrail = t / l;

                float xOffset;
                float yOffset;
                float targetRotation = velocity.ToRotation();
                float interpolant = MathHelper.SmoothStep(0, time, progressOnTrail);
                CalculateXY(interpolant, velocity, out xOffset, out yOffset);
                //Set Offset, now we can take this and offset it more in the projectile
                trailCache[t] = new Vector2(xOffset, yOffset).RotatedBy(targetRotation);
            }
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

                float radOffset = _swingRadians / 2;
                float targetRotation = velocity.ToRotation();
                CalculateXY(interpolant, velocity, out xOffset, out yOffset);
                //Set Offset, now we can take this and offset it more in the projectile
                trailCache[t] = new Vector2(xOffset * TrailOffset, yOffset * TrailOffset).RotatedBy(targetRotation);
            }
        }
    }
}
