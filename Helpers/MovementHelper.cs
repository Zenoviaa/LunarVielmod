using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Helpers
{
    internal class MovementHelper
    {
        public static Vector2 OrbitAround(Vector2 center, Vector2 startDirection, float distance, float radians)
        {
            Vector2 offsetPos = center + (startDirection * distance);
            Vector2 rotatedPos = offsetPos.RotatedBy(radians, center);
            return rotatedPos;
        }
    }
}
