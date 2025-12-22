using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Helpers
{
    public static class NPCAIHelper
    {
        public static Vector2 CalculatePositionToMoveTo(Vector2 targetCenter, 
            Vector2 startingCenter, Vector2 floatingOffset)
        {
            float direction = targetCenter.X > startingCenter.X ? 1 : -1;
            Vector2 offset = floatingOffset;
            offset.X *= -direction;
            Vector2 newPosition = targetCenter + offset;
            return newPosition;
        }


    }
}
