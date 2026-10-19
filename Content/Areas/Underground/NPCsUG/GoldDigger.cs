using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Particles;
using Stellamod.Core.Astar;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.NPCsUG;

public class GoldDigger : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        Patrol,
        ChaseYou,
        ChaseCoin,
        FleeAway,
        SwipeYou
    }

    Pathfinder _pathfinder;
    float _walkDirection;
    Vector2 _dashDirection;
    bool _contactDamage;
    Outliner _outliner;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float ChangeDirectionTimer => ref NPC.ai[2];

    float _targetAfAlpha;
    float _afAlpha;
    float _patrolTime;
    Vector2 PatrolTimeRange => new Vector2(300, 360);
    float CoinChaseRange => 1024 * 1024;
    float ChaseRange => 128 * 128;
    float AttackRange => 64 * 64;

    float RunSpeed => 6;
    float MaxJumpSpeed => 6;
    float DashPrepTime => 20;
    float DashTime => 45;
    float DashSpeed => 14;
    float IdleTime => 60;
    public override string Texture => TextureRegistry.EmptyTexture;
    const string ANIM_IDLE = "Idle";
    const string ANIM_RUN = "Run";
    const string ANIM_SWIPE = "Swipe";
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        _pathfinder.NetSend(writer);
        writer.WriteVector2(_dashDirection);
        writer.Write(_patrolTime);
        writer.Write(_walkDirection);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _pathfinder.NetReceive(reader);
        _dashDirection = reader.ReadVector2();
        _patrolTime = reader.ReadSingle();
        _walkDirection = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPC.SetTrailCacheLength(8);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        NPC.damage = 17;
        NPC.width = 34;
        NPC.height = 26;
        NPC.lifeMax = 80;
        NPC.defense = 6;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = false;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    public override void AI()
    {
        base.AI();

        _outliner.SetDefaults();
        _contactDamage = false;
        _targetAfAlpha = 0f;
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.ChaseYou:
                AI_ChaseYou();
                break;
            case AIState.FleeAway:
                AI_FleeAway();
                break;
            case AIState.ChaseCoin:
                AI_ChaseCoin();
                break;
            case AIState.Patrol:
                AI_Patrol();
                break;
            case AIState.SwipeYou:
                AI_SwipeYou();
                break;
        }
        PickupNearbyCoins();
        _afAlpha = MathHelper.Lerp(_afAlpha, _targetAfAlpha, 0.1f);
        _outliner.Update();
        NPC.DrawOrigin = new Vector2(34, 32);
    }

    Item GetNearestCoin()
    {
        Item nearest = null;
        var dist = float.MaxValue;
        foreach(var item in Main.ActiveItems)
        {
            if (!item.IsACoin || item.noGrabDelay > 0)
                continue;
            var distToItem = Vector2.DistanceSquared(NPC.Center, item.Center);
            if (distToItem > CoinChaseRange)
                continue;

            if(distToItem < dist)
            {
                dist = distToItem;
                nearest = item;
            }
        }
        return nearest;
    }

    void PickupNearbyCoins()
    {
        var rect = NPC.getRect();
        foreach(var item in Main.ActiveItems)
        {
            var itemRect = item.getRect();
            if (rect.Intersects(itemRect))
            {
                if (item.IsACoin && item.noGrabDelay <= 0)
                {
                    NPC.extraValue += (int)PlayerHelper.GetCoinValue(item);
                    item.active = false;
                    if(Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
                    }
                }
         
            }
        }
    }
    void AI_Idle()
    {
        Timer++;
        NPC.velocity.X *= 0.92f;
        NPC.PlayAnimationLooped(ANIM_IDLE);
        if(Timer >= IdleTime)
        {
            SwitchState(AIState.Patrol);
        }
    }

    void AI_Patrol()
    {
        Timer++;
        if(Timer == 1 && MultiplayerHelper.IsHost)
        {
            _walkDirection = Main.rand.NextBool(2) ? -1 : 1;
            _patrolTime = Main.rand.NextFloat(PatrolTimeRange.X, PatrolTimeRange.Y);
            NPC.netUpdate = true;
        }

        NPC.TargetClosest(false);
        NPC.QuickStepUp();
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, _walkDirection, 0.1f);
        NPC.PlayAnimationLooped(ANIM_RUN);
        FaceVelocity();
        if(ChangeDirectionTimer > 0)
        {
            ChangeDirectionTimer--;
        }
        else
        {
            if (NPC.collideX)
            {
                _walkDirection *= -1;
                ChangeDirectionTimer = 30;
            }
        }

        var nearestCoin = GetNearestCoin();
        if(nearestCoin != null)
        {
            SwitchState(AIState.ChaseCoin);
        }

        var targetingPlayer = NPC.PlayerImTargeting;
        var sqrDistanceToPlayer = Vector2.DistanceSquared(NPC.Center, targetingPlayer.Center);
        if(sqrDistanceToPlayer < ChaseRange)
        {
            if (NPC.extraValue <= 0)
                SwitchState(AIState.ChaseYou);
            else
                SwitchState(AIState.FleeAway);
        }

        if(Timer >= _patrolTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_FleeAway()
    {
        Timer++;

        var targetingPlayer = NPC.PlayerImTargeting;
        var sqrDistanceToPlayer = Vector2.DistanceSquared(NPC.Center, targetingPlayer.Center);
        if (sqrDistanceToPlayer > ChaseRange)
        {
            SwitchState(AIState.Patrol);
        }
        else
        {
            var dirX = targetingPlayer.Center.X < NPC.Center.X ? 1 : -1;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, dirX * RunSpeed, 0.2f);
            NPC.QuickStepUp();
        }

        FaceVelocity();
        NPC.PlayAnimationLooped(ANIM_RUN);

    }
    void AI_ChaseCoin()
    {
        Timer++;
        var nearestCoin = GetNearestCoin();
        if(nearestCoin == null)
        {
            SwitchState(AIState.Idle);
            return;
        }

        MoonUtils.CreateNewPathIfTooFar(_pathfinder, NPC.Center, nearestCoin.Center);
        MoonUtils.AIWalk_SusBush(_pathfinder, NPC, nearestCoin.Center, MoonUtils.IsGrounded(NPC), RunSpeed, MaxJumpSpeed);
        FaceVelocity();
        NPC.PlayAnimationLooped(ANIM_RUN);
    }
    void AI_ChaseYou()
    {
        Timer++;
        MoonUtils.CreateNewPathIfTooFar(_pathfinder, NPC.Center, NPC.PlayerImTargeting.Center);
        MoonUtils.AIWalk_SusBush(_pathfinder, NPC, NPC.PlayerImTargeting.Center, MoonUtils.IsGrounded(NPC), RunSpeed, MaxJumpSpeed);
        FaceVelocity();
        NPC.PlayAnimationLooped(ANIM_RUN);

        var targetingPlayer = NPC.PlayerImTargeting;
        var sqrDistanceToPlayer = Vector2.DistanceSquared(NPC.Center, targetingPlayer.Center);
        if (sqrDistanceToPlayer < AttackRange)
        {
            SwitchState(AIState.SwipeYou);
        }
    }
    void AI_SwipeYou()
    {
        Timer++;
        if(Timer == 1)
        {
            _dashDirection = (NPC.PlayerImTargeting.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            NPC.netUpdate = true;
        }

        NPC.PlayAnimationOneShot(ANIM_SWIPE);

        NPC.spriteDirection = _dashDirection.X < 0 ? -1 : 1;
        NPC.direction = NPC.spriteDirection;
        if (Timer < DashPrepTime)
        {
            _outliner.warning = true;
            NPC.velocity = Vector2.Lerp(NPC.velocity, -_dashDirection, 0.1f);
        }

        if(Timer == DashPrepTime)
        {
            var quickSlash = AssetReferences.Assets.Sounds.Melee.SlashQuick2.Asset with { PitchVariance = 0.4f };
            SoundEngine.PlaySound(quickSlash, NPC.position);
            var targetVelocity = _dashDirection * DashSpeed;
            NPC.velocity = targetVelocity;
        }

        if (Timer > DashPrepTime )
        {
            if(Timer < DashPrepTime + DashTime * 0.5f)
            {
                _targetAfAlpha = 1f;
                _outliner.attacking = true;
                _contactDamage = true;
            }
     
            NPC.velocity *= 0.91f;
        }

        if(Timer > DashPrepTime + DashTime)
        {
            var nearestCoin = GetNearestCoin();
            if (nearestCoin != null)
                SwitchState(AIState.ChaseCoin);
            else
                SwitchState(AIState.Idle);
        }
    }


    void FaceVelocity()
    {
        NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
        NPC.spriteDirection = NPC.direction;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        base.OnHitPlayer(target, hurtInfo);
        var coinDropSound = AssetReferences.Assets.Sounds.CoinDrop.Asset with { PitchVariance = 0.4f };
        SoundEngine.PlaySound(coinDropSound, NPC.position);
        PlayerHelper.DropCoins(target, 4);

        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(NPC.Center, target.Center, TriColorPalette.Fiery, new Vector2(5, 15));
        ParticleUtils.CreateSwirlingDustBurst(fac);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if (NPC.life > 0)
            return;

        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(0, -1));
        this.MakeGore("_Gore_Sack", NPC.position, NPC.velocity + new Vector2(-1, 0));
        this.MakeGore("_Gore_Body", NPC.position, NPC.velocity + new Vector2(0, 1));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if(_afAlpha > 0.02f)
        {
            for (var i = 0; i < NPC.oldPos.Length; i++)
            {
                var pos = NPC.oldPos[i] + NPC.Size * 0.5f;
                var ratio = (float)i / (float)NPC.oldPos.Length;
                var color = Color.Lerp(Color.Gold, Color.Transparent, ratio) * 0.2f * _afAlpha;
                NPC.DrawAnimator(spriteBatch, color, pos);
            }
        }

        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
