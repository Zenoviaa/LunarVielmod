using System.Collections;

namespace Stellamod.Core.Utilities;

public static class RectangleExtenstions
{
    extension(Rectangle rect)
    {
        public RectangleEnumerable Points => new RectangleEnumerable(rect);
    }
}

public readonly record struct RectangleEnumerable(Rectangle Rect) : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        return new RectangleEnum(Rect);
        //       throw new NotImplementedException();
    }
}
public struct RectangleEnum : IEnumerator
{
    private int _x;
    private int _y;
    public Rectangle _rectangle;
    public RectangleEnum(in Rectangle orientation)
    {
        _y = _rectangle.Top - 1;
        _x = _rectangle.Left;
        _rectangle = orientation;
    }

    public bool MoveNext()
    {
        _y++;
        if (_y >= _rectangle.Bottom)
        {
            _y = 0;
            _x++;
            if (_x >= _rectangle.Right)
                return false;
        }
        return true;
    }

    public void Reset()
    {
        _y = -1;
        _x = 0;
    }

    object IEnumerator.Current
    {
        get
        {
            return Current;
        }
    }

    public Point Current
    {
        get
        {
            return new Point(_x, _y);
        }
    }
}


