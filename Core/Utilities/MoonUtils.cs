using Stellamod.Core.Astar;
using Stellamod.Core.SwingSystem;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;

namespace Stellamod.Core.Utilities;

public struct SteinUppercutParameters
{
    public Vector2 start;
    public Vector2 end;
    public Vector2 direction;
    public float ratio;
    public float swingRadians;
    public float rotation;
    public float ySize;
}

public struct Targeter
{
    public Vector2 targetOldPos;
    public int targetNpc;
    public NPC Target
    {
        get
        {
            if (targetNpc == -1)
                return Main.npc[0];
            return Main.npc[targetNpc];
        }
    }
    public bool HasValidTarget => targetNpc != -1 && Target.active;
    public void NetSend(BinaryWriter writer)
    {
        writer.Write(targetNpc);
    }
    public void NetReceive(BinaryReader reader)
    {
        targetNpc = reader.ReadInt32();
    }
}

/// <summary>
///  A collection of common functions for implementing projectiles and npcs
/// </summary>
public static class MoonUtils
{
    const float SUMMON_SEARCH_DISTANCE = 512;
    const float REPATH_DISTANCE = 32 * 32;
    public static Vector2 RayCast(Vector2 startPosition, Vector2 velocity, float maxBeamLength, int numSamplePoints = 3)
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
        return startPosition + direction * averageLengthSample;
    }

    #region PathfindingAI

    /// <summary>
    /// Lerps velocity to the target point
    /// </summary>
    /// <param name="center"></param>
    /// <param name="velocity"></param>
    /// <param name="target"></param>
    public static void AI_FloatAbove( Vector2 center, ref Vector2 velocity, Vector2 target)
    {
        var targetVelocity = (target - center) * 0.05f;
        velocity = Vector2.Lerp(velocity, targetVelocity, 0.1f);
    }


    /// <summary>
    /// Searches for a new target based on what is close to the origin position and what can be seen from the current position
    /// </summary>
    /// <param name="centerSearchPos"></param>
    /// <param name="myPosition"></param>
    /// <param name="targetNpc"></param>
    public static void SearchForNewTargetByLineOfSight(Vector2 centerSearchPos, Vector2 myPosition, ref int targetNpc)
    {
        targetNpc = -1;
        var closestEnemy = MoonUtils.TargetClosestEnemy(centerSearchPos, SUMMON_SEARCH_DISTANCE);
        if (closestEnemy == null)
            return;
        if (!Collision.CanHitLine(myPosition, 1, 1, closestEnemy.position, 1, 1))
            return;

        targetNpc = closestEnemy.whoAmI;
    }

    /// <summary>
    /// Searches for a new target purely based on what's around the origin point
    /// </summary>
    /// <param name="centerSearchPos"></param>
    /// <param name="targetNpc"></param>
    public static void SearchForNewTargetByDistance(Vector2 centerSearchPos, ref int targetNpc)
    {
        targetNpc = -1;
        var closestEnemy = MoonUtils.TargetClosestEnemy(centerSearchPos, SUMMON_SEARCH_DISTANCE);
        if (closestEnemy == null)
            return;

        targetNpc = closestEnemy.whoAmI;
    }
    public static void SearchForNewTargetByDistance(Vector2 centerSearchPos, ref int targetNpc, float distance)
    {
        targetNpc = -1;
        var closestEnemy = MoonUtils.TargetClosestEnemy(centerSearchPos, distance);
        if (closestEnemy == null)
            return;

        targetNpc = closestEnemy.whoAmI;
    }
    public static void SetMouseFacingDirection(Vector2 center, float range, ref float direction)
    {
        var diffX = Main.MouseWorld.X - center.X;
        var distX = MathF.Abs(diffX);
        if(distX >= range)
        {
            direction = (Main.MouseWorld.X < center.X) ? -1 : 1;
        }
    }
    public static Vector2 CalculateHoverAbovePoint(Vector2 center, float timer, float minionPos, float hoverRange = 32)
    {

        var xOfffset = MathHelper.Lerp(-64, 64, ExtraMath.Osc(0f, 1f, speed: 0, minionPos * 2));
        xOfffset += MathHelper.Lerp(-hoverRange, hoverRange, MathF.Sin(timer * 0.025f) * 0.5f + 0.5f);
        var targetPos = center + new Vector2(0, -48) + new Vector2(xOfffset, 0);
        return targetPos;
    }

    public static void AIWalk_FloatingChaseRhapsody(Pathfinder pathfinder, Projectile entity, Vector2 destination, float runSpeed,ref Vector2 targetOldPos)
    {
        if (Vector2.DistanceSquared(targetOldPos, destination) > REPATH_DISTANCE || Main.GameUpdateCount % 30 == 0)
        {
            targetOldPos = destination;
            //    var startPost = TileUtilities.FallToSolidTileOrPlatform(entity.Center);
            pathfinder.NewPath(destination, entity.Center, 75);
        }

        if (pathfinder.currentNode != Vector2.Zero)
        {
            MoonUtils.AIWalk_FloatingChaseRhapsody(pathfinder, entity, destination, runSpeed);
        }
        else if (pathfinder.path != null && pathfinder.path.Count > 0)
        {
            pathfinder.Pop();
        }
    }

    public static void AIWalk_FloatingChaseRhapsody(Pathfinder pathfinder, Projectile entity, Vector2 destination, float runSpeed)
    {
        var target = pathfinder.currentNode;
        if (Collision.CanHitLine(entity.position, 1, 1, destination, 1, 1))
        {
            target = destination;
        }

        //We can make the assumption that whatever node we're moving too is VERY close to our actor
        //So here's how it works, we create a 16x16 rectangle around the the point we're moving to
        //If that rectangle intersects our hitbox rectangle, then the destination has been reached.
        var targetRectangle = DrawUtilities.CenterRectangle(pathfinder.currentNode, 16, 16);
        var nextRectangle = DrawUtilities.CenterRectangle(pathfinder.nextNode, 16, 16);
        var myRectangle = DrawUtilities.CenterRectangle(entity.Center, 32, 32);

        //The rectangle is padded slightly prevent the entity getting stuck if it's hitbox is slightly smaller than the rectangles

        float distanceToCurrentNode = Vector2.Distance(entity.Center, pathfinder.currentNode);
        float distanceToNextNode = Vector2.Distance(entity.Center, pathfinder.nextNode);

        if (myRectangle.Intersects(targetRectangle) || distanceToNextNode < distanceToCurrentNode ||
            myRectangle.Intersects(nextRectangle) || myRectangle.Contains(targetRectangle))
        {

            pathfinder.Pop();
        }

        var diff = (target - entity.Center);
        var targetVelocity = diff.Resize(runSpeed);
        var accel = ExtraMath.Osc(0.05f, 0.1f, speed: 0, offset: entity.identity);
        entity.velocity = Vector2.Lerp(entity.velocity, targetVelocity, accel);
    }


    public static void AIWalk_IvynStabber(Pathfinder pathfinder, Projectile entity, Vector2 destination, bool isGrounded, float runSpeed, float maxJumpSpeed, ref Vector2 targetOldPos)
    {
        if (Vector2.DistanceSquared( targetOldPos, destination) > REPATH_DISTANCE || Main.GameUpdateCount % 30 ==0)
        {
            targetOldPos = destination;
        //    var startPost = TileUtilities.FallToSolidTileOrPlatform(entity.Center);
            pathfinder.NewPath(destination, entity.Center, 75);
        }

        if (pathfinder.currentNode != Vector2.Zero)
        {
            MoonUtils.AIWalk_IvynStabber(pathfinder, entity, destination, isGrounded, runSpeed, maxJumpSpeed);
        }
        else if (pathfinder.path != null && pathfinder.path.Count > 0)
        {
            pathfinder.Pop();
        }
    }

    public static void AIWalk_IvynStabber(Pathfinder pathfinder, Projectile entity, Vector2 destination, bool isGrounded, float runSpeed, float maxJumpSpeed)
    {
        var target = pathfinder.currentNode;
        if (Collision.CanHitLine(entity.position, 1, 1, destination, 1, 1))
        {
            target = destination;
        }
        //We can make the assumption that whatever node we're moving too is VERY close to our actor
        //So here's how it works, we create a 16x16 rectangle around the the point we're moving to
        //If that rectangle intersects our hitbox rectangle, then the destination has been reached.
        var targetRectangle = DrawUtilities.CenterRectangle(pathfinder.currentNode, 16, 16);
        var nextRectangle = DrawUtilities.CenterRectangle(pathfinder.nextNode, 16, 16);
        var myRectangle = DrawUtilities.CenterRectangle(entity.Center, 32, 32);

        //The rectangle is padded slightly prevent the entity getting stuck if it's hitbox is slightly smaller than the rectangles
        
        float distanceToCurrentNode = Vector2.Distance(entity.Center, pathfinder.currentNode);
        float distanceToNextNode = Vector2.Distance(entity.Center, pathfinder.nextNode);

        if (myRectangle.Intersects(targetRectangle) || distanceToNextNode < distanceToCurrentNode ||
            myRectangle.Intersects(nextRectangle) || myRectangle.Contains(targetRectangle))
        {

            pathfinder.Pop();
        }

        //Since this is a grounded entity, we can't just directly move towards the point we wish to reach
        //First we'll try to reach the target destination on the X axis, and once the X axis has been satisfied, we'll try to reach it on the y Axis
        //If the y axis is above, then we'll jump
        var diffX = (target.X - entity.Center.X);
        var distX = MathF.Abs(diffX);
        if (distX <= myRectangle.Width)
        {
     
            var diffY = (target.Y - entity.Center.Y);
            
            var distY = MathF.Abs(diffY);

            if (diffY < 0 && distY > myRectangle.Height && isGrounded)
            {
     
                var maxSpeed = MathF.Min(maxJumpSpeed, distY / 8);
                entity.velocity.Y = -maxSpeed;
            }
        }

        var dirX = MathF.Sign(diffX);
        var targetXVelocity = dirX * runSpeed;
        var accel = ExtraMath.Osc(0.05f, 0.1f, speed: 0, offset: entity.identity);
        entity.velocity.X = MathHelper.Lerp(entity.velocity.X, targetXVelocity, accel);
        Collision.StepUp(ref entity.position, ref entity.velocity, entity.width, entity.height, ref entity.stepSpeed, ref entity.gfxOffY);
    }
    #endregion

    /// <summary>
    /// Checks if an entity is grounded by checking for a tile collision 1 tile underneath of it
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGrounded(Entity entity)
    {
        var tilePointBelow = entity.Bottom.ToTileCoordinates();
        var tileBelow = Main.tile[tilePointBelow];
        tilePointBelow.Y++;
        var tileBelow2 = Main.tile[tilePointBelow];
        return WorldGen.SolidOrSlopedTile(tileBelow) || WorldGen.SolidOrSlopedTile(tileBelow2) || Main.tileSolidTop[tileBelow.type] || Main.tileSolidTop[tileBelow2.type];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 VelocityTo(Entity start, Entity end, float speed) => VelocityTo(start.Center, end.Center, speed);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 VelocityTo(Vector2 start, Vector2 end, float speed)
    {
        var dir = (end - start);
        dir = dir.SafeNormalize(Vector2.Zero);
        dir *= speed;
        return dir;
    }

    /// <summary>
    /// Returns the nearest enemy to the current position
    /// </summary>
    /// <param name="currentPosition"></param>
    /// <param name="searchRange"></param>
    /// <returns></returns>
    public static NPC TargetClosestEnemy(Vector2 currentPosition, float searchRange)
    {
        NPC closest = null;
        var closestDistance = 9999999F;
        var sqrSearchDist = searchRange * searchRange;
        foreach(var npc in Main.ActiveNPCs)
        {
            if (npc.friendly)
                continue;
            if (!npc.CanBeChasedBy())
            {
                continue;
            }

            var sqrDist = Vector2.DistanceSquared(currentPosition, npc.Center);
            if (sqrDist > sqrSearchDist)
                continue;

            if(sqrDist < closestDistance)
            {
                closest = npc;
                closestDistance = sqrDist;
            }
        }
        return closest;
    }
    public static NPC TargetClosestEnemyLeveled(Vector2 currentPosition, float searchRange)
    {
        NPC closest = null;
        var closestDistance = 9999999F;
        var sqrSearchDist = searchRange * searchRange;
        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.friendly)
                continue;
            if (!npc.CanBeChasedBy())
            {
                continue;
            }

            var sqrDist = Vector2.DistanceSquared(currentPosition, npc.Center);
            if (sqrDist > sqrSearchDist)
                continue;

            var verticalSqrDist = MathF.Abs(npc.Center.Y - currentPosition.Y);
            if (verticalSqrDist > 64)
                continue;


            if (sqrDist < closestDistance)
            {
                closest = npc;
                closestDistance = sqrDist;
            }
        }
        return closest;
    }
    public static void JumpTowards(ref Vector2 velocity, Vector2 currentPosition, Vector2 targetPosition, Vector2 jumpSpeed)
    {
        var dirToTarget = (targetPosition - currentPosition).SafeNormalize(Vector2.Zero);
        if(dirToTarget.Y < 0)
        {
            var yDiff = (targetPosition.Y - currentPosition.Y); 
            var ySpeed = MathF.Min(-jumpSpeed.Y, yDiff);
            velocity.Y = ySpeed;

            var xDiff = (targetPosition.X - currentPosition.X);
            var xDir = MathF.Sign(xDiff);
            velocity.X = xDir * jumpSpeed.X;
        }
        else
        {
            //goober is below, what are we gonna jump towards
            //just do a tiny hop aybe?
            //unsure, doing nothing might be the best bet here
        }
    }


    public static void FaceMovementVelocity(NPC npc)
    {
        npc.spriteDirection = npc.velocity.X < 0 ? -1 : 1;
    }

    public static void AIMoveTowardsTarget(Vector2 currentPosition, Vector2 targetPosition,
        ref Vector2 velocity, float speed, float lerp)
    {
        Vector2 directionTo = targetPosition - currentPosition;
        directionTo = directionTo.SafeNormalize(Vector2.Zero);
        Vector2 targetVelocity = directionTo * speed;
        velocity = Vector2.Lerp(velocity, targetVelocity, lerp);
    }

    /// <summary>
    /// A
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public static Vector2 FindCeiling(in Vector2 worldPosition, int maxTileSteps = 100)
    {
        Point tilePoint = worldPosition.ToTileCoordinates();
        int x = tilePoint.X;
        int y = tilePoint.Y;
        for(int i = 0; i < maxTileSteps && y > 0; i++)
        {
            y--;
            Tile tile = Main.tile[x, y];
            if(tile.HasTile && Main.tileSolid[tile.TileType])
            {
                return new Point(x, y).ToWorldCoordinates();
            }
        }
        return worldPosition;
    }
    public static Vector2 FindFloor(in Vector2 worldPosition, int maxTileSteps = 100)
    {
        Point tilePoint = worldPosition.ToTileCoordinates();
        int x = tilePoint.X;
        int y = tilePoint.Y;
        for (int i = 0; i < maxTileSteps && y > 0; i++)
        {
            y++;
            Tile tile = Main.tile[x, y];
            if (tile.HasTile && Main.tileSolid[tile.TileType])
            {
                return new Point(x, y).ToWorldCoordinates();
            }
        }
        return worldPosition;
    }
    public static Vector2 FindFloorWet(in Vector2 worldPosition, int maxTileSteps = 100)
    {
        Point tilePoint = worldPosition.ToTileCoordinates();
        int x = tilePoint.X;
        int y = tilePoint.Y;
        for (int i = 0; i < maxTileSteps && y > 0; i++)
        {
            y++;
            Tile tile = Main.tile[x, y];
            if ((tile.HasTile && Main.tileSolid[tile.TileType]) || tile.LiquidAmount > 0)
            {
                return new Point(x, y).ToWorldCoordinates();
            }
        }
        return worldPosition;
    }
    public static Vector2 SteinGetEndPoint(Player player, in Vector2 startPosition, in Vector2 targetPosition, in float maxDistance)
    {
        float adjustedMaxDistance = player.GetModPlayer<MeleeEffectsPlayer>().steinDistanceBonus * maxDistance + maxDistance;
        float distSquared = adjustedMaxDistance * adjustedMaxDistance;
        Vector2 endPoint = targetPosition;
        if(Vector2.DistanceSquared(startPosition, targetPosition) > distSquared)
        {

            endPoint = startPosition + (targetPosition - startPosition).SafeNormalize(Vector2.Zero) * adjustedMaxDistance;
        }

        return endPoint;
    }

    /// <summary>
    /// Calculates the point for the given progress value for a stein's punching attack
    /// </summary>
    /// <param name="player"></param>
    /// <param name="progress"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static Vector2 SteinCalculateSwingPoint(in float progress, in Vector2 start, in Vector2 end)
    {
        float ease = EasingFunction.QuadraticBump(progress);
        Vector2 pos = Vector2.Lerp(start, end, ease);
        return pos;
    }

    /// <summary>
    /// Calculates the point for the given parameters for a stein's uppercut attack, see Gothinstein for an example implementation
    /// </summary>
    /// <param name="player"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static Vector2 SteinCalculateUppercutSwingPoint(in SteinUppercutParameters parameters)
    {
        float radians = parameters.swingRadians;
        float xSize = Vector2.Distance(parameters.start, parameters.end);
        Vector2 ovalPoint = MoonUtils.OvalProgressPoint(parameters.ratio, radians, xSize, parameters.ySize);
        Vector2 ovalOffset = MoonUtils.LocalOvalRotate(ovalPoint, parameters.direction, parameters.rotation);
        return parameters.start + ovalOffset;
    }
    public static Vector2[] SwingPoints(Vector2 offset, in float maxProgress, in float numPoints, in float radians, in float xSize, in float ySize, in float rotation)
    {
        var points = new Vector2[(int)numPoints];
        for(var i = 0; i < points.Length; i++)
        {
            var progress = (float)i / (float)points.Length;
            var ovalOffset = OvalProgressPoint(progress * maxProgress, radians, xSize, ySize);
            ovalOffset = ovalOffset.RotatedBy(rotation);
            ref var point = ref points[i];
            point = offset + ovalOffset;
        }
        return points;
    }
    public static Vector2 OvalProgressPoint(in float progress, in float radians, in float xSize, in float ySize)
    {
        float x = MathF.Sin(progress * radians) * xSize;
        float y = MathF.Cos(progress * radians) * ySize;
        return new Vector2(x, y);

    }

    public static Vector2 LocalOvalRotate(in Vector2 ovalPoint, in Vector2 direction, in float rotation)
    {
        Vector2 offset = ovalPoint;
        offset *= direction;
        offset = offset.RotatedBy(rotation);
        return offset;
    }

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
