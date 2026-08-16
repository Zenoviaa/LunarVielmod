using System.Collections;
using Terraria;

namespace Stellamod.Core.Utilities;


/// <summary>
/// Allows for iterating over randomly generated points in a circle
/// </summary>
/// <param name="origin"></param>
/// <param name="spawnEdgeRadius"></param>
/// <param name="numPoints"></param>
public record struct RandomCircleOrientation(Vector2 origin, float spawnRadius, float numPoints) : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        return new RandomCircleOrientationEnum(this);
    }
}


public struct RandomCircleOrientationEnum : IEnumerator
{
    public RandomCircleOrientation _orientation;

    // Enumerators are positioned before the first element
    // until the first MoveNext() call.
    int position = -1;

    public RandomCircleOrientationEnum(in RandomCircleOrientation orientation)
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
            Vector2 newPosition = _orientation.origin + Main.rand.NextVector2Circular(_orientation.spawnRadius, _orientation.spawnRadius);
            Vector2 dir = newPosition - _orientation.origin;
            dir = dir.SafeNormalize(Vector2.Zero);
            return new PositionVelocity(newPosition, dir);
        }
    }
}


