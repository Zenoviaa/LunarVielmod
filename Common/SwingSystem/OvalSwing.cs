using Microsoft.Xna.Framework;
using Stellamod.Common.Helpers.Math;
using System;
using Terraria;

namespace Stellamod.Common.SwingSystem
{
    internal class OvalSwing : ISwing
    {
        private int _dir;
        private float _swingRadians;
        public OvalSwing()
        {
            //Set some default values
            Duration = 30;
            XSwingRadius = 32;
            YSwingRadius = 24;
            SwingDegrees = 270;
            Easing = EasingFunction.InOutExpo;
        }

        public float Duration { get; set; }
        public float XSwingRadius { get; set; }
        public float YSwingRadius { get; set; }
        public float SwingDegrees
        {
            get => MathHelper.ToDegrees(_swingRadians);
            set => _swingRadians = MathHelper.ToRadians(value);
        }
        public Easer Easing { get; set; }

        public float GetDuration()
        {
            return Duration;
        }

        public void SetDirection(int direction)
        {
            _dir = direction;
        }

        public void UpdateSwing(float time, Vector2 velocity,
            out Vector2 offset)
        {
            //Calculate easing
            float easedInterpolant = Easing(time);

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

        public void CalculateTrailingPoints(ref Vector2[] trailCache)
        {
            throw new NotImplementedException();
        }
    }
}
