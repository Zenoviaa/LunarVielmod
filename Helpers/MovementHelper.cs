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

        public static Vector2 HomingVelocity(Vector2 currentVelocity, Vector2 targetPosition, float homingFactor)
        {
            homingFactor = MathHelper.Clamp(homingFactor, 0, 1);
            Vector2 directionToTargetPosition = (targetPosition - currentVelocity).SafeNormalize(Vector2.Zero);
            float targetRot = directionToTargetPosition.ToRotation();
            currentVelocity = currentVelocity.RotatedBy(targetRot * homingFactor);
            return currentVelocity;
        }
    }
}
