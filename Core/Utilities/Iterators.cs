using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Core.Utilities;

public static class Iterators
{
    public static OldPositionEnum IterateOldPosBackwards(this Projectile projectile)
    {
        return new OldPositionEnum(projectile.oldPos);
    }
}

public struct OldPosition
{
    public Vector2 position;
    public float progress;
    public int index;
}

public record struct OldPositionEnum(in Vector2[] Array) : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        return new OldPositionEnumerator(this);
    }
}

public struct OldPositionEnumerator : IEnumerator
{
    public OldPositionEnum _enumerable;
    public float floatLength;
    public float stepSize;


    // Enumerators are positioned before the first element
    // until the first MoveNext() call.
    int position = -1;

    public OldPositionEnumerator(in OldPositionEnum orientation)
    {
        _enumerable = orientation;
        floatLength = orientation.Array.Length;
        stepSize = 1f / orientation.Array.Length;
        position = _enumerable.Array.Length;
    }

    public bool MoveNext()
    {
        position--;
        return (position >= 0);
    }

    public void Reset()
    {
        position = _enumerable.Array.Length;
    }

    object IEnumerator.Current
    {
        get
        {
            return Current;
        }
    }

    public OldPosition Current
    {
        get
        {
            float p = stepSize * position;
            return new OldPosition
            {
                progress = stepSize * position,
                index = position,
                position = _enumerable.Array[position]
            };
        }
    }
}


