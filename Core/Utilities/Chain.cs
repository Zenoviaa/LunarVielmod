using System;

namespace Stellamod.Core.Utilities;

/// <summary>
/// Simulates very basic verlet integration for a set of points, it resolves from back to front
/// </summary>
public class Chain
{
    public Chain(Vector2 initialPosition, float pointLength, int numPoints)
    {
        this.segmentLength = pointLength;
        this.points = new Vector2[numPoints];
        for (int i = 0; i < this.points.Length; i++)
        {
            this.points[i] = initialPosition + Vector2.UnitX * i * 5;
        }
        this.pinned = new bool[numPoints];
    }
    public float segmentLength;
    public Vector2[] points;
    public bool[] pinned;
    public void Resolve()
    {

        for (int i = points.Length - 1; i >= 1; i--)
        {
            if (pinned[i])
                continue;

            ref Vector2 p2 = ref points[i - 1];
            ref Vector2 p1 = ref points[i];
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);
            if (distance > segmentLength)
            {
                float difference = segmentLength - distance;
                float percent = difference / distance;
                float offsetX = dx * percent;
                float offsetY = dy * percent;
                p1.X -= offsetX;
                p1.Y -= offsetY;

            }
        }
    }
}
