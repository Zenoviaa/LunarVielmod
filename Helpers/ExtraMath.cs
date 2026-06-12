using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
namespace Stellamod.Helpers
{
    public static class ExtraMath
    {
        public static float AngleDiff(float a, float b)
        {
            float a1 = MathHelper.ToDegrees(a);
            float a2 = MathHelper.ToDegrees(b);

            float dif = (float)Math.Abs(a1 - a2) % 360;

            if (dif > 180)
                dif = 360 - dif;

            dif = MathHelper.ToRadians(dif);
            return dif;
        }

        public static Vector2 CubicBezier(Vector2 start, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 end, float t)
        {
            float tSquared = t * t;
            float tCubed = t * t * t;
            return
                -(start * (-tCubed + (3 * tSquared) - (3 * t) - 1) +
                controlPoint1 * ((3 * tCubed) - (6 * tSquared) + (3 * t)) +
                controlPoint2 * ((-3 * tCubed) + (3 * tSquared)) +
                end * tCubed);
        }

        public static float Saturate(float a)
        {
            return MathHelper.Clamp(a, 0f, 1f);
        }
        /// <summary>
        /// Shorthand for the distance between two entities
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static float DistanceFrom(this Entity start, Entity end)
        {
            return Vector2.Distance(start.Center, end.Center);
        }

        /// <summary>
        /// Shorthand for calculating the velocity to an entity's center
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Vector2 VelocityTo(this Entity start, Entity end)
        {
            return (end.Center - start.Center);
        }

        /// <summary>
        /// Shorthand for calculating the normalized velocity to an entity's center
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Vector2 NormalizedVelocityTo(this Entity start, Entity end)
        {
            return (end.Center - start.Center).SafeNormalize(Vector2.Zero);
        }

        public static Vector2 GetRotation(Vector2[] oldPos, int index)
        {
            if (oldPos.Length == 1)
                return oldPos[0];

            if (index == 0)
                return Vector2.Normalize(oldPos[1] - oldPos[0]).RotatedBy(MathHelper.Pi / 2);

            return (index == oldPos.Length - 1
                ? Vector2.Normalize(oldPos[index] - oldPos[index - 1])
                : Vector2.Normalize(oldPos[index + 1] - oldPos[index - 1])).RotatedBy(MathHelper.Pi / 2);
        }

        public static void LerpTrailPoints(Vector2[] oldPos, out Vector2[] trailingPoints, float smoothFactor = 2)
        {
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < oldPos.Length - 1; i++)
            {
                Vector2 current = oldPos[i];
                Vector2 next = oldPos[i + 1];
                for (float j = 0; j < smoothFactor; j++)
                {
                    float p = j / smoothFactor;
                    Vector2 smoothedPoint = Vector2.Lerp(current, next, p);
                    points.Add(smoothedPoint);
                }
            }
            trailingPoints = points.ToArray();
        }

        public static Vector2[] RemoveZeros(Vector2[] arr, Vector2 offset)
        {
            var valid = new List<Vector2>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == Vector2.Zero || arr[i].HasNaNs())
                    break;
                if (i != 0)
                {
                    if (arr[i - 1] == arr[i])
                        continue;

                    var d = arr[i - 1] - arr[i];
                    if (d.X < -1000f || d.X > 1000f || d.Y < -1000f || d.Y > 1000f)
                    {
                        continue;
                    }
                }
                valid.Add(arr[i] + offset);
            }
            return valid.ToArray();
        }

        public static float Osc(float from, float to, float speed = 1f, float offset = 0f)
        {
            float dif = (to - from) / 2f;
            return from + dif + dif * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * speed + offset);
        }
        public static float OscTimer(float timer, float from, float to, float speed = 1f, float offset = 0f)
        {
            float dif = (to - from) / 2f;
            return from + dif + dif * (float)System.Math.Sin(timer * speed + offset);
        }
    }
}
