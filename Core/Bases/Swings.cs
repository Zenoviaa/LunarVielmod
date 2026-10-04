using Stellamod.Core.SwingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Core.Bases;

/// <summary>
/// A collection of functions to help build sword swings
/// </summary>
public static class Swings
{
    /// <summary>
    /// Spins around the player for the time
    /// </summary>
    public struct SpinningSwing : ISwordMovement
    {
        public float radius;
        public float radians;
        public SwordMovement CalculateSwordMovement(in SwingInput input)
        {
            var start = Vector2.Lerp(Vector2.Zero, Vector2.UnitX * radius, EasingFunction.OutExpo(input.uneasedLerpValue));
            var end = Vector2.Lerp(Vector2.UnitX * radius, Vector2.Zero, EasingFunction.InExpo(input.uneasedLerpValue));
            var pos = Vector2.Lerp(start, end, input.uneasedLerpValue);
            pos = pos.RotatedBy(radians * input.uneasedLerpValue);
            return new SwordMovement(pos, input.uneasedLerpValue, true);
        }
    }
    /// <summary>
    /// Quickly throws out the sword and waves a little bit
    /// </summary>
    public struct CurlSwing : ISwordMovement
    {
        public CurlSwing()
        {
            xRange = 64;
            frequency = 12;
            swingRadians = 1f;
        }

        public float swingRadians;
        public float xRange;
        public float frequency;
        public SwordMovement CalculateSwordMovement(in SwingInput input)
        {
            var uneasedInterpolant = input.uneasedLerpValue;
            var waviness = MathF.Sin(uneasedInterpolant * frequency) * 0.5f + 0.5f;
            waviness = MathHelper.Lerp(0.75f, 1f, waviness);
            var start = Vector2.Lerp(Vector2.Zero, Vector2.UnitX * xRange * waviness, EasingFunction.OutExpo(uneasedInterpolant));
            var end = Vector2.Lerp(Vector2.UnitX * xRange * waviness, Vector2.Zero, EasingFunction.InExpo(uneasedInterpolant));
            var pos = Vector2.Lerp(start, end, uneasedInterpolant);

            var range = swingRadians / 2f;
            pos = pos.RotatedBy(MathHelper.Lerp(-range * input.dir, range * input.dir, EasingFunction.OutSine(uneasedInterpolant)));

            var easer = EasingFunction.OutCirc(uneasedInterpolant);
            return new SwordMovement(pos, easer, true);
        }
    }
}
