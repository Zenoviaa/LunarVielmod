using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine;


public class Bishinine : ScarletBoss
{

    /*
    * A bell drops to the ground in front of her and she hits it at you, running behind you and hitting back at you, increasing speed 


    Throws the hammerscythe at the ceiling and a bunch of bells fall and bounce on the ground 


    She points her finger up and a humongous growing bell appears and she throws it at you 
    as it bounces from wall to wall as she is balancing on it like lenny from mario bros (second phase attack)


    She runs over to you and does a jump and spike attack making a bunch of ghastly spikes poke from the ground
    (grimm poking into the ground attack basically)

    Holds her scythe and crosses the ground with fast dashes.


    Jumps backwards and charges in the air before she shoots a shotgun of magic missiles 


    She jumps in the air and floats as a bunch of a comets fall onto the ground with like an electric like impact, 
    this happens for a decent bit (second phase), Signature attack 


    Second phase she just becomes faster

    */

    private Projectile _bigBellProjectile;
    private BellBaseball _baseballProjectile;
    private Animator _animator;
    private float _afterImageTime;
    private float _starTrailTime;
    private bool _fall;
    private bool _hammerRise;
    private bool _contactDamage;
    private bool _enabledPhase2Attacks;
    private bool _hasHammer = true;
    private bool _black;

    private AIState _nextState;

    private float _squishTimer;
    private Vector2 _startSquishScale = Vector2.One;
    private Vector2 _squishScale = Vector2.One;
    private Vector2 _deathCenter;
    private Vector2 _teleportCenter;
    private Color _outlineColor;
    private Color TargetOutlineColor;
    private PatternManager<AIState> _patternManager;
    private ref float Timer => ref NPC.ai[0];

    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float AttackTimer => ref NPC.ai[2];
    private ref float AttackNumber => ref NPC.ai[3];
    private enum AIState
    {
        Spawn,
        Idle,

        BellDrop_Start,
        BellDrop_RunToBell,
        BellDrop_Hit,
        BellDrop_End,

        BellFall_Start,
        BellFall_ThrowScythe,

        BellRoll_Start,
        BellRoll_Bounce,
        BellRoll_End,

        GrimmSpikes_RunToPlayer,
        GrimmSpikes_Jump,
        GrimmSpikes_Crash,

        ScytheDash_Startup,
        ScytheDash_Dash,
        ScytheDash_End,

        MagicMissle_Startup,
        MagicMissle_Barrage,
        MagicMissle_End,

        CometJump_Startup,
        CometJump_Float,
        CometJump_End,

        BouncingScytheStartup,
        BouncingScytheThrow,
        BouncingScytheEnd,

        Phase2Transition,
        Despawn,
        Death,
        HammerDrop,
        CorrectSelf,

    }
    private bool InPhase2
    {
        get => NPC.life <= NPC.lifeMax / 2f;
    }

    private int RisingScytheDamage => 40;
    private int GrimmSpikesDamage => 60;
    private int MagicMissileDamage => 60;
    private int CometDamage => 66;
    private int BellBalancingBounceDamage => 60;
    private int BouncingScytheDamage => 25;
    private int BaseballDamage => 25;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_fall);
        writer.Write(_contactDamage);
        writer.Write(_hammerRise);
        writer.Write((int)_nextState);
        writer.Write(_hasHammer);
        writer.WriteVector2(_deathCenter);
        writer.WriteVector2(_teleportCenter);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _fall = reader.ReadBoolean();
        _contactDamage = reader.ReadBoolean();
        _hammerRise = reader.ReadBoolean();
        _nextState = (AIState)reader.ReadInt32();
        _hasHammer = reader.ReadBoolean();
        _deathCenter = reader.ReadVector2();
        _teleportCenter = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        Main.npcFrameCount[NPC.type] = 83;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _squishScale = Vector2.One;
        NPC.width = 32;
        NPC.height = 70;
        NPC.damage = 60;
        NPC.defense = 15;
        NPC.lifeMax = 23000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
        NPC.knockBackResist = 0f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.value = Item.buyPrice(gold: 1);
        NPC.boss = true;
        NPC.npcSlots = 10f;
        NPC.takenDamageMultiplier = 0.9f;
        NPC.aiStyle = -1;
        Music = MusicLoader.GetMusicSlot("Stellamod/Assets/Music/Bishinine");
    }

    public override bool? CanFallThroughPlatforms()
    {
        return _fall;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }


    #region Animations
    private const string Anim_Idle = "idle";
    private const string Anim_Run = "run";
    private const string Anim_JumpStartup = "jumpstartup";
    private const string Anim_Jump = "jump";
    private const string Anim_Fall = "fall";
    private const string Anim_Land = "land";
    private const string Anim_HoldHammer = "holdhammer";
    private const string Anim_Hitbell = "hitbell";
    private const string Anim_SpinTeleportOut = "spinteleportout";
    private const string Anim_FingerUp = "fingerup";
    private const string Anim_ThrowBigBall = "throw";
    private const string Anim_HammerDrop = "hammerdrop";
    private const string Anim_Spinning = "spinning";
    private const string Anim_45 = "45";
    private const string Anim_HammerRise = "hammerrise";
    private const string Anim_ThrowBigBallReverse = "throwreverse";
    private const string Anim_FingerUpReverse = "fingerupreverse";
    private const string Anim_SpinningFast = "spinningfast";
    private const string Anim_HammerlessIdle = "hammerlessidle";
    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(frameHeight);
    }

    private Animator Animator
    {
        get
        {
            if (_animator == null)
                SetupAnimator();
            return _animator!;
        }
    }

    private bool DoesAttackUseHammer(AIState state)
    {
        switch (state)
        {
            default:
                return false;
            case AIState.BellDrop_Start:
                return true;
            case AIState.BouncingScytheStartup:
                return true;
            case AIState.BellFall_Start:
                return true;
        }
    }
    private void SetupAnimator()
    {
        _animator = new Animator();
        Vector2 animationDrawOrigin = new Vector2(45, 58);
        var idle = new SpriteAnimation(0, 4, isLooping: true, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Idle, idle);

        var running = new SpriteAnimation(7, 15, isLooping: true, drawOriginOverride: animationDrawOrigin, frameSpeed: 0.35f);
        _animator.AddAnimation(Anim_Run, running);

        var jumpStartup = new SpriteAnimation(16, 18, isLooping: false, drawOriginOverride: new Vector2(53, 57), frameSpeed: 0.15f);
        _animator.AddAnimation(Anim_JumpStartup, jumpStartup);

        var jump = new SpriteAnimation(19, 19, isLooping: true, drawOriginOverride: new Vector2(53, 57));
        _animator.AddAnimation(Anim_Jump, jump);

        var fall = new SpriteAnimation(20, 23, isLooping: true, drawOriginOverride: new Vector2(53, 57));
        _animator.AddAnimation(Anim_Fall, fall);

        var land = new SpriteAnimation(24, 24, isLooping: true, drawOriginOverride: new Vector2(53, 57));
        _animator.AddAnimation(Anim_Land, land);

        var hold = new SpriteAnimation(25, 26, isLooping: true, drawOriginOverride: new Vector2(22, 52), frameSpeed: 0.25f);
        _animator.AddAnimation(Anim_HoldHammer, hold);

        var hitbell = new SpriteAnimation(27, 33, isLooping: false, drawOriginOverride: new Vector2(22, 52), frameSpeed: 0.25f);
        _animator.AddAnimation(Anim_Hitbell, hitbell);

        var teleportOut = new SpriteAnimation(34, 43, isLooping: false, drawOriginOverride: new Vector2(22, 52), frameSpeed: 0.25f);
        _animator.AddAnimation(Anim_SpinTeleportOut, teleportOut);

        var fingerUp = new SpriteAnimation(44, 53, isLooping: false, drawOriginOverride: new Vector2(53, 57));
        _animator.AddAnimation(Anim_FingerUp, fingerUp);

        var throwBigBall = new SpriteAnimation(55, 61, isLooping: false, drawOriginOverride: new Vector2(53, 57));
        _animator.AddAnimation(Anim_ThrowBigBall, throwBigBall);

        var hammer = new SpriteAnimation(62, 69, isLooping: false, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_HammerDrop, hammer);

        var spin = new SpriteAnimation(70, 77, isLooping: true, drawOriginOverride: new Vector2(49, 62), frameSpeed: 0.25f);
        _animator.AddAnimation(Anim_Spinning, spin);


        var idle2 = new SpriteAnimation(44, 44, isLooping: true, drawOriginOverride: new Vector2(53, 57));
        _animator.AddAnimation(Anim_45, idle2);

        var hammerRise = new SpriteAnimation(62, 69, isLooping: false, drawOriginOverride: animationDrawOrigin, frameSpeed: 0.25f);
        hammerRise.reverse = true;
        _animator.AddAnimation(Anim_HammerRise, hammerRise);

        var fingerUpReverse = new SpriteAnimation(44, 53, isLooping: false, drawOriginOverride: new Vector2(53, 57));
        fingerUpReverse.reverse = true;
        _animator.AddAnimation(Anim_FingerUpReverse, fingerUpReverse);

        var throwBigBallReverse = new SpriteAnimation(55, 61, isLooping: false, drawOriginOverride: new Vector2(53, 57));
        throwBigBallReverse.reverse = true;
        _animator.AddAnimation(Anim_ThrowBigBallReverse, throwBigBallReverse);

        var spinfast = new SpriteAnimation(70, 77, isLooping: true, drawOriginOverride: new Vector2(49, 62), frameSpeed: 0.75f);
        _animator.AddAnimation(Anim_SpinningFast, spinfast);

        var baseball = new SpriteAnimation(78, 82, isLooping: true, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_HammerlessIdle, baseball);

        /*
        var land = new SpriteAnimation(24, 24, isLooping: true, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Land, land);*/
    }
    #endregion

    #region Squishing
    private void LandingSquish()
    {
        _squishTimer = 0;
        _startSquishScale = new Vector2(1.4f, 0.65f);
        _squishScale = new Vector2(1.4f, 0.65f);
    }

    private void UnSquish()
    {
        const float time = 30f;
        _squishTimer++;
        float completionRatio = _squishTimer / time;
        float ease = EasingFunction.InOutSine(completionRatio);
        _squishScale = Vector2.Lerp(_startSquishScale, Vector2.One, ease);
    }
    #endregion

    private bool CountsAsWall(Point tilePosition)
    {
        if (!WorldGen.InWorld(tilePosition.X, tilePosition.Y))
            return false;
        Tile tile = Main.tile[tilePosition];
        if (!Main.tileSolid[tile.TileType])
            return false;
        if (!tile.HasTile)
            return false;
        return true;
    }
    private bool IsInsideWalls()
    {
        Point tilePosition = NPC.Center.ToTileCoordinates();
        return CountsAsWall(tilePosition);
    }

    private bool IsAboutToHitWall()
    {
        Point currentTilePosition = NPC.Center.ToTileCoordinates();
        currentTilePosition.Y -= 1;
        int dir = MathF.Sign(NPC.velocity.X);
        for (int x = 0; x < 100; x++)
        {
            Point nextTilePosition = currentTilePosition;
            nextTilePosition.X += dir;
            if (!WorldGen.InWorld(nextTilePosition.X, nextTilePosition.Y))
                break;
            Tile tile = Main.tile[nextTilePosition];
            if (tile.HasTile && Main.tileSolid[tile.TileType])
                break;
            currentTilePosition = nextTilePosition;
        }

        Vector2 edge = currentTilePosition.ToWorldCoordinates();
        Vector2 checkPoint = NPC.Center + new Vector2(dir, 0) * 200;
        Vector2 dir1 = edge - NPC.Center;
        dir1 = dir1.SafeNormalize(Vector2.Zero);
        Vector2 dir2 = edge - checkPoint;
        dir2 = dir2.SafeNormalize(Vector2.Zero);
        return Vector2.Dot(dir1, dir2) < 0;
    }

    private void TeleportEffect(Vector2 position)
    {
        SoundStyle sound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Bishinine.BishinineSound1 : AssetRegistry.Sounds.Bishinine.BishinineSound2;
        SoundEngine.PlaySound(sound, position);
        Vector2 pos = position;
        var part = FXUtil.GlowCircleBoom(pos,
                       innerColor: Color.White,
                       glowColor: Color.Blue,
                       outerGlowColor: Color.Black, duration: 12, baseSize: 0.14f);
        part.Scale *= 1;


        var part2 = FXUtil.GlowCircleBoom(pos,
              innerColor: Color.White,
              glowColor: Color.Blue,
              outerGlowColor: Color.Black, duration: 12, baseSize: 0.14f);
        part2.Scale *= 3;
        for (float f = 0; f < 32; f++)
        {
            Dust.NewDustPerfect(pos, DustID.Torch,
                (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
        }


        for (float i = 0; i < 15; i++)
        {
            float rot = rot = Main.rand.NextFloat(-2f, 2f);
            rot += Main.rand.NextFloat(-0.5f, 0.5f);

            Vector2 offset = rot.ToRotationVector2() * Main.rand.NextFloat(32, 64);
            Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(2, 15);
            var particle = FXUtil.GlowCircleDetailedBoom1(pos + offset,
                innerColor: Color.White,
                glowColor: Color.Blue,
                outerGlowColor: Color.Black,
                baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                duration: Main.rand.NextFloat(5, 25));
            particle.Velocity = velocity;
            particle.Scale *= 0.35f;
            particle.Rotation = rot;
        }
    }
    private void AI_CorrectSelf()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.TargetClosest();

            if (MultiplayerHelper.IsHost)
            {
                Vector2 targetCenter = MyTarget.Center;
                targetCenter.Y -= 64;
                _teleportCenter = targetCenter;
                NPC.netUpdate = true;
            }
            TeleportEffect(NPC.Center);
        }
        if(Timer == 3)
        {
            TeleportEffect(NPC.Center);
        }

        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.velocity.X *= 0.92f;
        NPC.rotation *= 0.92f;
        Animator.PlayAnimation(Anim_Spinning);
        if(Timer >= 60)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override void AI()
    {
        base.AI();
        _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
            {
                SwitchState(AIState.Despawn);
            }
        }

        if (InPhase2 && !_enabledPhase2Attacks && State != AIState.CometJump_Startup)
        {
            _contactDamage = false;
            NPC.velocity.X = 0;
            NPC.velocity.Y = 0;
            AttackNumber = 0;
            _patternManager = null;
            _enabledPhase2Attacks = true;
            var part = FXUtil.GlowCircleBoom(NPC.Center,
                innerColor: Color.White,
                glowColor: Color.Blue,
                outerGlowColor: Color.Black, duration: 12, baseSize: 0.14f);
            part.Scale *= 1;
            SoundStyle laughSound = AssetRegistry.Sounds.Bishinine.Bishininelaugh;
            SoundEngine.PlaySound(laughSound, NPC.position);
            SwitchState(AIState.CometJump_Startup);
        }

        if (_teleportCenter != Vector2.Zero)
        {
            NPC.position.X = _teleportCenter.X - NPC.Size.X / 2;
            NPC.position.Y = _teleportCenter.Y - NPC.Size.Y / 2;
            NPC.velocity.X = 0f;
            NPC.velocity.Y = 0f;
            _teleportCenter = Vector2.Zero;

        }
        NPC.spriteDirection = NPC.direction;
        if (NPC.collideY && NPC.velocity.Y > 1)
        {
            LandingSquish();
        }
        UnSquish();
        switch (State)
        {
            case AIState.CorrectSelf:
                AI_CorrectSelf();
                break;
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Spawn:
                AI_Spawn();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Death:
                AI_Death();
                break;


            case AIState.BellDrop_Start:
                AI_BellDropStart();
                break;
            case AIState.BellDrop_RunToBell:
                AI_BellDropRunToBell();
                break;
            case AIState.BellDrop_Hit:
                AI_BellDropHit();
                break;
            case AIState.BellDrop_End:
                AI_BellDropEnd();
                break;



            case AIState.BellFall_Start:
                AI_ThrowScytheStartup();
                break;
            case AIState.BellFall_ThrowScythe:
                AI_ThrowScythe();
                break;



            case AIState.GrimmSpikes_RunToPlayer:
                AI_GrimChasePlayer();
                break;
            case AIState.GrimmSpikes_Jump:
                AI_GrimSpikesJump();
                break;
            case AIState.GrimmSpikes_Crash:
                AI_GrimSpikesCrash();
                break;

            case AIState.ScytheDash_Startup:
                AI_ScytheDashStartup();
                break;
            case AIState.ScytheDash_Dash:
                AI_ScytheDashDash();
                break;
            case AIState.ScytheDash_End:
                AI_ScytheDashEnd();
                break;

            case AIState.CometJump_Startup:
                AI_CometJumpStartup();
                break;
            case AIState.CometJump_Float:
                AI_CometJumpFloat();
                break;
            case AIState.CometJump_End:
                AI_CometJumpEnd();
                break;

            case AIState.MagicMissle_Startup:
                AI_MagicMissileStartup();
                break;
            case AIState.MagicMissle_Barrage:
                AI_MagicMissileBarrage();
                break;
            case AIState.MagicMissle_End:
                AI_MagicMissileEnd();
                break;

            case AIState.BellRoll_Start:
                AI_BellRollStart();
                break;
            case AIState.BellRoll_Bounce:
                AI_BellRollBounce();
                break;
            case AIState.BellRoll_End:
                AI_BellRollEnd();
                break;

            case AIState.BouncingScytheStartup:
                AI_BouncingScytheStartup();
                break;
            case AIState.BouncingScytheThrow:
                AI_BouncingScytheThrow();
                break;
            case AIState.BouncingScytheEnd:
                AI_BouncingScytheEnd();
                break;
            case AIState.HammerDrop:
                AI_HammerDrop();
                break;
        }
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }


    private void AI_HammerDrop()
    {
        Timer++;
        NPC.velocity.X *= 0.9f;
        NPC.rotation = NPC.velocity.X * 0.2f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        if (_hammerRise)
        {
            Animator.PlayAnimation(Anim_HammerRise);
        }
        else
        {
            Animator.PlayAnimation(Anim_HammerDrop);
        }

        if (Timer >= 30 && Animator.IsFinished())
        {
            _hammerRise = false;
            SwitchState(_nextState);
        }
    }


    #region Bouncing Scythe
    private void AI_BouncingScytheStartup()
    {
        Animator.PlayAnimation(Anim_HammerDrop);
        TargetOutlineColor = Color.Yellow;
        NPC.velocity.X *= 0.94f;
        NPC.rotation = 0;
        //Throws the hammerscythe at the ceiling and a bunch of bells fall and bounce on the ground
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            NPC.velocity.Y = -2;
            var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
            SoundStyle laugh = AssetRegistry.Sounds.Bishinine.Bishininelaugh;
            SoundEngine.PlaySound(laugh, NPC.position);
        }

        if (Timer >= 30 && NPC.collideY)
        {
            SwitchState(AIState.BouncingScytheThrow);
        }
    }


    private void AI_BouncingScytheThrow()
    {
        Timer++;
        if (Timer == 1)
        {
            _hasHammer = false;
            NPC.TargetClosest();
            NPC.direction = TargetDirection;
            NPC.velocity.X = -NPC.direction * 5;

            SoundStyle laugh = AssetRegistry.Sounds.Bishinine.BishinineSound1;
            SoundEngine.PlaySound(laugh, NPC.position);
            if (MultiplayerHelper.IsHost)
            {
                Vector2 velocity = -Vector2.UnitY * 10;
                Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                    ModContent.ProjectileType<BouncingScythe>(), BouncingScytheDamage, 1, Main.myPlayer);
            }
        }
        switch (AttackNumber)
        {
            case 0:
                Animator.PlayAnimation(Anim_FingerUp);
                if (Animator.IsFinished())
                {
                    AttackNumber++;
                }
                break;
            case 1:
                Animator.PlayAnimation(Anim_ThrowBigBall);
                if (Animator.IsFinished())
                {
                    AttackNumber++;
                }
                break;
            case 2:
                Animator.PlayAnimation(Anim_ThrowBigBallReverse);
                if (Animator.IsFinished())
                {
                    AttackNumber++;
                }
                break;
            case 3:
                Animator.PlayAnimation(Anim_FingerUpReverse);
                if (Animator.IsFinished())
                {
                    AttackNumber++;
                }
                break;
            case 4:
                Animator.PlayAnimation(Anim_HammerlessIdle);
                break;
        }

        NPC.velocity.X *= 0.94f;
        NPC.rotation = NPC.velocity.X * 0.025f;
        if (Timer >= 220)
        {
            SwitchState(AIState.Idle);
        }
    }


    private void AI_BouncingScytheEnd()
    {

    }
    #endregion



    #region Bell Roll
    private void AI_BellRollStart()
    {
        /*
         *         
         * 
         *  She points her finger up and a humongous growing bell appears and she throws it at you 
            as it bounces from wall to wall as she is balancing on it like lenny from mario bros (second phase attack)

         */
        Animator.PlayAnimation(Anim_FingerUp);
        TargetOutlineColor = Color.Yellow;
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            NPC.direction = TargetDirection;
            if (MultiplayerHelper.IsHost)
            {
                _bigBellProjectile = Projectile.NewProjectileDirect(SourceFromThis, NPC.Top - new Vector2(0, 48), Vector2.Zero, ModContent.ProjectileType<BigBell>(), BellBalancingBounceDamage, 2, Main.myPlayer);
            }
        }
        NPC.velocity.X *= 0.94f;
        NPC.rotation = NPC.velocity.X * 0.05f;
        if (Timer % 70 == 0 && AttackNumber < 3)
        {
            if (MultiplayerHelper.IsHost)
            {
                AttackNumber++;
                _bigBellProjectile.ai[1] = 1;
                _bigBellProjectile.netUpdate = true;
            }

        }
        if (Timer >= 300)
        {
            SwitchState(AIState.BellRoll_Bounce);
        }
    }

    private void AI_BellRollBounce()
    {



        Timer++;
        if (Timer == 1)
        {
            SoundStyle sound = AssetRegistry.Sounds.Bishinine.Bishininelaugh;
            SoundEngine.PlaySound(sound, NPC.position);
        }
        if (Timer == 25)
        {
            if (MultiplayerHelper.IsHost)
            {
                _bigBellProjectile.ai[1] = 2;
                _bigBellProjectile.netUpdate = true;
            }
        }
        if (Timer < 25)
        {
            Animator.PlayAnimation(Anim_ThrowBigBall);
        }

        if (Timer > 125)
        {
            Animator.PlayAnimation(Anim_ThrowBigBallReverse);
        }

        //The projectile will control her movement here.
        //It'll lerp her position, hence why it needs a reference to her existence
        if (Timer >= 360)
        {
            SwitchState(AIState.BellRoll_End);
        }
    }

    private void AI_BellRollEnd()
    {
        Animator.PlayAnimation(Anim_FingerUpReverse);
        Timer++;
        if (Timer >= 60)
        {
            SwitchState(AIState.Idle);
        }
    }
    #endregion



    #region Magic Missile

    private void AI_MagicMissileStartup()
    {
        //  Jumps backwards and charges in the air before she shoots a shotgun of magic missiles 

        TargetOutlineColor = Color.Yellow;
        Timer++;
        if (Timer == 10)
        {
            NPC.TargetClosest();
            NPC.direction = TargetDirection;
            Vector2 jumpVelocity = new Vector2();
            jumpVelocity.Y = -10;
            jumpVelocity.X = -FacingDirectionToTarget * 15;
            NPC.velocity = jumpVelocity;
        }

        if (Timer < 10)
        {
            Animator.PlayAnimation(Anim_JumpStartup);
        }

        if (Timer >= 10)
        {
            Animator.PlayAnimation(Anim_Jump);
        }
        if (Timer >= 20f)
        {
            NPC.velocity.X *= 0.94f;
        }
        //      NPC.rotation = NPC.velocity.X * 0.05f;
        if (Timer >= 40f)
        {
            SwitchState(AIState.MagicMissle_Barrage);
        }
    }

    private void AI_MagicMissileBarrage()
    {
        Animator.PlayAnimation(Anim_Spinning);
        TargetOutlineColor = Color.Red;
        Timer++;
        if (Timer == 1)
        {


        }
        if (Timer % 5 == 0)
        {
            var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.UnitY, newColor: Color.White);
            var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.UnitY * 5, newColor: Color.White);
            p2.Scale *= 0.5f;
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, NPC.Center, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f),
                    ModContent.ProjectileType<BisinineMissile>(), MagicMissileDamage, 1, Main.myPlayer);
            }

            if (InPhase2)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(4f, 12);
                    velocity = velocity.RotatedBy(-NPC.direction * 0.5f);
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                        ModContent.ProjectileType<BisinineMissile>(), MagicMissileDamage, 1, Main.myPlayer);
                }
            }
        }

        if (Timer < 35)
        {
            _afterImageTime = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 10f));
            NPC.velocity.X = MathHelper.Lerp(0f, NPC.direction * 25, EasingFunction.InOutSine(Timer / 10f));
            NPC.velocity.Y = 0;

        }
        NPC.rotation = NPC.velocity.X * 0.05f;
        if (Timer >= 35)
        {
            SwitchState(AIState.MagicMissle_End);
        }
    }

    private void AI_MagicMissileEnd()
    {
        if (!NPC.collideY)
        {
            Animator.PlayAnimation(Anim_Fall);
        }
        else
        {

        }

        _afterImageTime *= 0.9f;
        TargetOutlineColor = Color.Transparent;
        if (NPC.collideY)
        {
            if (Timer < 15)
            {
                Animator.PlayAnimation(Anim_Land);
            }
            else
            {
                Animator.PlayAnimation(Anim_45);
            }


            Timer++;
            if (Timer >= 30)
            {
                SwitchState(AIState.Idle);
            }
        }

        NPC.velocity.X *= 0.94f;
        NPC.rotation *= 0.94f;

    }
    #endregion



    #region Signature Comet Fall
    private void AI_CometJumpStartup()
    {
        NPC.noGravity = true;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type == ModContent.ProjectileType<BellBaseball>())
                proj.ai[2] = 1;
        }
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            NPC.direction = TargetDirection;
            NPC.velocity = Vector2.Zero;
        }
        if (Timer < 15)
        {
            Animator.PlayAnimation(Anim_JumpStartup);
        }
        if (Timer == 15)
        {
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(bellHit, NPC.position);
            NPC.velocity.Y = -14;
            float maxRads = MathHelper.ToRadians(45);
            var part = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.UnitY);
            for (float f = 0; f < 8; f++)
            {
                Vector2 vel = -Vector2.UnitY * 4;
                vel = vel.RotatedByRandom(maxRads);
                vel *= Main.rand.NextFloat(0.1f, 5);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowSparkleDust>(), vel, newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1.5f));
            }
        }
        if (Timer >= 15)
        {
            Animator.PlayAnimation(Anim_Jump);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction, 0.1f);
        }

        NPC.rotation = NPC.velocity.X * 0.05f;
        if (Timer >= 25)
        {
            SwitchState(AIState.CometJump_Float);
        }
    }

    private void AI_CometJumpFloat()
    {
        Animator.PlayAnimation(Anim_Spinning);
        OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -252);
        TargetOutlineColor = Color.Yellow;
        Timer++;
        NPC.velocity.X *= 0.99f;
        if (Timer >= 15 && Timer <= 25)
        {
            // NPC.velocity.Y *= 0.95f;
        }

        if (Timer % 20 == 0)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0f, 0.5f));
        }
        _afterImageTime = MathHelper.Lerp(0f, 0.5f, EasingFunction.InOutSine(Timer / 30f));
        NPC.direction = TargetDirection;

        float targetY = MyTarget.Center.Y - 252;
        if (NPC.Center.Y > targetY)
        {
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, -5, 0.05f);
        }
        else
        {
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 2, 0.05f);
        }

        if (Timer >= 64)
        {
            NPC.velocity.X += MathF.Sin(Timer * 0.1f) * 0.2f;

            float xDistance = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
            if (xDistance > 64)
            {
                NPC.velocity.X += TargetDirection * 0.1f;
            }
            NPC.noGravity = true;
        }

        NPC.rotation = NPC.velocity.X * 0.05f;
        if (Timer % 5 == 0)
        {
            if (MultiplayerHelper.IsHost)
            {
                Vector2 spawnPos = NPC.Center;
                spawnPos.Y -= 1000;
                spawnPos.X += Main.rand.NextFloat(-1000, 1000);
                Projectile.NewProjectile(SourceFromThis, spawnPos, Vector2.UnitY * 4, ModContent.ProjectileType<BishinineComet>(), CometDamage, 1, Main.myPlayer);
            }
            AttackNumber++;
        }
        if (AttackNumber >= 100)
        {
            SwitchState(AIState.CometJump_End);
        }
    }
    private void AI_CometJumpEnd()
    {
        if (NPC.collideY)
        {
            Animator.PlayAnimation(Anim_Land);
            Timer++;
            if (Timer == 1)
            {
                LandingSquish();
            }
            if (Timer >= 60)
            {
                SwitchState(AIState.Idle);
            }
        }
        else
        {
            Animator.PlayAnimation(Anim_Fall);
        }

        _afterImageTime *= 0.9f;

        NPC.noGravity = false;
        NPC.velocity.X *= 0.9f;
        NPC.rotation = NPC.velocity.X * 0.05f;
    }


    #endregion



    #region Scythe Dash

    private void AI_ScytheDashStartup()
    {

        _starTrailTime *= 0.8f;
        _afterImageTime *= 0.8f;
        TargetOutlineColor = Color.Yellow;
        //   Holds her scythe and crosses the ground with fast dashes.
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            NPC.direction = TargetDirection;
            SoundStyle bSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Bishinine.BishinineSound1 : AssetRegistry.Sounds.Bishinine.BishinineSound2;
            bSound.PitchVariance = 0.1f;
            SoundEngine.PlaySound(bSound, NPC.position);
        }

        if (Timer < 10)
        {
            Animator.PlayAnimation(Anim_JumpStartup);
        }
        if (Timer == 10)
        {
            NPC.velocity.X = -NPC.direction * 8;
            NPC.velocity.Y = -4;
            if (AttackNumber == 0)
            {
                NPC.velocity.Y = -8;
            }
        }

        if (Timer >= 10 && NPC.velocity.Y < 0)
        {
            Animator.PlayAnimation(Anim_Jump);
        }
        else if (Timer >= 10)
        {
            Animator.PlayAnimation(Anim_Fall);
        }
        NPC.velocity.X *= 0.94f;
        if (Timer >= 30 && NPC.collideY)
        {
            SwitchState(AIState.ScytheDash_Dash);
        }
        NPC.rotation = NPC.velocity.X * 0.015f;


    }

    private void BounceEffect()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * (i + 1);
            var donutParticle = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center + velocity.SafeNormalize(Vector2.Zero) * 32, velocity);
            donutParticle.Scale = MathHelper.Lerp(1f, 2f, (float)i / 4f);
        }
    }
    private void AI_ScytheDashDash()
    {
        Animator.PlayAnimation(Anim_SpinningFast);
        _contactDamage = true;
        _afterImageTime = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 5f));
        _starTrailTime = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 5f));
        TargetOutlineColor = Color.Red;
        Timer++;
        if (Timer == 1)
        {
            AttackNumber++;
            NPC.direction = TargetDirection;
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, NPC.Center - (NPC.direction * Vector2.UnitX * 40) - Vector2.UnitY * 320, Vector2.UnitY,
                    ModContent.ProjectileType<DashLightning>(), NPC.damage, 1, Main.myPlayer);
            }
        }

        if (IsAboutToHitWall())
        {
            BounceEffect();
            NPC.velocity.X *= -1;
        }

        if (NPC.collideX)
        {
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            NPC.direction = -NPC.direction;
        }
        if (Timer % 5 == 0)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0.2f, 1f));
        }
        if (Timer % 1 == 0)
        {
            var spark = LegacyParticle.NewParticle<SparkParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero);
            spark.outerColor = Color.Blue;
            spark.fadeToColor = Color.Black;
        }
        if (Timer % 1 == 0)
        {
            Dust.NewDustPerfect(NPC.Bottom, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: 0.5f, Velocity: Vector2.Zero);
        }

        if (Main.rand.NextBool(4))
        {
            var p = Particle<ThickSmokeParticle>.Spawn(NPC.Bottom, Vector2.Zero, Color.DarkGray);
        }
        if (Timer >= 10)
        {
            NPC.velocity.X *= 0.9f;
        }
        else
        {
            NPC.velocity.X = MathHelper.Lerp(0, 80 * NPC.direction, EasingFunction.InOutSine(Timer / 10f));

        }
        NPC.velocity.Y -= 0.5f;
        NPC.rotation = NPC.velocity.X * 0.0025f;
        if (Timer >= 10)
        {
            SwitchState(AIState.ScytheDash_End);
        }
    }

    private void AI_ScytheDashEnd()
    {
        _starTrailTime *= 0.8f;
        _afterImageTime *= 0.8f;
        TargetOutlineColor = Color.Transparent;
        Timer++;

        if (IsAboutToHitWall())
        {
            BounceEffect();
            NPC.velocity.X *= -1;
        }

        NPC.velocity.X *= 0.9f;
        NPC.rotation = NPC.velocity.X * 0.0025f;

        if (AttackNumber >= 8)
        {

            if (Timer >= 35)
            {
                NPC.velocity.X = 0;
                NPC.rotation = 0;
                SwitchState(AIState.Idle);
            }
        }
        else
        {

            if (Timer >= 5)
            {
                SwitchState(AIState.ScytheDash_Startup);
            }

        }
    }
    #endregion



    #region Grim Poking Attack

    private void AI_GrimChasePlayer()
    {

        _afterImageTime = MathHelper.Lerp(_afterImageTime, 1f, 0.2f);
        TargetOutlineColor = Color.Yellow;
        /*
         *     She runs over to you and does a jump and spike attack making a bunch of ghastly spikes poke from the ground
    (grimm poking into the ground attack basically)/
        */
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            SoundStyle bellHit = AssetRegistry.Sounds.Bishinine.Bishininelaugh;
            bellHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(bellHit, NPC.position);
        }
        Animator.PlayAnimation(Anim_Run);
        NPC.direction = TargetDirection;

        float side = AttackTimer % 2 == 0 ? 1 : -1;
        Vector2 targetCenter = MyTarget.Center;
        targetCenter.X += side * 32;

        float xDistance = MathF.Abs(targetCenter.X - NPC.Center.X);
        float yDistance = MathF.Abs(targetCenter.Y - NPC.Center.Y);
        float maxRunSpeed = 15;
        float accel = 1;
        if (NPC.collideX)
        {
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }

        if (xDistance > 90)
        {
            //Zoom zoom, we gotta run up to the bell
            if (NPC.Center.X < MyTarget.Center.X)
            {
                if (NPC.velocity.X < maxRunSpeed)
                {
                    NPC.velocity.X += accel;
                }
            }
            else if (NPC.Center.X > MyTarget.Center.X)
            {
                if (NPC.velocity.X > -maxRunSpeed)
                {
                    NPC.velocity.X -= accel;
                }
            }
        }
        else if (NPC.collideY)
        {
            SwitchState(AIState.GrimmSpikes_Jump);
        }

    }

    private void AI_GrimSpikesJump()
    {
        if (Timer < 30)
        {
            Animator.PlayAnimation(Anim_JumpStartup);
        }
        else
        {
            if (NPC.velocity.Y < 0)
            {
                Animator.PlayAnimation(Anim_Jump);
            }
            else
            {
                Animator.PlayAnimation(Anim_Fall);
            }

        }


        TargetOutlineColor = Color.Yellow;
        Timer++;
        if (Timer == 30)
        {
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(bellHit, NPC.position);
            NPC.velocity.Y = -17;
            var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
            var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY * 4);
            p2.Scale *= 0.5f;
        }

        if (Timer >= 45)
        {
            if (Timer <= 75)
            {
                NPC.velocity.Y *= 0.95f;
                NPC.rotation = NPC.velocity.X * 0.05f;
            }
            else
            {
                if (Timer == 76)
                {
                    SoundStyle fallSound = AssetRegistry.Sounds.Bishinine.BishinineFastfall;
                    fallSound.PitchVariance = 0.1f;
                    SoundEngine.PlaySound(fallSound, NPC.position);
                }
                _afterImageTime = MathHelper.Lerp(_afterImageTime, 1f, 0.1f);
                NPC.rotation = -NPC.velocity.X * 0.05f;
                NPC.velocity.X += NPC.direction * 0.1f;
                NPC.velocity.Y *= 1.07f;
                NPC.noGravity = true;
                if (Timer % 5 == 0)
                {
                    var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, -NPC.velocity);
                    p2.Scale *= 0.5f;
                }
            }

        }


        NPC.velocity.X *= 0.94f;

        if (Timer >= 40 && NPC.collideY)
        {
            SwitchState(AIState.GrimmSpikes_Crash);
        }
    }



    private void AI_GrimSpikesCrash()
    {
        Animator.PlayAnimation(Anim_Land);
        _afterImageTime *= 0.94f;
        TargetOutlineColor = Color.Red;
        Timer++;
        NPC.velocity.X = 0;
        NPC.rotation = 0;
        if (Timer == 1)
        {
            //CometCrash(NPC.Bottom);
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, NPC.Bottom, Vector2.UnitY, ModContent.ProjectileType<BishinineCometBoom>(), GrimmSpikesDamage, 1, Main.myPlayer, ai1: 1);
            }
            SoundStyle bellHitSound = AssetRegistry.Sounds.Bishinine.BellHit1;
            bellHitSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(bellHitSound, NPC.position);
            MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
            myPlayer.ShakeAtPosition(NPC.Center, 1024f, 30f);
            ShakeScreenPosition.Shake = 2;
            SoundStyle boom = SoundID.DD2_ExplosiveTrapExplode;
            boom.PitchVariance = 0.3f;
            SoundEngine.PlaySound(boom, NPC.position);
            for (int i = 0; i < 16; i++)
            {
                float radius = 150;
                Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                offset *= Main.rand.NextFloat(1f, radius);
                offset += new Vector2(radius / 2, 0);

                Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                velocity *= Main.rand.NextFloat(1f, 2f);
                var p = Particle<ThickSmokeParticle>.Spawn(NPC.Bottom + offset, velocity, Color.DarkGray);
            }

            FXUtil.GlowCircleBoom(NPC.Bottom,
               innerColor: Color.White,
               glowColor: Color.Black,
               outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(240);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Bottom,
                    innerColor: Color.White,
                    glowColor: Color.Black,
                    outerGlowColor: Color.Black,
                    baseSize: 0.24f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (int i = 0; i < 7; i++)
            {
                Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(15f, 35f);
                var particle = FXUtil.GlowStretch(NPC.Bottom, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightCyan;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                particle.VectorScale *= 0.5f;
            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger"), NPC.position);

            FXUtil.ShakeCamera(NPC.position, 1024, 16);
            FXUtil.PunchCamera(NPC.position, Vector2.UnitY, 8, 8, 8);
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, NPC.Bottom, Vector2.Zero,
                    ModContent.ProjectileType<BellSpikeSummon>(), GrimmSpikesDamage, 1, Main.myPlayer);
            }
        }

        if (Timer >= 60)
        {
            SwitchState(AIState.Idle);
        }
    }
    #endregion



    #region Throw Scythe

    private void AI_ThrowScytheStartup()
    {
        Animator.PlayAnimation(Anim_HammerDrop);
        TargetOutlineColor = Color.Yellow;
        NPC.velocity.X *= 0.94f;
        NPC.rotation = 0;
        //Throws the hammerscythe at the ceiling and a bunch of bells fall and bounce on the ground
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            NPC.velocity.Y = -2;
            var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
            SoundStyle laugh = AssetRegistry.Sounds.Bishinine.Bishininelaugh;
            SoundEngine.PlaySound(laugh, NPC.position);
        }

        if (Timer >= 30 && NPC.collideY)
        {
            SwitchState(AIState.BellFall_ThrowScythe);
        }

    }

    private void AI_ThrowScythe()
    {
        TargetOutlineColor = Color.Red;
        Timer++;
        if (Timer == 1)
        {
            _hasHammer = false;
            NPC.TargetClosest();
            NPC.direction = TargetDirection;
            NPC.velocity.X = -NPC.direction * 5;

            SoundStyle laugh = AssetRegistry.Sounds.Bishinine.BishinineSound1;
            SoundEngine.PlaySound(laugh, NPC.position);
            if (MultiplayerHelper.IsHost)
            {
                Vector2 velocity = -Vector2.UnitY * 24;
                Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                    ModContent.ProjectileType<RisingScythe>(), RisingScytheDamage, 1, Main.myPlayer);
            }
        }
        if (Timer < 120)
        {
            Animator.PlayAnimation(Anim_FingerUp);
        }
        else
        {
            Animator.PlayAnimation(Anim_FingerUpReverse);
        }

        NPC.velocity.X *= 0.94f;
        NPC.rotation = NPC.velocity.X * 0.025f;
        if (Timer >= 240)
        {
            SwitchState(AIState.Idle);
        }
    }
    #endregion



    #region Bell Drop Attack

    private void AI_BellDropStart()
    {
        Timer++;
        if (Timer == 1)
        {
            SoundStyle fallingBell = AssetRegistry.Sounds.Bishinine.FallingBell;
            fallingBell.PitchVariance = 0.2f;
            SoundEngine.PlaySound(fallingBell, NPC.position);
        }

        TargetOutlineColor = Color.Yellow;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            NPC.direction = TargetDirection;
            if (MultiplayerHelper.IsHost)
            {

                Vector2 spawnPosition = NPC.Center;
                spawnPosition.X += NPC.direction * 80;
                spawnPosition.Y += -700;
                _baseballProjectile = Projectile.NewProjectileDirect(SourceFromThis, spawnPosition, Vector2.Zero,
                    ModContent.ProjectileType<BellBaseball>(), BaseballDamage, 1, Main.myPlayer).ModProjectile as BellBaseball;
                NPC.netUpdate = true;
            }
            NPC.velocity.Y = -8;
        }

        //Just sit here really
        NPC.velocity.X *= 0.9f;
        NPC.rotation = NPC.velocity.X * 0.03f;
        if (MultiplayerHelper.IsHost)
        {
            if (Timer >= 60 && _baseballProjectile.IsReadyToHit)
            {
                SwitchState(AIState.BellDrop_RunToBell);
            }
            else if (Timer >= 300)
            {
                //Failsafe
                //Just go back to idle state if this attack don't work some reason
                SwitchState(AIState.Idle);
            }
        }

    }

    private void AI_BellDropRunToBell()
    {
        Animator.PlayAnimation(Anim_HoldHammer);
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            AttackTimer++;
        }

        if (Timer == 3)
        {
            TeleportEffect(NPC.Center);

        }
        TargetOutlineColor = Color.Yellow;


        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                Vector2 targetCenter = _baseballProjectile.Projectile.Center;
                float side = MyTarget.Center.X < _baseballProjectile.Projectile.Center.X ? 1 : -1;
                targetCenter.X += side * 32;
                _teleportCenter = targetCenter;
                NPC.netUpdate = true;
            }
            TeleportEffect(NPC.Center);
        }
        NPC.direction = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
        _afterImageTime = MathHelper.Lerp(_afterImageTime, 1f, 0.1f);


        NPC.noTileCollide = true;
        NPC.noGravity = true;
        float waitTime = MathHelper.Lerp(10, 5, MathHelper.Clamp(AttackNumber / 6f, 0f, 1f));
        if (Timer >= waitTime)
        {
            SwitchState(AIState.BellDrop_Hit);
        }
    }

    private void AI_BellDropHit()
    {

        Timer++;
        _afterImageTime *= 0.95f;

        NPC.velocity.X *= 0.7f;
        NPC.velocity.Y = 0;
        NPC.direction = MyTarget.Center.X > NPC.Center.X ? 1 : -1;

        if (Timer == 10)
        {
            NPC.velocity.X = NPC.direction * 4;
            NPC.rotation = NPC.direction * 0.2f;
        }
        if (Timer == 10)
        {
            TargetOutlineColor = Color.Red;
            AttackNumber++;
            NPC.velocity.X = -NPC.direction * 8;
            NPC.rotation = -NPC.direction * 0.2f;

            if (MultiplayerHelper.IsHost)
            {
                float hitDirection = (MyTarget.Center - NPC.Center).ToRotation();
                _baseballProjectile.Projectile.ai[1] = hitDirection;
            }
        }
        else if (Timer >= 14)
        {
            NPC.rotation *= 0.9f;
            Animator.PlayAnimation(Anim_SpinTeleportOut);
        }
        if (Timer < 14)
        {
            TargetOutlineColor = Color.Yellow;
            Animator.PlayAnimation(Anim_Hitbell);
        }
        float waitTime = MathHelper.Lerp(60, 40, MathHelper.Clamp(AttackNumber / 6f, 0f, 1f));
        if (Timer >= waitTime)
        {
            int number = InPhase2 ? 22 : 14;
            if (AttackNumber >= number)
            {
                SwitchState(AIState.BellDrop_End);
            }
            else
            {
                SwitchState(AIState.BellDrop_RunToBell);
            }
        }
    }

    private void AI_BellDropEnd()
    {
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _baseballProjectile.Projectile.ai[2] = 1;
                _baseballProjectile.Projectile.netUpdate = true;
            }
            NPC.velocity.Y = -5;
            Vector2 pos = NPC.Center;
            var part = FXUtil.GlowCircleBoom(pos,
                           innerColor: Color.White,
                           glowColor: Color.Blue,
                           outerGlowColor: Color.Black, duration: 12, baseSize: 0.14f);
            part.Scale *= 1;


            var part2 = FXUtil.GlowCircleBoom(pos,
                  innerColor: Color.White,
                  glowColor: Color.Blue,
                  outerGlowColor: Color.Black, duration: 12, baseSize: 0.14f);
            part2.Scale *= 3;
            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(pos, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }


            for (float i = 0; i < 15; i++)
            {
                float rot = rot = Main.rand.NextFloat(-2f, 2f);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);

                Vector2 offset = rot.ToRotationVector2() * Main.rand.NextFloat(32, 64);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(2, 15);
                var particle = FXUtil.GlowCircleDetailedBoom1(pos + offset,
                    innerColor: Color.White,
                    glowColor: Color.Blue,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                    duration: Main.rand.NextFloat(5, 25));
                particle.Velocity = velocity;
                particle.Scale *= 0.35f;
                particle.Rotation = rot;
            }
        }
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.velocity.X *= 0.94f;
        if (!NPC.collideY)
        {
            Animator.PlayAnimation(Anim_Fall);
        }
        else
        {

            _hasHammer = false;
            Animator.PlayAnimation(Anim_Land);
            if (Timer >= 90)
            {
                SwitchState(AIState.Idle);
            }
        }


    }

    #endregion



    #region Idle and Spawning

    private void AI_Spawn()
    {
        Animator.PlayAnimation(Anim_Idle);
        _contactDamage = false;
        TargetOutlineColor = Color.Transparent;
        Timer++;
        if (Timer == 1)
        {
            ShowNamePlate();
        }
        NPC.velocity.X *= 0.9f;
        if (Timer >= 120)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Idle()
    {
        if (IsInsideWalls())
        {
            SwitchState(AIState.CorrectSelf);
            return;
        }
        //Set some default vars here
        if (!_hasHammer)
        {
            Animator.PlayAnimation(Anim_HammerlessIdle);
        }
        else
        {
            Animator.PlayAnimation(Anim_Idle);
        }


        _contactDamage = false;
        TargetOutlineColor = Color.Transparent;
        Timer++;
        AttackTimer = 0;
        AttackNumber = 0;
        NPC.velocity.X *= 0.9f;
        NPC.rotation = NPC.velocity.X * 0.2f;
        NPC.noGravity = false;
        NPC.noTileCollide = NPC.Bottom.Y < MyTarget.Top.Y;
        if (NPC.noTileCollide)
            return;

        float timeToWait = InPhase2 ? 95 : 60;
        if (Timer >= timeToWait)
        {
            ChooseAttack();
        }
    }

    private void AI_Despawn()
    {
        TargetOutlineColor = Color.Transparent;
        Timer++;
        float interpolant = Timer / 60f;
        float ease = EasingFunction.InOutSine(interpolant);
        NPC.scale = MathHelper.Lerp(1f, 0f, ease);
        if (Timer >= 60f)
        {
            NPC.active = false;
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0 && State != AIState.Death)
        {
            NPC.life = 1;
            SwitchState(AIState.Death);
        }


        if (NPC.life <= 0)
        {
            NPC.life = 1;
        }
    }

    private void AI_Death()
    {

        TargetOutlineColor = Color.Transparent;
        Timer++;
        if (Timer == 1)
        {
            AttackNumber = 0;
        }
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _bigBellProjectile = Projectile.NewProjectileDirect(SourceFromThis, NPC.Top - new Vector2(0, 48), Vector2.Zero, ModContent.ProjectileType<BigBell>(), BellBalancingBounceDamage, 2, Main.myPlayer);
                _deathCenter = _bigBellProjectile.Center;
                NPC.netUpdate = true;
            }
            SoundStyle laughSound = AssetRegistry.Sounds.Bishinine.Bishininelaugh;
            SoundEngine.PlaySound(laughSound, NPC.position);
        }
        if (Timer % 40 == 0 && AttackNumber < 3)
        {
            if (MultiplayerHelper.IsHost)
            {
                AttackNumber++;
                _bigBellProjectile.ai[1] = 1;
                _bigBellProjectile.netUpdate = true;
            }
        }
        if (Timer < 90)
        {
            Animator.PlayAnimation(Anim_Spinning);

        }
        else if (Timer < 120)
        {
            Animator.PlayAnimation(Anim_HoldHammer);
        }
        if (Timer < 120)
        {
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, -1, 0.1f);
            NPC.velocity.X *= 0.9f;
            NPC.rotation *= 0.9f;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        if (Timer >= 120 && Timer < 150)
        {
            float side = MyTarget.Center.X < _deathCenter.X ? 1 : -1;
            Vector2 targetCenter = _deathCenter;
            targetCenter.X += side * 100;
            Vector2 targetVelocity = (targetCenter - NPC.Center);
            NPC.direction = targetCenter.X < _deathCenter.X ? 1 : -1;
            NPC.velocity = targetVelocity * 0.1f;

            if (Timer >= 140)
            {
                Animator.PlayAnimation(Anim_Hitbell);
            }
        }

        if (Timer == 150)
        {
            NPC.velocity.Y = -8;
            NPC.velocity.X = -NPC.direction * 7;
            _black = true;
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.UnitY,
                    ModContent.ProjectileType<DeathLightning>(), 0, 0, Main.myPlayer);
            }
            if (MultiplayerHelper.IsHost)
            {
                _bigBellProjectile.ai[1] = 3;
                _bigBellProjectile.netUpdate = true;
            }
        }

        if (Timer <= 180)
        {
            CameraTargetSystem.AddTarget(NPC.Center);
        }
        if (Timer >= 150 && Timer % 5 == 0)
        {
            Vector2 vel = Main.rand.NextVector2Circular(4, 4);
            LegacyParticle.NewParticle<EmberParticle>(NPC.Center, vel);
        }
        if (Timer >= 150 && Main.rand.NextBool(10))
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ash);
        }

        if (Timer >= 150)
        {
            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
            NPC.noGravity = false;
            _black = true;
        }
        if (Timer >= 300)
        {
            NPC.Kill();
        }

    }


    private void ChooseAttack()
    {
        if (!MultiplayerHelper.IsHost)
            return;

        if (_patternManager == null)
        {
            if (InPhase2)
            {
                _patternManager = new PatternManager<AIState>(
                   new Tuple<AIState, float>(AIState.BellDrop_Start, 1.0f),
                   new Tuple<AIState, float>(AIState.BellFall_Start, 1.0f),
                   new Tuple<AIState, float>(AIState.GrimmSpikes_RunToPlayer, 1.0f),
                   new Tuple<AIState, float>(AIState.ScytheDash_Startup, 1.0f),
                   new Tuple<AIState, float>(AIState.MagicMissle_Startup, 1.0f),
                   new Tuple<AIState, float>(AIState.BouncingScytheStartup, 1.0f),
                   new Tuple<AIState, float>(AIState.CometJump_Startup, 0.1f),
                   new Tuple<AIState, float>(AIState.BellRoll_Start, 1.0f));
            }
            else
            {
                _patternManager = new PatternManager<AIState>(
                    new Tuple<AIState, float>(AIState.BellDrop_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.BellFall_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.GrimmSpikes_RunToPlayer, 1.0f),
                    new Tuple<AIState, float>(AIState.ScytheDash_Startup, 1.0f),
                    new Tuple<AIState, float>(AIState.MagicMissle_Startup, 1.0f));
            }
        }

        AIState state = _patternManager.NextPattern();
        bool needsHammer = DoesAttackUseHammer(state);
        if (_hasHammer && !needsHammer)
        {
            _nextState = state;
            _hasHammer = false;
            SwitchState(AIState.HammerDrop);
        }
        else if (!_hasHammer && needsHammer)
        {
            _nextState = state;
            _hammerRise = true;
            _hasHammer = true;
            SwitchState(AIState.HammerDrop);
        }
        else
        {
            SwitchState(state);
        }
    //    SwitchState(AIState.GrimmSpikes_RunToPlayer);

    }
    #endregion


    #region Draw Code
    private Vector2 GetDrawOrigin()
    {
        if (_animator == null)
            return NPC.frame.Size() / 2f;
        Vector2? drawOrigin = _animator.GetDrawOrigin();
        if (drawOrigin.HasValue)
            return drawOrigin.Value;
        return NPC.frame.Size() / 2f;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_black)
            drawColor = Color.Black;
        string texturePath = Texture;
        Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
        Vector2 drawPos = NPC.Center - screenPos;
        drawPos.Y += NPC.Size.Y / 2;

        Vector2 drawOrigin = GetDrawOrigin();

        float drawRotation = NPC.rotation;
        Vector2 drawScale = _squishScale * NPC.scale;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (NPC.spriteDirection == -1)
            drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;

        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 oldPos = NPC.oldPos[i];
            Vector2 oldDrawPos = oldPos - Main.screenPosition;
            oldDrawPos.Y += NPC.Size.Y / 2;
            float f = i;
            float interpolant = f / NPC.oldPos.Length;
            Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant) * 0.25f;
            fadeColor *= _afterImageTime;
            oldDrawPos += NPC.Size / 2f;
            spriteBatch.Draw(texture, oldDrawPos, NPC.frame, fadeColor, NPC.oldRot[i], drawOrigin, drawScale * 2, spriteEffects, 0f);
        }


        Texture2D starTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
        Vector2 sdrawOrigin = starTexture.Size() / 2f;
        Color cometColor = Color.GhostWhite;
        cometColor.A = 0;
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 oldPos = NPC.oldPos[i];
            Vector2 oldDrawPos = oldPos - Main.screenPosition;
            float f = i;
            float interpolant = f / NPC.oldPos.Length;
            Color fadeColor = Color.Lerp(Color.White, Color.Blue, interpolant) * 0.25f;
            fadeColor *= (1.0f - interpolant);
            fadeColor.A = 0;
            oldDrawPos += NPC.Size / 2f;
            spriteBatch.Draw(starTexture, oldDrawPos, null, fadeColor * _starTrailTime, NPC.oldRot[i], sdrawOrigin, NPC.scale * 1.5f, SpriteEffects.None, 0f);
        }
        spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale * 2, spriteEffects, 0f);
        OutlineRenderer.Queue(DrawWhite);
        return false;
    }

    private void DrawWhite(SpriteBatch spriteBatch)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawPos = NPC.Center - Main.screenPosition;
        drawPos.Y += NPC.Size.Y / 2;

        Vector2 drawOrigin = GetDrawOrigin();
        float drawRotation = NPC.rotation;
        Vector2 drawScale = _squishScale * NPC.scale * 2;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (NPC.spriteDirection == -1)
            drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;
        Color outlineColor = _outlineColor;
        spriteBatch.Draw(texture, drawPos, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
    }

    #endregion


    public override void OnKill()
    {
        base.OnKill();
        DownedBossTracker.ClearFlag(DownedBossFlag.Bishinine);
    }
}
