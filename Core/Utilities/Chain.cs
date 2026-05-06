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
    
    private void ResolveInner(int index)
    {
        if (pinned[index])
            return;

        ref Vector2 p2 = ref points[index - 1];
        ref Vector2 p1 = ref points[index];
        float dx = p2.X - p1.X;
        float dy = p2.Y - p1.Y;
        float distance = MathF.Sqrt(dx * dx + dy * dy);

        //Calculating one direction is one way to get around the bounciness the other verlet integration implementation has
        //This looks a lot cleaner and way less stiff
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

    public void ResolveBackToRoot()
    {
        for (int i = points.Length - 1; i >= 1; i--)
        {
            ResolveInner(i);
        }
    }

    public void ResolveRootToBack()
    {
        for (int i = 1; i < points.Length; i++)
        {
            ResolveInner(i);
        }
    }
}
