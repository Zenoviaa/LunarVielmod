using Microsoft.Xna.Framework;

namespace Stellamod.Core.TriggersSystem
{
    public class DraggablePointField
    {
        public int X;
        public int Y;
        public Point Point
        {
            get
            {
                return new Point(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
    }
}
