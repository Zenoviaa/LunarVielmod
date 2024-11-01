using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Helpers
{
    internal static class ProjectileHelper
    {
        public static bool IsValidTarget(NPC target, Vector2 currentPosition)
        {
            // This method checks that the NPC is:
            // 1. active (alive)
            // 2. chaseable (e.g. not a cultist archer)
            // 3. max life bigger than 5 (e.g. not a critter)
            // 4. can take damage (e.g. moonlord core after all it's parts are downed)
            // 5. hostile (!friendly)
            // 6. not immortal (e.g. not a target dummy)
            // 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
            return target.CanBeChasedBy() && Collision.CanHit(currentPosition, 1, 1, target.position, target.width, target.height);
        }
        public static NPC FindNearestEnemy(Vector2 currentPosition, float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                // Check if NPC able to be targeted. 
                if (IsValidTarget(target, currentPosition))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, currentPosition);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public static Vector2 SimpleHomingVelocity(Projectile projectile, Vector2 targetPosition, float degreesToRotate = 3)
        {
            float length = projectile.velocity.Length();
            float targetAngle = projectile.AngleTo(targetPosition);
            Vector2 newVelocity = projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            return newVelocity;
        }
        public static Vector2 SimpleHomingVelocity(
            Vector2 currentPosition, Vector2 targetPosition,
            Vector2 currentVelocity,
            float homingFactor)
        {

            return currentVelocity;
        }
    }
}
