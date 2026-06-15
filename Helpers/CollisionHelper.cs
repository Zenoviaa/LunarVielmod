using Microsoft.Xna.Framework;
using Stellamod.Core.LunarLightingSystem;
using System;
using Terraria;

namespace Stellamod.Helpers
{
    public struct Triangle
    {
        public Point a;
        public Point b;
        public Point c;
    }

    public static class CollisionHelper
    {
        //https://www.geeksforgeeks.org/dsa/check-whether-a-given-point-lies-inside-a-triangle-or-not/
        /* A utility function to calculate area of triangle 
    formed by (x1, y1) (x2, y2) and (x3, y3) */
        public static double area(int x1, int y1, int x2,
                           int y2, int x3, int y3)
        {
            return Math.Abs((x1 * (y2 - y3) +
                             x2 * (y3 - y1) +
                             x3 * (y1 - y2)) / 2.0);
        }

        /* A function to check whether point P(x, y) lies
        inside the triangle formed by A(x1, y1),
        B(x2, y2) and C(x3, y3) */
        public static bool isInside(int x1, int y1, int x2,
                             int y2, int x3, int y3,
                             int x, int y)
        {
            /* Calculate area of triangle ABC */
            double A = area(x1, y1, x2, y2, x3, y3);

            /* Calculate area of triangle PBC */
            double A1 = area(x, y, x2, y2, x3, y3);

            /* Calculate area of triangle PAC */
            double A2 = area(x1, y1, x, y, x3, y3);

            /* Calculate area of triangle PAB */
            double A3 = area(x1, y1, x2, y2, x, y);

            /* Check if sum of A1, A2 and A3 is same as A */
            return (A == A1 + A2 + A3);
        }

        public static bool IsInsideTriangle(Triangle triangle, Point point)
        {
            return isInside(triangle.a.X, triangle.a.Y, triangle.b.X, triangle.b.Y, triangle.c.X, triangle.c.Y, point.X, point.Y);
        }

        public static Vector2 RayCast(Vector2 startPosition, Vector2 velocity, float maxBeamLength, int numSamplePoints = 3)
        {
            // By default, the hitscan interpolation starts at the Projectile's center.
            // If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
            Vector2 samplingPoint = startPosition;

            // Perform a laser scan to calculate the correct length of the beam.
            // Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
            // return MaxBeamLength;
            float[] laserScanResults = new float[numSamplePoints];


            Vector2 direction = velocity.SafeNormalize(Vector2.Zero);
            Collision.LaserScan(samplingPoint, direction, 0 * 1f, maxBeamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= numSamplePoints;
            return startPosition + direction * averageLengthSample;
        }
        //https://stackoverflow.com/questions/5514366/how-to-know-if-a-line-intersects-a-rectangle
        public static bool LineIntersectsRect(Point p1, Point p2, Rectangle r)
        {
            return LineIntersectsLine(p1, p2, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y), new Point(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y + r.Height), new Point(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X, r.Y + r.Height), new Point(r.X, r.Y)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }

        private static bool LineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }
    }
}
