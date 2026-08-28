using Stellamod.Core.SwingSystem;
using System;
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

public class MovementUtilities
{

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
        Vector2 ovalPoint = MovementUtilities.OvalProgressPoint(parameters.ratio, radians, xSize, parameters.ySize);
        Vector2 ovalOffset = MovementUtilities.LocalOvalRotate(ovalPoint, parameters.direction, parameters.rotation);
        return parameters.start + ovalOffset;
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
