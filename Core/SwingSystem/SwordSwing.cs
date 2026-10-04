using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Core.SwingSystem;

/// <summary>
/// Defines a movement for a sword swing
/// </summary>
public interface ISwordMovement
{
    SwordMovement CalculateSwordMovement(in SwingInput input);
}
public record struct SwordMovement(Vector2 Offset, float EasedInterpolant, bool CanHurt);
public readonly record struct SwingInput(float uneasedLerpValue, int dir);
public class SwordSwing : ISwing
{
    int _dir;
    bool _playedSound;
    public SwordSwing()
    {
        ttrailOffset = 1.2f;
        baseSwingTime = 32;
        sound = AssetReferences.Assets.Sounds.Melee.SwordSwing.Asset;
    }

    public float baseSwingTime;
    public int hitCount;
    public float ttrailOffset;
    public SoundStyle? sound;
    public ISwordMovement swordMovementFunction;
    SwordMovement GetOffset(float uneasedInterpolant, float targetRotation)
    {
        var input = new SwingInput(uneasedInterpolant, _dir);
        var swordEasing = swordMovementFunction.CalculateSwordMovement(input);
        swordEasing.Offset = swordEasing.Offset.RotatedBy(targetRotation);
        return swordEasing;
    }

    public void CalculateAfterImagePoints(BaseSwingProjectileV2 swingProjectile)
    {
        ref Vector2[] trailCache = ref swingProjectile.afterImageCache;
        float[] oldTime = swingProjectile.oldTime;
        ref float[] trailRotationCache = ref swingProjectile.swingRotationCache;
        Vector2 velocity = swingProjectile.Projectile.velocity;
        float interpolant = swingProjectile.Interpolant;
        //Alright, calculating trail points
        //The points will be offset by the position matrix
        //So we just calculate the local points here
        for (int t = 0; t < trailCache.Length; t++)
        {
            float time = swingProjectile.oldTime[t * 5];
            var targetRotation = velocity.ToRotation();
            var swordEasing = GetOffset(time, targetRotation);
            trailCache[t] = swingProjectile.Owner.Center + swordEasing.Offset;
            trailRotationCache[t] = (trailCache[t] - swingProjectile.Owner.Center).ToRotation() + MathHelper.PiOver4;
        }
    }

    public void CalculateTrailingPoints(BaseSwingProjectileV2 swingProjectile)
    {
        float time = swingProjectile.Interpolant;
        ref Vector2[] trailCache = ref swingProjectile.swingTrailCache;
        ref Vector2[] bigTrailCache = ref swingProjectile.bigSwingTrailCache;
        Vector2 velocity = swingProjectile.Projectile.velocity;
        if (time - swingProjectile.trailVisibilityOffset < 0)
            return;
        //Set Offset, now we can take this and offset it more in the projectile
        float trailOffset = ttrailOffset;
        if (swingProjectile.trailOffsetOverride.HasValue)
        {
            trailOffset = swingProjectile.trailOffsetOverride.Value;
        }
        //Alright, calculating trail points
        //The points will be offset by the position matrix
        //So we just calculate the local points here
        float length = (float)trailCache.Length;
        for (int t = trailCache.Length - 1; t >= 0; t--)
        {
            var timer = swingProjectile.Timer;
            timer -= t * 1f;
            timer = MathHelper.Clamp(timer, 0, swingProjectile.swingTime);
            var targetRotation = velocity.ToRotation();
            var interpolant = timer / swingProjectile.swingTime;

            var o = GetOffset(interpolant, targetRotation).Offset;
            Vector2 offset = new Vector2(o.X * trailOffset, o.Y * trailOffset);
            trailCache[t] = offset;

            Vector2 offset2 = offset;
            offset2 *= 2;
            bigTrailCache[t] = offset2.RotatedBy(targetRotation);
        }
    }

    public bool CanHurt(BaseSwingProjectileV2 swingProjectile)
    {
        float time = swingProjectile.Interpolant;
        var ease = GetOffset(time, swingProjectile.Projectile.velocity.ToRotation());
        return ease.CanHurt;
    }

    public float GetDuration(float attackSpeedMultiplier)
    {
        return baseSwingTime * attackSpeedMultiplier;
    }

    public int GetHitCount()
    {
        return hitCount;
    }

    public void SetDirection(int direction)
    {
        direction = (int)Math.Clamp(direction, -1, 1);
        _dir = direction;
    }

    public void UpdateSwing(BaseSwingProjectileV2 swingProjectile)
    {
        float time = swingProjectile.Interpolant;
        Vector2 position = swingProjectile.Projectile.Center;
        Vector2 velocity = swingProjectile.Projectile.velocity;
        float targetRotation = velocity.ToRotation();
        //Calculate easing
        var swordEasing = GetOffset(time, targetRotation);
        float easedInterpolant = swordEasing.EasedInterpolant;
        swingProjectile.EasedInterpolant = easedInterpolant;
        if (!_playedSound && easedInterpolant >= 0.35f && sound != null)
        {
            var soundInstance = sound.Value;
            if (!swingProjectile.isAfterImageProjectile)
            {
                SoundEngine.PlaySound(soundInstance, position);
            }

            _playedSound = true;
        }

        var projectile = swingProjectile.Projectile;

        projectile.Center = swingProjectile.Owner.Center + swordEasing.Offset;
        projectile.rotation = (projectile.Center - swingProjectile.Owner.Center).ToRotation() + MathHelper.PiOver4;

    }
}
