using System.Collections;
using Terraria;

namespace Stellamod.Core.Utilities;

/// <summary>
/// Allows for iterating over points along a circle
/// </summary>
/// <param name="origin"></param>
/// <param name="spawnEdgeRadius"></param>
/// <param name="numPoints"></param>
public record struct CircleOrientation(Vector2 origin, float spawnEdgeRadius, float numPoints) : IEnumerable
{
    public PositionVelocity Get(int index)
    {
        float ratio = index / numPoints;
        Vector2 dir = (ratio * MathHelper.TwoPi).ToRotationVector2();
        dir *= spawnEdgeRadius;
        Vector2 newPosition = origin + dir;
        return new PositionVelocity(newPosition, dir);
    }
    public IEnumerator GetEnumerator()
    {
        return new CircleOrientationEnum(this);
    }
}


public struct CircleOrientationEnum : IEnumerator
{
    public CircleOrientation _orientation;

    // Enumerators are positioned before the first element
    // until the first MoveNext() call.
    int position = -1;

    public CircleOrientationEnum(in CircleOrientation orientation)
    {
        _orientation = orientation;
    }

    public bool MoveNext()
    {
        position++;
        return (position < _orientation.numPoints);
    }

    public void Reset()
    {
        position = -1;
    }

    object IEnumerator.Current
    {
        get
        {
            return Current;
        }
    }

    public PositionVelocity Current
    {
        get
        {
            float ratio = position / _orientation.numPoints;
            Vector2 dir = (ratio * MathHelper.TwoPi).ToRotationVector2();
            dir *= _orientation.spawnEdgeRadius;
            Vector2 newPosition = _orientation.origin + dir;
            return new PositionVelocity(newPosition, dir);
        }
    }

    public PositionVelocity Get(int index)
    {
        float ratio = index / _orientation.numPoints;
        Vector2 dir = (ratio * MathHelper.TwoPi).ToRotationVector2();
        dir *= _orientation.spawnEdgeRadius;
        Vector2 newPosition = _orientation.origin + dir;
        return new PositionVelocity(newPosition, dir);
    }
}


