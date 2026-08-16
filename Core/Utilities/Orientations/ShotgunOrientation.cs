using System.Collections;
using Terraria;

namespace Stellamod.Core.Utilities;


/// <summary>
/// Allows for iterating over randomly generated positions and velocities in a shotgun fashion, note that each new foreach loop will generate different points
/// </summary>
/// <param name="origin"></param>
/// <param name="direction"></param>
/// <param name="spreadBox"></param>
/// <param name="spreadRadians"></param>
/// <param name="numPoints"></param>
public record struct ShotgunOrientation(Vector2 origin, Vector2 direction, float spreadBox, float spreadRadians, float numPoints, float speedVariance = 0.2f) : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        return new ShotgunOrientationEnum(this);
    }
}

public struct ShotgunOrientationEnum : IEnumerator
{
    public ShotgunOrientation _orientation;

    // Enumerators are positioned before the first element
    // until the first MoveNext() call.
    int position = -1;

    public ShotgunOrientationEnum(in ShotgunOrientation orientation)
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
            Vector2 offset = Main.rand.NextVector2Circular(_orientation.spreadBox, _orientation.spreadBox);
            Vector2 newPosition = _orientation.origin + offset;
            Vector2 dir = _orientation.direction;
            dir = dir.RotatedByRandom(_orientation.spreadRadians);
            dir *= Main.rand.NextFloat(1f - _orientation.speedVariance, 1f);
            return new PositionVelocity(newPosition, dir);
        }
    }
}
