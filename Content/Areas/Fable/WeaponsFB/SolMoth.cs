using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Astar;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class SolMoth : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToBellMinion(ModContent.ProjectileType<SolMothMinionProj>());
        Item.damage = 27;
        Item.knockBack = 3f;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankRune>(),
            material: ModContent.ItemType<AlcadizScrap>());
    }
}



public class SolMothMinionProj : AbstractBellSummon,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        Chase,
        Attack,
        GoHome
    }

    Targeter _targeter;
    Pathfinder _pathfinder;
    ref float Timer => ref Projectile.ai[0];
    ref float TimerOffset => ref Projectile.ai[1];
    AIState State
    {
        get => (AIState)Projectile.ai[2];
        set => Projectile.ai[2] = (float)value;
    }

    float HomeSqrDistance => 252 * 252;
    float RunSpeed => 12;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
        Projectile.StaticDefaultToMinionProjectile();
    }

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

    public sealed override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        Projectile.DefaultToMinionProjectile();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.LocalPiercingImmunityTime = 20;
        Projectile.light = 0.6f;
    }

    public override bool? CanCutTiles()
    {
        return false;
    }

    public override bool MinionContactDamage()
    {
        return State == AIState.Attack;
    }

    void DrawGlow(SpriteBatch sb, Vector2 sp)
    {
        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Projectile.Center);
        glowDrawer.color = Color.Lerp(Color.DarkOrange * 0.5f, Color.Gold * 0.5f, ExtraMath.Osc(0f, 1f, speed: 3));
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.3f;
        sb.Draw(glowDrawer);
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        Projectile.netUpdate = true;
    }

    void AI_Attack()
    {
        if (!_targeter.HasValidTarget)
        {
            SwitchState(AIState.GoHome);
            return;
        }
        Timer++;
        if (Timer == 1)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon");
            soundStyle.PitchVariance = 0.15f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(_targeter.Target.Center, DustID.GoldFlame, (Vector2.One * Main.rand.Next(1, 5))
                    .RotatedByRandom(19.0), 0, Color.White, 1f).noGravity = true;
            }
        }

        if (Timer < 10)
        {
            Projectile.velocity *= 0.92f;
        }

        if (Timer == 10)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                TimerOffset = Main.rand.Next(0, 30);
                Projectile.netUpdate = true;
            }

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2");
            soundStyle.PitchVariance = 0.15f;
            soundStyle.Volume = 0.5f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            Dust.QuickDustLine(Projectile.Center, _targeter.Target.Center, 50, Color.Goldenrod);

            var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, _targeter.Target.Center, TriColorPalette.Fiery, new Vector2(5, 15f));
            factory.particleCount = 8;
            ParticleUtils.CreateSwirlingDustCircle(factory);

            FXUtil.GlowCircleBoom(_targeter.Target.Center, Color.Yellow, Color.OrangeRed, Color.DarkRed, duration: 18, baseSize: 0.17f);

            Projectile.velocity = (_targeter.Target.Center - Projectile.Center).Resize(15);
            Projectile.Center = _targeter.Target.Center;
        }
        else if (Timer < 30)
        {
            Projectile.velocity *= 0.98f;
        }
        else
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(0.05f * MathF.Sign(Projectile.velocity.X));
            Projectile.velocity *= 0.98f;
        }

        if (Timer >= 60 + TimerOffset)
        {
            Timer = 0;
            SwitchState(AIState.GoHome);
        }
    }

    void AI_GoHome()
    {
        Timer++;
        MoonUtils.AIWalk_FloatingChaseRhapsody(_pathfinder, Projectile, Owner.Center + new Vector2(0, -16), RunSpeed, ref _targeter.targetOldPos);
        var sqrDist = Vector2.DistanceSquared(Owner.Center, Projectile.Center);
        if (sqrDist < HomeSqrDistance * 0.9f)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_Chase()
    {
        Timer++;
        if (Collision.CanHitLine(Projectile.position, 1, 1, _targeter.Target.position, 1, 1))
        {
            SwitchState(AIState.Attack);
            return;
        }
        MoonUtils.AIWalk_FloatingChaseRhapsody(_pathfinder, Projectile, _targeter.Target.Center, RunSpeed, ref _targeter.targetOldPos);
        MoonUtils.SearchForNewTargetByLineOfSight(Owner.Center, Projectile.Center, ref _targeter.targetNpc);
        if (!_targeter.HasValidTarget)
        {
            SwitchState(AIState.GoHome);
        }
    }

    void AI_Idle()
    {
        Timer++;
        if (Timer >= 30)
            MoonUtils.SearchForNewTargetByDistance(Owner.Center, ref _targeter.targetNpc);

        var targetPoint = MoonUtils.CalculateHoverAbovePoint(Owner.Center, Timer, Projectile.minionPos);
        MoonUtils.AI_FloatAbove(Projectile.Center, ref Projectile.velocity, targetPoint);

        Timer++;
        if (Timer >= 90 && _targeter.HasValidTarget)
        {
            SwitchState(AIState.Chase);
        }

        var sqrDistToOwner = Vector2.DistanceSquared(Owner.Center, Projectile.Center);
        if (sqrDistToOwner > HomeSqrDistance)
        {
            SwitchState(AIState.GoHome);
        }
    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.Attack:
                AI_Attack();
                break;
            case AIState.GoHome:
                AI_GoHome();
                break;
        }

        // So it will lean slightly towards the direction it's moving
        Projectile.rotation = Projectile.velocity.X * 0.05f;
        DrawHelper.AnimateTopToBottom(Projectile, 8);
        // Some visuals here
        Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1f * Main.essScale);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawGlow);
    }
}