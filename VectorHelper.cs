using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;
using Terraria;

namespace Stellamod
{
    public static class VectorHelper
    {
        /// <summary>
        /// Returns a point on a heart
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2 PointOnHeart(float t, float scale)
        {
            float x = 16 * MathF.Pow(MathF.Sin(t), 3);
            float y = 13f * MathF.Cos(t) - 5 * MathF.Cos(2 * t) - 2 * MathF.Cos(3 * t) - MathF.Cos(4 * t);
            return new Vector2(x, -y) * scale;
        }

        /// <summary>
        /// Makes a velocity home into another position
        /// <br>Homing strength should be between 0-1, with 1 being 100% accuracy. Use values below 0.2f for the best results</br>
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="speed"></param>
        /// <param name="homingStrength"></param>
        /// <returns></returns>
        public static Vector2 VelocityHomingTo(Vector2 startPosition, Vector2 currentVelocity, Vector2 endPosition, float homingStrength)
        {
            Vector2 vectorToTarget = endPosition - startPosition;
            Vector2 directionToTarget = vectorToTarget.SafeNormalize(Vector2.Zero);
            Vector2 velocityToTarget = directionToTarget * currentVelocity.Length();
            Vector2 newVelocity = Vector2.Lerp(currentVelocity, velocityToTarget, homingStrength);
            return newVelocity;
        }

        /// <summary>
        /// Returns a velocity towards a target position
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Vector2 VelocityDirectTo(Vector2 startPosition, Vector2 endPosition, float speed)
        {
            Vector2 direction = endPosition - startPosition;
            direction = direction.SafeNormalize(Vector2.Zero);
            Vector2 velocity = direction * speed;
            return velocity;
        }

        public static Vector2 VelocitySlowdownTo(Vector2 startPosition, Vector2 endPosition, float speed)
        {
            float distanceToEndPosition = Vector2.Distance(startPosition, endPosition);
            if (distanceToEndPosition < speed)
                speed = distanceToEndPosition;
            Vector2 direction = endPosition - startPosition;
            direction = direction.SafeNormalize(Vector2.Zero);
            Vector2 velocity = direction * speed;
            return velocity;
        }

        /// <summary>
        /// Moves the current velocity up to the target velocity, or does nothing if you are already moving faster than it
        /// <br>Good for recoil effects</br>
        /// </summary>
        /// <param name="currentVelocity"></param>
        /// <param name="targetVelocity"></param>
        /// <returns></returns>
        public static Vector2 VelocityUpTo(Vector2 currentVelocity, Vector2 targetVelocity)
        {
            if (currentVelocity.Length() < targetVelocity.Length())
            {
                Vector2 diff = targetVelocity - currentVelocity;
                currentVelocity += diff;
            }

            return currentVelocity;
        }

        public static Color MultiplyAlpha(this Color c, float alpha)
        {
            return new Color(c.R, c.G, c.B, (int)(c.A / 255f * MathHelper.Clamp(alpha, 0f, 1f) * 255f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Osc(float from, float to, float speed = 1f, float offset = 0f)
        {
            float dif = (to - from) / 2f;
            return from + dif + dif * (float)Math.Sin(Main.GlobalTimeWrappedHourly * speed + offset);
        }

        public static Vector2 MovemontVelocity(Vector2 A, Vector2 B, float speed)
        {
            Vector2 move = B - A;
            move *= speed / move.Length();
            if (!move.HasNaNs())
            {
                return move;
            }
            return Vector2.Zero;
        }

        public static Vector2 DirectionTo(this Vector2 origin, Vector2 target)
        {
            Vector2 diff = target - origin;
            return diff.SafeNormalize(Vector2.Zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Vector2 v, Vector2 To)
        {
            float dist = Vector2.Distance(v, To);
            if (!float.IsNaN(dist))
            {
                return dist;
            }
            return 0f;
        }
    }
}