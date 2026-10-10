using Stellamod.Common;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Astar;
using Stellamod.Core.Bases;
using Stellamod.Items;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS;

public class ArncharFlyingDrone : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToBellMinion(ModContent.ProjectileType<ArncharMinionProj>());
        Item.damage = 38;
        Item.knockBack = 3f;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<Cinderscrap>());
    }
}

public class ArncharMinionProj : AbstractBellSummon
{
    Targeter _targeter;
    Pathfinder _pathfinder;
    ref float Timer => ref Projectile.ai[0];
    enum AIState
    {
        Idle,
        Attack,
        GoHome,
        SeekTarget
    }

    AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    ref float IdleTimer => ref Projectile.ai[2];

    float RunSpeed => 9;
    float HomeRange => 200 * 200;
    float SqrDistHome => 1240 * 1240;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        _targeter.NetSend(writer);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _targeter.NetReceive(reader);
    }

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 4;
        Projectile.SetTrailCacheLength(16);
        Projectile.StaticDefaultToMinionProjectile();
    }

    public override void SetDefaults()
    {
        _pathfinder = new();
        Projectile.DefaultToMinionProjectile();
        Projectile.tileCollide = false;
        Projectile.WidthAndHeight = 24;
        Projectile.tileCollide = true;
        Projectile.friendly = false;
        Projectile.light = 0.67f;
    }

    public override int GetAggro()
    {
        return -90;
    }

    public override bool? CanCutTiles()
    {
        return false;
    }

    public override void DrawSpectral(SpriteBatch spriteBatch)
    {
        base.DrawSpectral(spriteBatch);

    }

    public override bool MinionContactDamage()
    {
        return false;
    }

    void AI_Idle()
    {
        if (IdleTimer >= 30)
            MoonUtils.SearchForNewTargetByDistance(Owner.Center, ref _targeter.targetNpc);

        var targetPoint = MoonUtils.CalculateHoverAbovePoint(Owner.Center, IdleTimer, Projectile.minionPos);
        MoonUtils.AI_FloatAbove(Projectile.Center, ref Projectile.velocity, targetPoint);

        IdleTimer++;
        if (IdleTimer >= 90 && _targeter.HasValidTarget)
        {
            SwitchState(AIState.SeekTarget);
        }

        var sqrDistToOwner = Vector2.DistanceSquared(Owner.Center, Projectile.Center);
        if (sqrDistToOwner > HomeRange * 1.2f)
        {
            SwitchState(AIState.GoHome);
        }
    }

    void AI_Attack()
    {
        IdleTimer = 0;
        Timer++;

        Vector2 targetHoverPos = _targeter.Target.Center - new Vector2(0, 48);
        targetHoverPos.X += MathF.Sin(Timer) * 32;

        Vector2 targetVelocity = (targetHoverPos - Projectile.Center) * 0.05f;
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.1f);
        if (Timer > 30 && Timer % 20 == 0)
        {
            int Sound = Main.rand.Next(1, 3);
            SoundStyle mySound = new SoundStyle("Stellamod/Assets/Sounds/ArcharilitDrone1");
            if (Sound == 1)
            {
                mySound = new SoundStyle("Stellamod/Assets/Sounds/ArcharilitDrone1");
            }
            else
            {
                mySound = new SoundStyle("Stellamod/Assets/Sounds/ArcharilitDrone2");
            }

            mySound.PitchVariance = 0.2f;
            mySound.Volume = 0.46f;
            SoundEngine.PlaySound(mySound, Projectile.position);
            Vector2 fireVelocity = (_targeter.Target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12;
            Projectile.velocity -= fireVelocity * 0.5f;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireVelocity,
                    ModContent.ProjectileType<ArchariliteArrowSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 0);
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(35));
                Projectile.netUpdate = true;
            }
        }
        if (Timer > 120)
        {
            SwitchState(AIState.GoHome);
            Timer = 0;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }


    void AI_GoHome()
    {
        Timer++;
        var targetPoint = MoonUtils.CalculateHoverAbovePoint(Owner.Center, IdleTimer, Projectile.minionPos);
        MoonUtils.AIWalk_FloatingChaseRhapsody(_pathfinder, Projectile, targetPoint, RunSpeed, ref _targeter.targetOldPos);
        var sqrDst = Vector2.DistanceSquared(Projectile.Center, targetPoint);
        if (sqrDst <= HomeRange)
        {
            SwitchState(AIState.Idle);
        }
    }


    void AI_SeekTarget()
    {
        Timer++;
        MoonUtils.AIWalk_FloatingChaseRhapsody(_pathfinder, Projectile, _targeter.Target.Center, RunSpeed, ref _targeter.targetOldPos);
        MoonUtils.SearchForNewTargetByLineOfSight(Owner.Center, Projectile.Center, ref _targeter.targetNpc);
        if (_targeter.targetNpc != -1 &&
            Collision.CanHitLine(Projectile.position, 1, 1, _targeter.Target.position, 1, 1))
        {
            SwitchState(AIState.Attack);
        }
        if (!_targeter.Target.active)
            SwitchState(AIState.Idle);
    }

    private void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        Projectile.netUpdate = true;
    }

    public override void AI()
    {
        base.AI();
        Projectile.spriteDirection = Projectile.direction;
        if (Main.rand.NextBool(16))
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Projectile.velocity * 0.1f, 0, Color.OrangeRed, 1f).noGravity = true;
        }

        var dstHome = Vector2.DistanceSquared(Owner.Center, Projectile.Center);
        if (dstHome > SqrDistHome)
        {
            var posToGoTo = Owner.Center + new Vector2(0, -64);
            var targetVelocity = posToGoTo - Projectile.Center;
            Projectile.velocity = targetVelocity * 0.1f;
            Projectile.tileCollide = false;
            return;
        }
        else
        {
            Projectile.tileCollide = true;
        }

        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Attack:
                AI_Attack();
                break;
            case AIState.GoHome:
                AI_GoHome();
                break;
            case AIState.SeekTarget:
                AI_SeekTarget();
                break;
        }

        Projectile.rotation = Projectile.velocity.X * 0.035f;
        DrawHelper.AnimateTopToBottom(Projectile, 4);
    }
}