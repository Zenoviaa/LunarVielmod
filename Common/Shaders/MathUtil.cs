using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public static class MathUtil
    {

        public static Vector2 GetRotation(Vector2[] oldPos, int index)
        {
            if (oldPos.Length == 1)
                return oldPos[0];

            if (index == 0)
                return Vector2.Normalize(oldPos[1] - oldPos[0]).RotatedBy(MathHelper.PiOver2);

            return (index == oldPos.Length - 1
                ? Vector2.Normalize(oldPos[index] - oldPos[index - 1])
                : Vector2.Normalize(oldPos[index + 1] - oldPos[index - 1])).RotatedBy(MathHelper.PiOver2);
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

        public static float Osc(float from, float to, float speed = 1f, float offset = 0f)
        {
            float dif = (to - from) / 2f;
            return from + dif + dif * (float)Math.Sin(Main.GlobalTimeWrappedHourly * speed + offset);
        }
    }
}
