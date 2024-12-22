using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Stellamod.Helpers
{
    internal static class ProjectileHelper
    {
        public static bool? OldPosColliding(Vector2[] positions, Rectangle projHitbox, Rectangle targetHitbox, float lineWidth = 6)
        {
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                float dist = Vector2.Distance(previousPosition, position);
                if (dist > 1000)
                    continue;

                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, lineWidth, ref collisionPoint))
                    return true;
            }
            return false;
        }
        public static float PerformBeamHitscan(Vector2 startPosition, Vector2 velocity, float maxBeamLength, int numSamplePoints = 3)
        {
            // By default, the hitscan interpolation starts at the Projectile's center.
            // If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
            Vector2 samplingPoint = startPosition;

            // Perform a laser scan to calculate the correct length of the beam.
            // Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
            // return MaxBeamLength;
            float[] laserScanResults = new float[numSamplePoints];


            Vector2 direction = velocity.SafeNormalize(Vector2.Zero);
            Collision.LaserScan(samplingPoint, direction, 0 * 1f, maxBeamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= numSamplePoints;
            return averageLengthSample;
        }
        public static float PerformBeamHitscan(Projectile projectile, float maxBeamLength, int numSamplePoints = 3)
        {
            // By default, the hitscan interpolation starts at the Projectile's center.
            // If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
            Vector2 samplingPoint = projectile.Center;

            // Perform a laser scan to calculate the correct length of the beam.
            // Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
            // return MaxBeamLength;
            float[] laserScanResults = new float[numSamplePoints];


            Vector2 direction = projectile.velocity.SafeNormalize(Vector2.Zero);
            Collision.LaserScan(samplingPoint, direction, 0 * projectile.scale, maxBeamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= numSamplePoints;
            return averageLengthSample;
        }
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
        public static NPC FindNearestEnemyThroughWalls(Vector2 currentPosition, float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                // Check if NPC able to be targeted. 
                if (target.CanBeChasedBy())
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
        public static NPC FindNearestEnemyUnderneath(Vector2 currentPosition, float maxDetectDistance, float maxXDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                if (target.position.Y < currentPosition.Y)
                    continue;
                if (MathF.Abs(target.position.X - currentPosition.X) > maxXDistance)
                    continue;

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
