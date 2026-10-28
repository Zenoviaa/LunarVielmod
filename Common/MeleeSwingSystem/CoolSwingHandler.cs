using Stellamod.Core.SwingSystem;
using System.Collections.Generic;
using Terraria;


namespace Stellamod.Common.MeleeSwingSystem;

public class CoolSwingHandler
{
    /// <summary>
    /// A buffer for calculating trail points
    /// </summary>
    public static readonly Vector2[] TrailCacheBuffer = new Vector2[64];
    public readonly List<Swinger> Movements;
    public CoolSwingHandler()
    {
        Movements = new List<Swinger>();
    }
    public void Add(Swinger movement)
    {
        Movements.Add(movement);
    }



    public ProjFirer KillThisSwingGetNext(Projectile projectile)
    {
        SwingPlayerV2 comboPlayer = projectile.PlayerOwner.GetModPlayer<SwingPlayerV2>();
        int combo = projectile.CoolSwingComboIndex;
        int dir = (int)projectile.CoolSwingDirection;
        comboPlayer.IncreaseCombo();
        projectile.Kill();
        var firer = ProjFirer.Copy(projectile);
        firer.ai0 = 0;
        firer.ai1 = dir;
        firer.ai2 = combo + 1;
        return firer;
    }

    /// <summary>
    /// Sets the correct position and rotation of the swing.
    /// Assumes that this is a swing projectile and has the ai values 1 and 2 populated
    /// </summary>
    /// <param name="p"></param>
    public void ApplyTo(Projectile p, bool allowKill = true)
    {
        float targetRotation = p.velocity.ToRotation();
        var movement = this[p];

            movement.Offset = movement.Offset.RotatedBy(targetRotation);
        if (Movements[(int)p.CoolSwingComboIndex].fixRot)
        {
            if(p.PlayerOwner.direction == -1)
            {
                movement.Offset = movement.Offset.RotatedBy(MathHelper.Pi);
                movement.Offset *= new Vector2(-1, 1);
            }
        }



        p.friendly = movement.CanHurt;
        p.Center = p.PlayerOwner.Center + movement.Offset;
        p.rotation = (p.Center - p.PlayerOwner.Center).ToRotation() + MathHelper.PiOver4;
        if (movement.OverrideRotation.HasValue)
        {
            p.rotation = movement.OverrideRotation.Value + targetRotation + MathHelper.PiOver4;
        }
        if (movement.EasedInterpolant >= 1f && allowKill)
        {
            p.Kill();
        }
    }

    public int Count => Movements.Count;
    public float GetTime(Projectile comboIndex) => this[comboIndex.CoolSwingComboIndex].swingTime * (1f / comboIndex.PlayerOwner.GetTotalAttackSpeed(comboIndex.DamageType));
    public float GetInterpolant(Projectile comboIndex) => comboIndex.CoolSwingTimer /GetTime(comboIndex);
    public SwordMovement this[Projectile p]
    {
        get
        {
            var combo = p.CoolSwingComboIndex;
            if (combo >= Movements.Count)
                combo = 0;
            return this[combo].movement.CalculateSwordMovement(new()
            {
                dir = this[combo].direction == 0 ? combo % 2 == 0 ? 1 : -1 : this[combo].direction,
                uneasedLerpValue = (p.CoolSwingTimer / GetTime(p))
            });
        }
    }

    public Swinger GetSwinger(Projectile swingProjectile) => Movements[(int)swingProjectile.CoolSwingComboIndex];
    public void CalculateTrailingPoints(Projectile swingProjectile, ref Vector2[] trailCache, float trailOffset = 1.25f)
    {
        float time = GetInterpolant(swingProjectile);
        Vector2 velocity = swingProjectile.velocity;

        //Alright, calculating trail points
        //The points will be offset by the position matrix
        //So we just calculate the local points here
        float length = (float)trailCache.Length;
        var swingTime = GetTime(swingProjectile);
        for (int t = trailCache.Length - 1; t >= 0; t--)
        {
            var timer = swingProjectile.CoolSwingTimer;
            timer -= t * 0.2f;
            timer = MathHelper.Clamp(timer, 0, swingTime);
            var targetRotation = velocity.ToRotation();
            var interpolant = timer / swingTime;

            var combo = swingProjectile.CoolSwingComboIndex;
            if (combo >= Movements.Count)
                combo = 0;

            var swordMovement = this[combo].movement.CalculateSwordMovement(new()
            {
                dir = this[combo].direction == 0 ? combo % 2 == 0 ? 1 : -1 : this[combo].direction,
                uneasedLerpValue = (timer / swingTime)
            });

       


            var o = swordMovement.Offset;

            if (Movements[(int)swingProjectile.CoolSwingComboIndex].fixRot)
            {
                if (swingProjectile.PlayerOwner.direction == -1)
                {
                    o = o.RotatedBy(MathHelper.Pi);
                    o *= new Vector2(-1, 1);
                }
            }


            Vector2 offset = new Vector2(o.X * trailOffset, o.Y * trailOffset).RotatedBy(targetRotation);
            trailCache[t] = offset;
        }
    }
    public Swinger this[int i]
    {
        get => Movements[i];
        set
        {
            Movements[i] = value;

        }
    }
}
