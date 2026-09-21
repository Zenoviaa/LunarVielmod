using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Core.Utilities
{
    public static class RectangleExtensions
    {
        extension(Rectangle rectangle)
        {
            public Rectangle MakeCenteredFromTopLeft()
            {
                Rectangle centeredRectangle = rectangle;
                centeredRectangle.X -= centeredRectangle.Width / 2;
                centeredRectangle.Y -= centeredRectangle.Height / 2;
                return centeredRectangle;
            }
            public Rectangle MakeCenteredFromBottomLeft()
            {
                Rectangle centeredRectangle = rectangle;
                centeredRectangle.X -= centeredRectangle.Width / 2;
                centeredRectangle.Y += centeredRectangle.Height / 2;
                return centeredRectangle;
            }
        }
    }
}
