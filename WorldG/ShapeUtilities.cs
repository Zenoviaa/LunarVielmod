using Microsoft.Xna.Framework;
using System;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG
{
    public static class ShapeUtilities
    {
        public class HalfCircle : GenShape
        {
            private int _verticalRadius;
            private int _horizontalRadius;

            public HalfCircle(int radius)
            {
                _verticalRadius = radius;
                _horizontalRadius = radius;
            }

            public HalfCircle(int horizontalRadius, int verticalRadius)
            {
                _horizontalRadius = horizontalRadius;
                _verticalRadius = verticalRadius;
            }

            public void SetRadius(int radius)
            {
                _verticalRadius = radius;
                _horizontalRadius = radius;
            }

            public override bool Perform(Point origin, GenAction action)
            {
                int radius = _verticalRadius;
                int num = (radius + 1) * (radius + 1);
                for (int i = origin.Y + radius; i >= origin.Y; i--)
                {
                    int num2 = Math.Min(radius, (int)Math.Sqrt(num - (i - origin.Y) * (i - origin.Y)));
                    for (int j = origin.X - num2; j <= origin.X + num2; j++)
                    {
                        if (!UnitApply(action, origin, j, i) && _quitOnFail)
                            return false;
                    }
                }

                return true;
            }
        }
    }


}
