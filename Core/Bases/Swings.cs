using Stellamod.Core.SwingSystem;
using System;
using Terraria;

namespace Stellamod.Core.Bases;

/// <summary>
/// A collection of functions to help build sword swings
/// </summary>
public static class Swings
{

    /// <summary>
    /// Thrusts and holds the sword up, this will not deal any damage.
    /// </summary>
    public struct HoldUpSwing : ISwordMovement
    {
        public float thrustDistance;
        public SwordMovement CalculateSwordMovement(in SwingInput input)
        {
            var forwardOffsetStart = Vector2.Lerp(Vector2.Zero, Vector2.UnitX * thrustDistance, EasingFunction.OutCirc(input.uneasedLerpValue));
            var forwardOffsetEnd = Vector2.Lerp(Vector2.UnitX * thrustDistance, Vector2.Zero, EasingFunction.InExpo(input.uneasedLerpValue));
            var forwardOffset = Vector2.Lerp(forwardOffsetStart, forwardOffsetEnd, EasingFunction.InCirc(input.uneasedLerpValue));
            return new(forwardOffset, input.uneasedLerpValue, false);
        }
    }

    public struct GroundSwipeSwing : ISwordMovement
    {
        public float thrustDistance;
        public float arcRadians;
        public SwordMovement CalculateSwordMovement(in SwingInput input)
        {
            var halfArcRadians = arcRadians / 2f;
            var leftRadians = -halfArcRadians;
            var rightRadians = halfArcRadians;

            var radians = MathHelper.Lerp(rightRadians, leftRadians, EasingFunction.InOutBack(input.uneasedLerpValue));
            radians *= input.dir;

            var forwardOffsetStart = Vector2.Lerp(Vector2.Zero, Vector2.UnitX * thrustDistance, EasingFunction.OutCirc(input.uneasedLerpValue));
            var forwardOffsetEnd = Vector2.Lerp(Vector2.UnitX * thrustDistance, Vector2.Zero, EasingFunction.InExpo(input.uneasedLerpValue));
            var forwardOffset = Vector2.Lerp(forwardOffsetStart, forwardOffsetEnd, EasingFunction.InCirc(input.uneasedLerpValue));
            var combinedOffset = forwardOffset;
            combinedOffset = combinedOffset.RotatedBy(radians);
            
            return new SwordMovement(combinedOffset, input.uneasedLerpValue, combinedOffset.X > 4, combinedOffset.ToRotation());
        }
    }

    //Throws backward and then upward and arcs slightly upward
    public struct ChainsawSwing : ISwordMovement
    {
        public float thrustDistance;
        public float backupDistance;
        public float arcRadians;
        public SwordMovement CalculateSwordMovement(in SwingInput input)
        {
            var backupOffsetStart = Vector2.Lerp(Vector2.Zero, -Vector2.UnitX * backupDistance, EasingFunction.OutExpo(input.uneasedLerpValue / 0.7f));
            var backupOffsetEnd = Vector2.Lerp(-Vector2.UnitX * backupDistance, Vector2.Zero, EasingFunction.InCirc(input.uneasedLerpValue / 0.7f));
            var backupOffset = Vector2.Lerp(backupOffsetStart, backupOffsetEnd, input.uneasedLerpValue);

            var forwardOffsetStart = Vector2.Lerp(Vector2.Zero, Vector2.UnitX * thrustDistance, EasingFunction.OutCirc(input.uneasedLerpValue));
            var forwardOffsetEnd = Vector2.Lerp(Vector2.UnitX * thrustDistance, Vector2.Zero, EasingFunction.InExpo(input.uneasedLerpValue));
            var forwardOffset = Vector2.Lerp(forwardOffsetStart, forwardOffsetEnd, EasingFunction.InCirc(input.uneasedLerpValue));
            var combinedOffset = backupOffset + forwardOffset;

            combinedOffset = combinedOffset.RotatedBy(MathHelper.Lerp(arcRadians, -arcRadians, EasingFunction.InSine(input.uneasedLerpValue)));
            return new SwordMovement(combinedOffset, input.uneasedLerpValue, combinedOffset.X > 4, forwardOffset.ToRotation());
        }
    }

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
