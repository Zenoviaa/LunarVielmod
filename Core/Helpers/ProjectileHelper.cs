using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Core.Helpers
{
    internal class ProjectileHelper
    {
        public static float AngleTo(Vector2 Center, Vector2 Destination) => (float)System.Math.Atan2(Destination.Y - Center.Y, Destination.X - Center.X);
        public static Vector2 SimpleHomingVelocity(Vector2 currentPosition, Vector2 targetPosition, Vector2 currentVelocity, float degreesToRotate = 3)
        {
            float length = currentVelocity.Length();
            float targetAngle = AngleTo(currentPosition, targetPosition);
            Vector2 newVelocity = currentVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            return newVelocity;
        }
        public static Vector2 SimpleHomingVelocity(Projectile projectile, Vector2 targetPosition, float degreesToRotate = 3)
        {
            float length = projectile.velocity.Length();
            float targetAngle = projectile.AngleTo(targetPosition);
            Vector2 newVelocity = projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            return newVelocity;
        }
        public static Vector2 SimpleHomingVelocity(Projectile projectile, Vector2 targetPosition,
            Vector2 currentVelocity, float degreesToRotate = 3)
        {
            float length = currentVelocity.Length();
            float targetAngle = projectile.AngleTo(targetPosition);
            Vector2 newVelocity = currentVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            return newVelocity;
        }
    }
}
