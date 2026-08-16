using System.Collections;

namespace Stellamod.Core.Utilities;

/// <summary>
/// Allows for iterating over points on a straight line
/// </summary>
/// <param name="left"></param>
/// <param name="right"></param>
/// <param name="numPoints"></param>
public record struct LineOrientation(Vector2 left, Vector2 right, float numPoints) : IEnumerable
{
    public Vector2 GetPoint(float progress) => Vector2.Lerp(left, right, progress);
    public IEnumerator GetEnumerator()
    {
        return new LineOrientationEnum(this);
    }
}

public struct LineOrientationEnum : IEnumerator
{
    public LineOrientation _orientation;

    // Enumerators are positioned before the first element
    // until the first MoveNext() call.
    int position = -1;

    public LineOrientationEnum(in LineOrientation orientation)
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

    public Vector2 Current
    {
        get
        {
            float ratio = position / _orientation.numPoints;
            return Vector2.Lerp(_orientation.left, _orientation.right, ratio);
        }
    }
}

