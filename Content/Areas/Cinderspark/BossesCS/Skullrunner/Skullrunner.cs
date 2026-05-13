using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner.Projectiles;
using Stellamod.Content.Areas.WondrousDarkspace.NPCsWD;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner;

public class Skullrunner : ScarletBoss
{
    private enum AnimationState
    {
        Laugh,
        Deadass,
        NoDamage,
        Sideframe,
        Dunking,
        Confusednograb,
        Abttograb,
        Outtabreath,
    }
    private enum AIState
    {
        Idle,
        BobbingFlyingSkulls,
        Reposition,
 

        EightwayBlobs,
        OutOfBreath,

        Dash_Startup,
        Dash_Startup_Circle,
        Dash,
        Dash_Big,
        Dash_Headbop,
  
  

        QuickDunkStart,
        DunkBeginStart,
        DunkStart,
        Dunking,
        Dunkgrab,
        Dunkfail,
        Dunksucceed,
        Dunksink,
        DunkRise,
        Despawn,
        Death,
    }
    private float _lifeTimer;
    private AnimationState _animation;

    private float _auraInterpolant;
    private bool _drawAura;
    private bool _drawTrail;
    private float _trailInterpolant;
    private bool _oscScale;
    private bool _grabbedTarget;
    private bool _freezeFrame;
    private bool _showNamePlate;
    private int _frame;
    private Vector2 _scale = Vector2.One;
    private Vector2 _targetReposition;
    private Vector2 _startBobPos;
    private Vector2 _dashVelocity;
    private Vector2 _initialVelocity;
    private Vector2 _arenaCenter;
    private Vector2 _spawnPos;
    private Vector2 _handRiseStartPosition;
    private Vector2 _handPosition;
    private int _handFrame;
    private float _handDrawRotation;
    private Vector2 _handDrawScale = Vector2.One;
    private bool _showHand;

    private float _lastDunkDirection = 1;
    private Vector2 _startDunkPosition;
    private Vector2 _endDunkPosition;

    private bool _longDash;
    private float Alpha = 1f;
    private Color OutlineColor;
    private Color HandOutlineColor;
    private float _dashCounter;
    private float _bopCounter;
    private float _attackSequenceCounter;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float BeatTimer => ref NPC.ai[2];
    private ref float Cycle => ref NPC.ai[3];
    private float MultiDunkPrepareTime => 80;
    private float FirstDunkPrepareTime => 170;
    private int BurningBlackSkullDamage => 20;
    private int LavaBubbleDamage => 20;
    private Player Target => Main.player[NPC.target];
    private Vector2 DirectionToTarget => (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startBobPos);
        writer.WriteVector2(_targetReposition);
        writer.Write(_dashCounter);
        writer.WriteVector2(_dashVelocity);
        writer.Write(_bopCounter);
        writer.Write(_attackSequenceCounter);
        writer.WriteVector2(_spawnPos);
        writer.WriteVector2(_handPosition);
        writer.Write(_grabbedTarget);
        writer.WriteVector2(_startDunkPosition);
        writer.WriteVector2(_endDunkPosition);
        writer.Write(_beatCounter);
        writer.Write(_lifeTimer);
        writer.Write(_localBeatCounter);
        writer.Write(_longDash);
        writer.Write(_lastDunkDirection);
        writer.WriteVector2(_initialVelocity);
        writer.WriteVector2(_arenaCenter);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startBobPos = reader.ReadVector2();
        _targetReposition = reader.ReadVector2();
        _dashCounter = reader.ReadSingle();
        _dashVelocity = reader.ReadVector2();
        _bopCounter = reader.ReadSingle();
        _attackSequenceCounter = reader.ReadSingle();
        _spawnPos = reader.ReadVector2();
        _handPosition = reader.ReadVector2();
        _grabbedTarget = reader.ReadBoolean();
        _startDunkPosition = reader.ReadVector2();
        _endDunkPosition = reader.ReadVector2();
        _beatCounter = reader.ReadSingle();
        _lifeTimer = reader.ReadSingle();
        _localBeatCounter = reader.ReadSingle();
        _longDash = reader.ReadBoolean();
        _lastDunkDirection = reader.ReadSingle();
        _initialVelocity = reader.ReadVector2();
        _arenaCenter = reader.ReadVector2();
    }


    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.TrailCacheLength[Type] = 32;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        Main.npcFrameCount[Type] = 17;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _scale = Vector2.One;
        NPC.width = 64;
        NPC.height = 64;
        NPC.damage = 80;
        NPC.defense = 10;
        NPC.lifeMax = 5500;
        NPC.HitSound = SoundID.NPCHit16;
        NPC.value = Item.buyPrice(silver: 50);
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.boss = true;
        NPC.npcSlots = 10f;

        //Setup the music and boss bar
        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Skullrunner");
        NPC.aiStyle = -1;
    }

    private bool _beatHit;
    private float _beatCounter;
    private float _localBeatCounter;
    private void Metronome()
    {
      
        float beatsPerTick= 130 / 60f / 60f;
        BeatTimer += beatsPerTick;

        _beatHit = false;
        while(BeatTimer >= 1f)
        {
            BeatTimer -= 1f;
            _beatCounter++;
            _localBeatCounter++;
            _beatHit = true;
        }
        if(_beatCounter >= 96)
        {
            _longDash = false;
            _bopCounter = 0;
            _dashCounter = 0;
            _beatCounter = 0;
            _localBeatCounter = 0;
            _attackSequenceCounter = 0;
            NPC.netUpdate = true;
        }
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        if (!_freezeFrame)
            NPC.frameCounter += 0.25f;
        if (NPC.frameCounter >= 1f)
        {
            _frame++;
            NPC.frameCounter = 0f;
        }

        switch (_animation)
        {
            case AnimationState.Laugh:
                if (_frame >= 5)
                {
                    _frame = 0;
                }
                break;
            case AnimationState.Deadass:
                _frame = 5;
                break;
            case AnimationState.NoDamage:
                _frame = 6;
                break;
            case AnimationState.Sideframe:
                _frame = 7;
                break;
            case AnimationState.Dunking:
                if (_frame < 8)
                {
                    _frame = 8;
                }
                if (_frame >= 12)
                {
                    _frame = 8;
                }
                break;
            case AnimationState.Confusednograb:
                _frame = 12;
                break;
            case AnimationState.Abttograb:
                _frame = 13;
                break;
            case AnimationState.Outtabreath:
                if (_frame < 14)
                {
                    _frame = 14;
                }
                if (_frame >= 17)
                {
                    _frame = 14;
                }
                break;
        }

        NPC.frame.Y = frameHeight * _frame;
    }

    private void NextDunkPositions()
    {
        float dir = MyTarget.Center.X > _arenaCenter.X ? 1 : -1;
        float dunkRange = 384;
        _startDunkPosition = _arenaCenter + Vector2.UnitX * dunkRange * dir;
        _startDunkPosition = TileUtilities.FallToSolidTile(_startDunkPosition.ToTileCoordinates()).ToWorldCoordinates();
        _startDunkPosition.Y -= 400;
        _endDunkPosition = _startDunkPosition - Vector2.UnitX * dunkRange * dir * 2;
    }
    public override void AI()
    {
        base.AI();
        if (_spawnPos == Vector2.Zero)
        {
            NPC.TargetClosest();
            _arenaCenter = TileUtilities.GuessArenaCenter(MyTarget.Center);
            _spawnPos = NPC.Center;
            _handRiseStartPosition = _spawnPos;
        }
        Lighting.AddLight(NPC.Center, Color.Yellow.ToVector3() * 2);
        Metronome();
        if (_drawTrail)
        {
            _trailInterpolant = MathHelper.Lerp(_trailInterpolant, 1f, 0.1f);
        } else
        {
            _trailInterpolant = MathHelper.Lerp(_trailInterpolant, 0f, 0.1f);
        }
        if (_drawAura)
        {
            _auraInterpolant = MathHelper.Lerp(_auraInterpolant, 0.85f, 0.1f);
        } else
        {
            _auraInterpolant = MathHelper.Lerp(_auraInterpolant, 0.2f, 0.1f);
        }
        _drawTrail = false;
        _drawAura = false;
        _oscScale = false;
        if (!_showNamePlate)
        {
            ShowNamePlate();
            _showNamePlate = true;
        }

        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if(!NPC.HasValidTarget && State != AIState.Despawn)
            {
                SwitchState(AIState.Despawn);
            }
        }

        if (Main.rand.NextBool(8))
        {
            Vector2 pos = NPC.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var fs = FaintSmokeParticle.SpawnInAlphaLayer(pos, -Vector2.UnitY, Scale: Main.rand.NextFloat(0.25f, 0.5f));
            fs.noShrink = true;
            fs.behindLayer = true;
            fs.Scale *= Main.rand.NextFloat(0.25f, 0.5f);
            fs.color = Color.Lerp(Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f)), Color.Black, 0.7f);
            fs.fadeToColor = Color.Lerp(Color.OrangeRed, Color.Black, 0.8f);
        }
        _lifeTimer++;
        switch (State)
        {
            case AIState.Death:
                AI_Death();
                break;
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.OutOfBreath:
                AI_OutOfBreath();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.BobbingFlyingSkulls:
                _oscScale = true;
                _drawAura = true;
                AI_BobbingFlyingSkulls();
                break;

            case AIState.Dash_Startup:
                _drawTrail = true;
                AI_DashStartup();
                break;

            case AIState.Dash_Startup_Circle:
                _drawTrail = true;
                AI_DashStartupCircle();
                break;
            case AIState.Dash:
                _drawTrail = true;
                AI_Dash();
                break;
            case AIState.Dash_Big:
                _drawTrail = true;
                AI_DashBig();
                break;
            case AIState.Dash_Headbop:
                AI_DashHeadbop();
                break;

            case AIState.EightwayBlobs:
                _drawAura = true;
                AI_EightwayBlobs();
                break;
            case AIState.DunkBeginStart:
                AI_DunkPrepare();
                break;
            case AIState.DunkStart:
                _drawTrail = true;
                AI_DunkStart();
                break;
            case AIState.Dunking:
                _drawTrail = true;
                AI_Dunking();
                break;
            case AIState.Dunkgrab:
                _drawTrail = true;
                AI_Dunkgrab();
                break;
            case AIState.Dunkfail:
                AI_Dunkfail();
                break;
            case AIState.Dunksucceed:
                AI_Dunksucceed();
                break;
            case AIState.Dunksink:
                AI_Dunksink();
                break;
            case AIState.DunkRise:
                AI_DunkRise();
                break;
        }

        Vector2 oscScale = Vector2.Lerp(Vector2.One, new Vector2(1.1f, 0.9f), ExtraMath.Osc(0f, 1f, speed: 3));
        if (_oscScale)
        {
            _scale = Vector2.Lerp(_scale, oscScale, 0.1f);
        }
        else
        {
            _scale = Vector2.Lerp(_scale, Vector2.One, 0.1f);
        }

        Lighting.AddLight(NPC.position, Color.OrangeRed.ToVector3() * 0.78f);
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot)
            && (State == AIState.Dash || State == AIState.Dash_Big);
    }

    private bool BeatHit()
    {
        return _beatHit;
    }

    public override void OnKill()
    {
        base.OnKill();
        DownedBossTracker.ClearFlag(DownedBossFlag.Skullrunner);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if(NPC.life <= 0)
        {
            if (State != AIState.Death)
                SwitchState(AIState.Death);
            NPC.life = 1;
        }
    }
    private void AI_Death()
    {
        float deathTime = 240f;
        Timer++;
        if(Timer == 1)
        {
            _initialVelocity = NPC.velocity;
        }
        if (Timer % 2 == 0)
        {
            float range = Main.rand.NextFloat(252, 512);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (NPC.Center - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Red;
        }

        if (Timer % 2 == 0)
        {
            float range = Main.rand.NextFloat(384, 666);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (NPC.Center - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Lerp(Color.White, Color.Red, Main.rand.NextFloat(0f, 1f));
            fx.VectorScale *= 0.5f;
        }

        if(Timer > deathTime * 0.5f)
        {
            CameraTargetSystem.AddTarget(NPC.Center);
            CameraTargetSystem.SetLingerTime(120);
        }

        ShakeScreenPosition.Shake = MathHelper.Lerp(0, 4, EasingFunction.InOutSine(Timer / deathTime));

        //Move to the center of the arena and slowly move up
        Vector2 positionToDieAt = _arenaCenter;
        positionToDieAt.Y -= MathHelper.Lerp(0, 128, Timer / 180f);
        Vector2 targetVelocity = (positionToDieAt - NPC.Center );
        NPC.velocity = Vector2.Lerp(_initialVelocity, targetVelocity, EasingFunction.InExpo(Timer / 90f));
       
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.rotation = MathHelper.Lerp(-0.05f, 0.05f, ExtraMath.Osc(0f, 1f, speed: 12));
        if(Timer < 90)
        {
            _animation = AnimationState.Deadass;
        }
        else
        {
            _animation = AnimationState.Laugh;
        }
        
        if(Timer % 15 == 0)
        {
            FXUtil.GlowCircleBoom(NPC.Center, Color.White * 0.5f, Color.Yellow * 0.5f, Color.Red * 0.5f, 25, baseSize: MathHelper.Lerp(0.1f, 0.24f, EasingFunction.InOutSine(Timer/180f)));
        }

        if(Timer >= deathTime)
        {
            GoreUtilities.CreateDeathGores(this, 3);
            NPC target = NPC;

            SoundStyle deathSound = new SoundStyle($"Stellamod/Assets/Sounds/DMHeart__Dash");
            SoundEngine.PlaySound(deathSound);

            SoundStyle hitSound = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireExplosion1");
            SoundEngine.PlaySound(hitSound, target.position);

            FXUtil.ShakeCamera(target.Center, 1024, 32);
            FXUtil.GlowCircleBoom(target.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

            for (float f = 0; f < 32; f++)
            {
                Vector2 vel = -Vector2.UnitY * 8 * Main.rand.NextFloat(0.2f, 1f);
                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(32, 32), DustID.Torch, vel, Scale: 3f);
            }

            //Big ass explosion
            float num = 10f;
            for (float f = 0; f < num; f++)
            {
                Vector2 upwardVelocity = -Vector2.UnitY * 8;
                upwardVelocity = upwardVelocity.RotatedByRandom(MathHelper.ToRadians(45));
                DustParticle.Spawn(target.Center, upwardVelocity);
            }

            for (float f = 0; f < num; f++)
            {
                Vector2 upwardVelocity = -Vector2.UnitY * 2;
                upwardVelocity = upwardVelocity.RotatedByRandom(MathHelper.ToRadians(45));
                SparkleParticle.Spawn(target.Center, upwardVelocity, Scale: 0.5f);
            }

            for (float f = 0; f < 16; f++)
            {
                var dp = Particle<DustParticle>.Spawn(target.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.innerColor = Color.Yellow;
                dp.gravity = 0;
                dp.dampening = 0.05f;
                dp.noTileCollide = true;
            }

            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(target.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }

            for (float i = 0; i < 4; i++)
            {
                float rot = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(60)).ToRotation();
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot;
                particle.VectorScale *= 3;
            }

            NPC.Kill();
        }
    }

    private void AI_Despawn()
    {
        Timer++;
        NPC.velocity.X *= 0.5f;
        NPC.velocity.Y += 0.5f;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.rotation = NPC.velocity.X * 0.05f;
        if (Timer >= 100)
        {
            NPC.active = false;
        }
    }
    private void AI_DashStartupCircle()
    {
        _showHand = false;
        _animation = AnimationState.Deadass;
        OutlineColor = Color.Yellow;
        Timer++;
        if(Timer == 1)
        {
            _startDunkPosition = NPC.Center;
        }

        if (Timer % 3 == 0)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork);
        }

        //At 0:30 seconds he does a lot of circular movement and then dashes towards the player,
        //does this 3 times before he just bumps his head to the beat.
        //Repeats that cycle one more time before returning to phase 1 again

        Vector2 targetPosition = _startDunkPosition;
        Vector2 offset = -Vector2.UnitY.RotatedBy(_lifeTimer * 0.15f) * 128;
        targetPosition += offset;
        Vector2 velocityToPosition = targetPosition - NPC.Center;
        velocityToPosition *= 0.2f;
        NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToPosition, 0.5f);
        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);

        if (_localBeatCounter >= 1)
        {
            SwitchState(AIState.Idle);
        }
    }
    private void AI_EightwayBlobs()
    {
        _showHand = false;
        _animation = AnimationState.Laugh;
        OutlineColor = Color.Transparent;
        HandOutlineColor = Color.Transparent;
        Timer++;

        //Nove in a circle
        float rotOffset = Timer * 0.025f;
        float offset = 80;
        Vector2 targetCirclePos = Target.Center + rotOffset.ToRotationVector2() * offset;

        switch (Cycle)
        {
            default:
            case 0:
                targetCirclePos.Y -= 32;
                break;
            case 1:
                targetCirclePos.Y += 32;
                break;
        }

        Vector2 velocityToCirclePos = (targetCirclePos - NPC.Center);
        velocityToCirclePos *= 0.1f;

        NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToCirclePos, 0.1f);
        float targetRotation = MathF.Sin(Timer * 0.125f) * 0.5f;
        NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);


        //Bobble his head to the beat
        //It's 130 beat sper minute
        //3600 ticks per minute
        //3600 / 130
        //27 ticks per beat, roughly, not evenly but close enough, you'd need a long fight to notice desync
        if (BeatHit())
        {
            if (MultiplayerHelper.IsHost)
            {

                Vector2 spawnPos = Target.Center + Vector2.UnitY * 128;
                spawnPos.X += Main.rand.NextFloat(-252, 252);
                Vector2 velocity = -Vector2.UnitY * 10;
                int projectileType = ModContent.ProjectileType<RisingLavaBubble>();
                int lavaBubbleDamage = LavaBubbleDamage;
                int lavaBubbleKnockback = 1;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, velocity, projectileType, lavaBubbleDamage, lavaBubbleKnockback, Main.myPlayer);
            }

            Cycle++;
            if (Cycle >= 2)
            {
                Cycle = 0;
            }
        }

        if(_localBeatCounter >= 3)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_GetReadyToDunk(float dunkPrepareTime)
    {
        Timer++;
        //Get the hand up
        if (!_showHand)
        {
            _handPosition = NPC.Center;
        }
        _showHand = true;
        if(Timer == 1)
        {
            _initialVelocity = NPC.Center;
            NPC.TargetClosest();
        }

        Vector2 targetHandPosition = NPC.Center + Vector2.UnitX * NPC.direction * 32;
        _handPosition = _handPosition.MoveTowards(targetHandPosition, MathHelper.Lerp(0f, 32, EasingFunction.InOutExpo7(Timer / dunkPrepareTime)));
        Vector2 sidePosition = NPC.Center + -Vector2.UnitX * NPC.direction * 72 + Vector2.UnitY * 48;
        FaceDirection();
        if(Timer < 40)
        {
            _animation = AnimationState.Sideframe;
        }

        HandOutlineColor = Color.Yellow;
        float targetRotation = (Target.Center - _handPosition).ToRotation();
        _handDrawRotation = Utils.AngleLerp(_handDrawRotation, targetRotation, 0.07f);
        _startBobPos = Target.Center + Vector2.UnitX * _lastDunkDirection * 72;
        Vector2 targetSidePosition = _startBobPos;
        Vector2 startSidePosition = _initialVelocity;

        float ratio = Timer / dunkPrepareTime;
        float ease = EasingFunction.InOutExpo7(ratio);
        Vector2 interp = Vector2.Lerp(startSidePosition, targetSidePosition, ease);
        Vector2 vel = interp - NPC.Center;
        NPC.velocity = vel;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);

        if(Timer >= dunkPrepareTime * 0.8f)
        {
            _handFrame = 1;
        }
    }

    private void AI_DunkPrepare()
    {
        AI_GetReadyToDunk(FirstDunkPrepareTime);
        if (Timer >= FirstDunkPrepareTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_DunkStart()
    {
        AI_GetReadyToDunk(MultiDunkPrepareTime);
        if (_localBeatCounter >= 2)
        {
            SwitchState(AIState.Dunking);
        }
    }

    private void AI_Dunking()
    {
        Timer++;

        //If you move out of the way you won't get grabbed, but this attack is fast sooo
        if (Timer == 1)
        {
            _handRiseStartPosition = _handPosition;
            NextDunkPositions();
        }

        NPC.velocity *= 0.9f;



        //We have to use a lerp here cause every attack needs to have a set time
        //Otherwise it'll desync
        float grabTicks = 12;
        float interpolant = Timer / grabTicks;
        float ease = EasingFunction.InOutCubic(interpolant);
        Vector2 newHandPosition = Vector2.Lerp(_handRiseStartPosition, MyTarget.Center, ease);
        _handPosition = newHandPosition;

        float targetRotation = (_endDunkPosition - _handPosition).ToRotation();
        _handDrawRotation = Utils.AngleLerp(_handDrawRotation, targetRotation, 0.1f);

        //check if completed
        if(Timer >= grabTicks)
        {
            float distanceToTarget = Vector2.Distance(_handPosition, Target.Center);
            if(distanceToTarget <= 200 && !Target.immune)
            {
                _grabbedTarget = true;
            }
            else
            {
                _grabbedTarget = false;
            }
            SwitchState(AIState.Dunkgrab);
        }
    }

    private void AI_Dunkgrab()
    {
        OutlineColor = Color.Yellow;
        _animation = AnimationState.Dunking;
        Timer++;
        if (Timer == 1)
        {
            NPC.netUpdate = true;
        }

        _handFrame = 2;
        _handPosition += NPC.velocity;

        //We need to create a cool oval and lerp
        float dunkingTicks = 30;
        if (_longDash)
        {
            dunkingTicks *= 4.2f;
        }

        float dunkingInterpolant = Timer / dunkingTicks;
        float dunkEase = EasingFunction.InOutSine(dunkingInterpolant);
        Vector2 linearPosition = Vector2.Lerp(_startDunkPosition, _endDunkPosition, dunkEase);
       // _endDunkPosition = _endDunkPosition.RotatedBy(0.005f, _startDunkPosition);
        //Go up
        float jumpEase = EasingFunction.QuadraticBump(dunkEase);
        float yOffset = MathHelper.Lerp(0f, -700, jumpEase);
        if (_longDash)
        {
            yOffset *= 1.25f;
        }

        Vector2 dunkPosition = linearPosition + new Vector2(0, yOffset);
        Vector2 velocityToDunkPosition = (dunkPosition - NPC.Center);
        NPC.velocity = Vector2.Lerp(Vector2.Zero, velocityToDunkPosition, EasingFunction.InExpo(Timer / (dunkingTicks)));
        NPC.rotation = NPC.velocity.ToRotation();
        if(NPC.spriteDirection == -1)
        {
            NPC.rotation += MathHelper.Pi;
        }


        if (_grabbedTarget)
        {
            HandOutlineColor = Color.Red;
            MovePlayer throwModPlayer = Target.GetModPlayer<MovePlayer>();
            throwModPlayer.targetSuckPosition = _handPosition;
        }

        if (Timer >= dunkingTicks + 2)
        {
            if (_grabbedTarget)
            {
                SwitchState(AIState.Dunksucceed);
            }
            else
            {
                SwitchState(AIState.Dunkfail);
            }
        }
    }

    private void Eruption(Vector2 position, Vector2 velocity)
    {
        SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
        hitSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(hitSound, position);

        for (int i = 0; i < 7; i++)
        {
            Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
        }

        for (int i = 0; i < 7; i++)
        {
            Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
        }

        FXUtil.ShakeCamera(position, 1024, 32);
        FXUtil.GlowCircleBoom(position,
            innerColor: Color.White,
            glowColor: Color.Yellow,
            outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, position);
        for (float f = 0; f < 32; f++)
        {
            Dust.NewDustPerfect(position, DustID.Torch,
                (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
        }


        for (float i = 0; i < 8; i++)
        {
            float progress = i / 4f;
            float rot = progress * MathHelper.ToRadians(360);
            rot += Main.rand.NextFloat(-0.5f, 0.5f);
            Vector2 offset = rot.ToRotationVector2() * 24;
            var particle = FXUtil.GlowCircleDetailedBoom1(position,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red,
                baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                duration: Main.rand.NextFloat(15, 25));
            particle.Rotation = rot + MathHelper.ToRadians(45);
        }

        for (float f = 0; f < 16; f++)
        {
            Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
            pVelocity *= Main.rand.NextFloat(0.5f, 2f);
            var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
            FXUtil.GlowFragmentParticle(position, pVelocity,
                innerColor: Color.Red,
                outerColor: Color.Orange,
                fadeToColor: Color.Purple,
                distortOut: true);

            if (Main.rand.NextBool(4))
            {
                Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                 velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
            }
            if (Main.rand.NextBool(4))
            {
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                 velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
            }
            if (Main.rand.NextBool(4))
            {

                var part = FXUtil.GlowFragmentParticle(position, pVelocity,
                 innerColor: Color.DarkRed,
                 outerColor: Color.DarkBlue,
                 fadeToColor: Color.Black,
                 distortOut: false);
                part.Scale *= 1.3f;
            }
        }
        //Dust Particles
        for (int k = 0; k < 4; k++)
        {
            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7)) * 15 * Main.rand.NextFloat(0.5f, 1f);
            newVelocity *= 1f - Main.rand.NextFloat(0.3f);
            Dust.NewDust(NPC.Center, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
        }


        for (int i = 0; i < 16; i++)
        {
            Vector2 speed = velocity.RotatedByRandom(MathHelper.PiOver4) * 15 * Main.rand.NextFloat(0.5f, 1f);
            var d = Dust.NewDustPerfect(position, DustID.InfernoFork, speed, Scale: 3f);
        }
        FXUtil.ShakeCamera(position, 1024, 8);
    }
    private void AI_Dunksucceed()
    {
        OutlineColor = Color.Transparent;
        HandOutlineColor = Color.Transparent;

        _handFrame = 1;


        Timer++;
        if(Timer == 1)
        {
            MovePlayer throwModPlayer = Target.GetModPlayer<MovePlayer>();
            throwModPlayer.throwVelocity = Vector2.UnitY * 40;
            Eruption(NPC.Center, -Vector2.UnitY);
        }
       

        Vector2 velocity = Vector2.UnitY * MathF.Sin(Timer * 0.2f) * 0.1f;
        velocity -= Vector2.UnitY * MathHelper.Lerp(3, 0f, Timer / 60f);
        NPC.velocity = Vector2.Lerp(NPC.velocity, velocity, 0.1f);

        _handPosition += NPC.velocity;
        NPC.rotation = NPC.velocity.X * 0.05f;
        if (Timer >= 6)
        {
            _animation = AnimationState.Laugh;
        }
        else
        {
            _animation = AnimationState.Deadass;
        }
        if (Timer >= 11)
        {
            SwitchState(AIState.Dunksink);
        }
    }

    private void AI_Dunksink()
    {
        Timer++;
        _animation = AnimationState.Laugh;
        OutlineColor = Color.Transparent;
        HandOutlineColor = Color.Transparent;

        if(Timer == 1)
        {
            //Should be safe to use these variables I think
        
      
            _startDunkPosition = NPC.Center;
            NPC.netUpdate = true;
        }
        float offset = 48;
        _endDunkPosition = Target.Center + Vector2.UnitX * offset * -NPC.spriteDirection;

        float sinkingTinks = 25;
        float interpolant = Timer / sinkingTinks;
        float ease = EasingFunction.InOutSine(interpolant);
        Vector2 targetPosition = Vector2.Lerp(_startDunkPosition, _endDunkPosition, ease);
        targetPosition.X += EasingFunction.QuadraticBump(interpolant) * NPC.spriteDirection * 180;
        targetPosition.Y -= 100;
        Vector2 velocityToSinkPosition = (targetPosition - NPC.Center);
        NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToSinkPosition, 0.5f);
        NPC.rotation = NPC.velocity.ToRotation();

        _handPosition += NPC.velocity;
        if(Timer >= sinkingTinks)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_DunkRise()
    {
        Timer++;
        if(Timer == 1)
        {
            //Choose which side to rise from
            float offset = 48;
            _endDunkPosition = Target.Center + Vector2.UnitX * offset * -NPC.spriteDirection;
            _startDunkPosition = _endDunkPosition + Vector2.UnitY * 128;


            NPC.netUpdate = true;
        }

        float risingTicks = 6;
        float interpolant = Timer / risingTicks;
        float ease = EasingFunction.InOutSine(interpolant);
        Vector2 targetPosition = Vector2.Lerp(_startDunkPosition, _endDunkPosition, ease);
        Vector2 velocityToSinkPosition = (targetPosition - NPC.Center);
        NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToSinkPosition, 0.75f);
        NPC.rotation *= 0.9f;
        Alpha = MathHelper.Lerp(0f, 1f, ease);
        _handPosition += NPC.velocity;
        if(Timer >= risingTicks)
        {
            ProgressSequence();  
        }
    }
    private void AI_Dunkfail()
    {

        Timer++;
        OutlineColor = Color.Transparent;
        HandOutlineColor = Color.Transparent;
        NPC.velocity *= 0.9f;

        _handFrame = 1;
        _animation = AnimationState.Confusednograb;
        if (Timer >= 11)
        {
            SwitchState(AIState.Dunksink);
        }
    }

    private void AI_OutOfBreath()
    {
        _showHand = false;
        _animation = AnimationState.Outtabreath;
        OutlineColor = Color.Transparent;
        HandOutlineColor = Color.Transparent;
        //As the song stops yelling he becomes out of breathe,
        //little exploding 8-way lava bubbles come from the lava and pop like hive knight, 
        Timer++;
        NPC.TargetClosest();

        Vector2 hoverAroundPos = _spawnPos;
        Vector2 velocityToHoverPos = (hoverAroundPos - NPC.Center).SafeNormalize(Vector2.Zero);
        float distance = Vector2.Distance(Target.Center, hoverAroundPos);
        float maxSpeed = 15;
        if (distance < maxSpeed)
        {
            velocityToHoverPos *= distance;
        }
        else
        {
            velocityToHoverPos *= maxSpeed;
        }

        if (distance > 32)
        {
            NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToHoverPos, 0.05f);
        }
        else
        {
            NPC.velocity *= 0.98f;
        }


        NPC.rotation = NPC.velocity.X * 0.05f;
        if (BeatHit())
        {
            Cycle++;
            if(Cycle % 2 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {

                    Vector2 spawnPos = Target.Center + Vector2.UnitY * 128;
                    spawnPos.X += Main.rand.NextFloat(-252, 252);
                    Vector2 velocity = -Vector2.UnitY * 10;
                    int projectileType = ModContent.ProjectileType<RisingLavaBubble>();
                    int lavaBubbleDamage = LavaBubbleDamage;
                    int lavaBubbleKnockback = 1;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, velocity, projectileType, lavaBubbleDamage, lavaBubbleKnockback, Main.myPlayer);
                }
            }

        }
        float targetRotation = MathF.Sin(Timer * 0.2f) * 0.05f;
        NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);
        if (_localBeatCounter >= 6)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_DashHeadbop()
    {
        _dashCounter = 0;
        _animation = AnimationState.Deadass;
        Timer++;
        if (Timer % 8 == 0)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Lava);
        }

        OutlineColor = Color.Transparent;
        HandOutlineColor = Color.Transparent;
        _startBobPos = Target.Center - Vector2.UnitY * 120;
        //Nove in a circle
        float rotOffset = Timer * 0.025f;
        float offset = 80;
        Vector2 targetCirclePos = _startBobPos + rotOffset.ToRotationVector2() * offset;

        switch (Cycle)
        {
            default:
            case 0:
                targetCirclePos.Y -= 32;
                break;
            case 1:
                targetCirclePos.Y += 32;
                break;
        }

        Vector2 velocityToCirclePos = (targetCirclePos - NPC.Center);
        velocityToCirclePos *= 0.1f;

        NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToCirclePos, 0.1f);
        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);

        //Bobble his head to the beat
        //It's 130 beat sper minute
        //3600 ticks per minute
        //3600 / 130
        //27 ticks per beat, roughly, not evenly but close enough, you'd need a long fight to notice desync
        if (BeatHit())
        {
            Cycle++;
            if (Cycle >= 2)
            {
                Cycle = 0;
            }
        }

        if(Timer >= 100)
        {
            _animation = AnimationState.Laugh;
        }

        if (Timer >= 200 && BeatHit())
        {
            _bopCounter++;
            if (_bopCounter >= 2)
            {
                SwitchState(AIState.Idle);
            }
            else
            {
                SwitchState(AIState.Dash_Startup);
            }

        }
    }
    private void AI_DashStartup()
    {
        _animation = AnimationState.Sideframe;
        OutlineColor = Color.Yellow;
       
        Timer++;
        if (Timer % 8 == 0)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork);
        }
        float waitTime = _dashCounter < 1 ? 57 : 27;
        //At 0:30 seconds he does a lot of circular movement and then dashes towards the player,
        //does this 3 times before he just bumps his head to the beat.
        //Repeats that cycle one more time before returning to phase 1 again

        Vector2 targetPosition = Target.Center;
        Vector2 offset = -Vector2.UnitY.RotatedBy(_lifeTimer * 0.15f) * MathHelper.Lerp(250, 400, Timer / waitTime);
        targetPosition += offset;
        Vector2 velocityToPosition = targetPosition - NPC.Center;
        NPC.direction = -1;
        NPC.spriteDirection = -NPC.direction;
        NPC.velocity = Vector2.Lerp(Vector2.Zero, velocityToPosition, EasingFunction.InOutSine(Timer / 90f));
        NPC.rotation = NPC.velocity.ToRotation();
        if (_localBeatCounter >= 5)
        {
            SwitchState(AIState.Idle);
        }
    }
    private void AI_DashBig()
    {
        Timer++;
        OutlineColor = Color.Red;
        if (Timer == 1)
        {

            SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
            hitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(hitSound, NPC.position);

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
            }

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
            }

            FXUtil.ShakeCamera(NPC.Center, 1024, 32);
            FXUtil.GlowCircleBoom(NPC.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.position);
            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(NPC.Center, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }


            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            SoundEngine.PlaySound(SoundID.Item73, NPC.position);
            _dashVelocity = DirectionToTarget * 62;
        }
        NPC.velocity = _dashVelocity;

        if (Timer % 1 == 0)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork);
        }


        if (Timer >= 2 && BeatHit())
        {
            SwitchState(AIState.Idle);
        }
        NPC.rotation = NPC.velocity.X * 0.035f;
    }

    private void AI_Dash()
    {

        Timer++;
        OutlineColor = Color.Red;
        if (Timer == 1)
        {

            SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
            hitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(hitSound, NPC.position);

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
            }

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
            }

            FXUtil.ShakeCamera(NPC.Center, 1024, 32);
            FXUtil.GlowCircleBoom(NPC.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.position);
            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(NPC.Center, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }


            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            SoundEngine.PlaySound(SoundID.Item73, NPC.position);
            _dashVelocity = DirectionToTarget * 22;
        }
        NPC.velocity = _dashVelocity;

        if (Timer % 1 == 0)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork);
        }


        if (Timer >= 2 && BeatHit())
        {
            SwitchState(AIState.Idle);
        }
        NPC.rotation = NPC.velocity.X * 0.035f;
    }

    private void FaceDirection()
    {
        NPC.direction = (Target.Center.X < NPC.Center.X) ? 1 : -1;
        NPC.spriteDirection = -NPC.direction;
    }


    private void AI_BobbingFlyingSkulls()
    {
        //Bobbles his head to the beat as little tiny flying skulls come from the side as
        //he has his glowing cool aura circle,
        //you can attack him here as he’s just floating around to the beat.

        _startBobPos = _spawnPos;
        NPC.TargetClosest();

        OutlineColor = Color.Yellow;
        Timer++;
        if (Timer % 8 == 0)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Lava);
        }

        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _startBobPos = NPC.Center;
                NPC.netUpdate = true;
            }
        }

        if (Timer < 270)
        {
            _animation = AnimationState.Deadass;
        }
        else
        {
            _animation = AnimationState.Laugh;
        }

        //Nove in a circle
        float rotOffset = Timer * 0.025f;
        float offset = 80;
        Vector2 targetCirclePos = _startBobPos + rotOffset.ToRotationVector2() * offset;

        switch (Cycle)
        {
            default:
            case 0:
                targetCirclePos.Y -= 32;
                break;
            case 1:
                targetCirclePos.Y += 32;
                break;
        }

        Vector2 velocityToCirclePos = (targetCirclePos - NPC.Center);
        velocityToCirclePos *= 0.1f;

        NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToCirclePos, 0.1f);
        float targetRotation = MathF.Sin(Timer * 0.125f) * 0.5f;
        NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);

 
        //Bobble his head to the beat
        //It's 130 beat sper minute
        //3600 ticks per minute
        //3600 / 130
        //27 ticks per beat, roughly, not evenly but close enough, you'd need a long fight to notice desync
        if (BeatHit())
        {
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < 2; i++)
                {
                    float skullSpawnOffset = 1512;
                    Vector2 skullSpawnPoint = NPC.Center
                        + Main.rand.NextVector2CircularEdge(skullSpawnOffset, skullSpawnOffset);

                    Vector2 skullSpawnVelocity = (NPC.Center - skullSpawnPoint);
                    skullSpawnVelocity = skullSpawnVelocity.SafeNormalize(Vector2.Zero);
                    skullSpawnVelocity *= Main.rand.NextFloat(4, 7);

                    int skullProjectileType = ModContent.ProjectileType<BurningBlackSkull>();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), skullSpawnPoint, skullSpawnVelocity,
                        skullProjectileType, BurningBlackSkullDamage, 1, Main.myPlayer);
                }

            }

            Cycle++;
            if (Cycle >= 2)
            {
                Cycle = 0;
            }
        }

        //This attack is uh
        //How many beats long?
        if (_beatCounter >= 24)
        {
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (!proj.active)
                    continue;
                if (proj.type == ModContent.ProjectileType<BurningBlackSkull>())
                    proj.Kill();
            }
            SwitchState(AIState.Idle);
        }
    }
    private void AI_Idle()
    {
        Timer++;
        Vector2 velocity = Vector2.UnitY * MathF.Sin(Timer * 0.2f) * 0.1f;
        NPC.velocity = Vector2.Lerp(NPC.velocity, velocity, 0.1f);
        NPC.rotation = NPC.velocity.X * 0.05f;
        ProgressSequence();
    }

    private void ProgressSequence()
    {
        if (MultiplayerHelper.IsHost)
        {
            switch (_attackSequenceCounter)
            {
                case 0:
                    SwitchState(AIState.BobbingFlyingSkulls);
                    break;
                case 1:
                    if (_beatCounter < 24)
                        return;

                    SwitchState(AIState.DunkBeginStart);
                    break;
                case 2:
                    if (_beatCounter < 29)
                        return;

                    SwitchState(AIState.DunkStart);
                    break;
                case 3:
                    if (_beatCounter < 35)
                        return;

                    SwitchState(AIState.DunkStart);
                    break;
                case 4:
                    if (_beatCounter < 39)
                        return;

                    SwitchState(AIState.EightwayBlobs);
                    break;
                case 5:
                    if (_beatCounter < 46)
                        return;

                    SwitchState(AIState.DunkStart);
                    break;
                case 6:
                    if (_beatCounter < 50)
                        return;

                    SwitchState(AIState.DunkStart);
                    break;
                case 7:
                    if (_beatCounter < 54)
                        return;

                    _longDash = true;
                    SwitchState(AIState.DunkStart);
                    break;
                case 8:
                    if (_beatCounter < 64)
                        return;

                    SwitchState(AIState.Dash_Startup_Circle);
                    break;
                case 9:

                    SwitchState(AIState.Dash);
                    break;
                case 10:
                    SwitchState(AIState.Dash_Startup_Circle);
                    break;
                case 11:
                    SwitchState(AIState.Dash);
                    break;
                case 12:
                    SwitchState(AIState.Dash_Startup_Circle);
                    break;
                case 13:
                    SwitchState(AIState.Dash);
                    break;
                case 14:
                    SwitchState(AIState.Dash_Startup_Circle);
                    break;
                case 15:
                    SwitchState(AIState.Dash);
                    break;
                case 16:
                    SwitchState(AIState.OutOfBreath);
                    break;
                case 17:
                    if (_beatCounter < 81)
                        return;
                    SwitchState(AIState.Dash_Startup);
                    break;
                case 18:
                    if (_beatCounter < 85)
                        return;
                    SwitchState(AIState.Dash_Big);
                    break;
                case 19:
                    SwitchState(AIState.OutOfBreath);
                    break;
            }
            _attackSequenceCounter++;
        }

    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            Cycle = 0;
            _localBeatCounter = 0;
            if (State != AIState.Reposition && _targetReposition != Vector2.Zero && state == AIState.Idle)
            {
                SwitchState(AIState.Reposition);
            }
        }
    }


    private void DrawFlame(SpriteBatch spriteBatch)
    {     
        Texture2D flameTexture = ModContent.Request<Texture2D>(Texture + "_Flame").Value;
        Vector2 drawOrigin = flameTexture.Size() / 2f;
        float drawRotation = 0;
        Vector2 drawScale = new Vector2(0.4f, 0.8f);
        Vector2 drawPosition = NPC.Center - Main.screenPosition - Vector2.UnitY * 64;
        Color drawColor = Color.White * Alpha;

        var flameShader = SkullfireShader.Instance;

        spriteBatch.Restart(blendState: BlendState.Additive, effect: flameShader.Effect);
        spriteBatch.Draw(flameTexture, drawPosition, null, drawColor, drawRotation, drawOrigin, drawScale * 0.8f, SpriteEffects.None, 0);
        spriteBatch.Draw(flameTexture, drawPosition, null, drawColor, drawRotation, drawOrigin, drawScale * 0.8f, SpriteEffects.None, 0);
      //  spriteBatch.Draw(flameTexture, drawPosition, null, drawColor * 0.5f, drawRotation, drawOrigin, drawScale * 0.85f, SpriteEffects.None, 0);
     //   spriteBatch.Draw(flameTexture, drawPosition, null, drawColor, drawRotation, drawOrigin, drawScale * 0.75f, SpriteEffects.None, 0);
        spriteBatch.RestartDefaults();
    }


    private void DrawAura(SpriteBatch spriteBatch)
    {
        Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
        Color glowColor = Color.OrangeRed;
        glowColor.A = 0;
        glowColor *= Alpha;
        for (int i = 0; i < 2; i++)
        {
            spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, glowColor, NPC.rotation, new Vector2(32, 32), 0.35f * (5 + 0.6f) * 1.5f, SpriteEffects.None, 0f);
        }
    }

    private void DrawPixelatedTrails(GraphicsDevice gDevice)
    {
        BlackFireShader blackFireShader = BlackFireShader.Instance;
        blackFireShader.InnerColor = Color.Yellow;
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, null, ColorFunction, WidthFunction, blackFireShader, NPC.Size / 2f);

        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, null, ColorFunction2, WidthFunction2, bloomTrailShader, NPC.Size / 2f);

    }

    private void DrawTrail(SpriteBatch spriteBatch)
    {
        FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
        flamingTrailShader.BlendState = BlendState.Additive;
        TrailDrawer.Draw(spriteBatch, NPC.oldPos, NPC.oldRot, ColorFunction, WidthFunction, flamingTrailShader, NPC.Size / 2f);
    }

    public float WidthFunction(float completionRatio)
    {
        float baseWidth = 64;
        return MathHelper.SmoothStep(baseWidth, 0.5f, completionRatio) * _trailInterpolant;
    }
    public float WidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 1.2f;
    }
    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.Yellow, Color.Red, completionRatio) * Alpha;
    }
    public Color ColorFunction2(float completionRatio)
    {
        return Color.Lerp(Color.Yellow, Color.Red, completionRatio) * Alpha * 0.8f;
    }

    private void DrawHand(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!_showHand)
            return;

        Texture2D handTexture = ModContent.Request<Texture2D>(Texture + "_Hand").Value;
        Rectangle handRect = handTexture.GetFrame(_handFrame, 3);
        Vector2 handDrawPosition = _handPosition - screenPos;
        Vector2 drawOrigin = handRect.Size() / 2f;

        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;

        Color outlineColor = HandOutlineColor * Alpha;
        spriteBatch.Restart(effect: whiteShader.Effect);

        float outlineOffset = 2;
        Vector2 left = Vector2.UnitX * -outlineOffset;
        Vector2 right = Vector2.UnitX * outlineOffset;
        Vector2 up = Vector2.UnitY * -outlineOffset;
        Vector2 down = Vector2.UnitY * outlineOffset;
        float drawScale = 1.5f;
        spriteBatch.Draw(handTexture, handDrawPosition + left, handRect, outlineColor, _handDrawRotation, drawOrigin, _scale * drawScale, SpriteEffects.None, 0);
        spriteBatch.Draw(handTexture, handDrawPosition + right, handRect, outlineColor, _handDrawRotation, drawOrigin, _scale * drawScale, SpriteEffects.None, 0);
        spriteBatch.Draw(handTexture, handDrawPosition + up, handRect, outlineColor, _handDrawRotation, drawOrigin, _scale * drawScale, SpriteEffects.None, 0);
        spriteBatch.Draw(handTexture, handDrawPosition + down, handRect, outlineColor, _handDrawRotation, drawOrigin, _scale * drawScale, SpriteEffects.None, 0);

        spriteBatch.RestartDefaults();

        spriteBatch.Draw(handTexture, handDrawPosition, handRect, drawColor * Alpha, _handDrawRotation, drawOrigin, _handDrawScale * drawScale, SpriteEffects.None, 0);
    }

    private void DrawGlowingAura(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        float drawScale = 1;
        Texture2D auraTexture = ModContent.Request<Texture2D>(Texture + "_Aura").Value;
        Texture2D auraTexture2 = ModContent.Request<Texture2D>(Texture + "_Aura2").Value;



        Vector2 auraDrawPos = NPC.Center - screenPos;
        auraDrawPos -= Vector2.UnitY * 16 * ExtraMath.Osc(0f, 1f);
        Vector2 auraDrawOrigin = auraTexture.Size() / 2f;
        Vector2 auraDrawOrigin2 = auraTexture2.Size() / 2f;
        Vector2 auraDrawScale = Vector2.One * 0.75f;
        spriteBatch.Restart(blendState: BlendState.Additive);

        float auraDrawRotation = Main.GlobalTimeWrappedHourly * 0.4f;
        spriteBatch.Draw(auraTexture2, auraDrawPos, null, Color.White * Alpha * _auraInterpolant, auraDrawRotation, auraDrawOrigin2, auraDrawScale * drawScale, SpriteEffects.None, 0);
        spriteBatch.Draw(auraTexture, auraDrawPos, null, Color.White * Alpha * _auraInterpolant, -auraDrawRotation * 0.5f, auraDrawOrigin, auraDrawScale * drawScale, SpriteEffects.None, 0);
        
        spriteBatch.RestartDefaults();
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        float drawScale = 1.5f;
        DrawFlame(spriteBatch);
        DrawAura(spriteBatch);
        DrawGlowingAura(spriteBatch, screenPos);

        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrails, DrawLayer.BehindNPCsWithOutline);
        //DrawTrail(spriteBatch);
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawPos = NPC.position - screenPos + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);


        float outlineOffset = 2;
        Vector2 left = Vector2.UnitX * -outlineOffset;
        Vector2 right = Vector2.UnitX * outlineOffset;
        Vector2 up = Vector2.UnitY * -outlineOffset;
        Vector2 down = Vector2.UnitY * outlineOffset;
        SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;

        Color outlineColor = OutlineColor * Alpha;
        Vector2 drawOrigin = NPC.frame.Size() / 2;
        spriteBatch.Restart(effect: whiteShader.Effect);


        spriteBatch.Draw(texture, drawPos + left, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale * drawScale, spriteEffects, 0);
        spriteBatch.Draw(texture, drawPos + right, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale * drawScale, spriteEffects, 0);
        spriteBatch.Draw(texture, drawPos + up, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale * drawScale, spriteEffects, 0);
        spriteBatch.Draw(texture, drawPos + down, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale * drawScale, spriteEffects, 0);

        spriteBatch.RestartDefaults();

  

        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 oldPos = NPC.oldPos[i];
            Vector2 oldDrawPos = oldPos - Main.screenPosition;
            oldDrawPos += NPC.Size / 2;
            float f = i;
            float interpolant = f / (float)NPC.oldPos.Length;
            Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant) * 0.12f;
            fadeColor *= Alpha;

            spriteBatch.Draw(texture, oldDrawPos, NPC.frame, fadeColor, NPC.oldRot[i], drawOrigin, drawScale, spriteEffects, 0f);
        }



        spriteBatch.Draw(texture, drawPos, NPC.frame, Color.White.MultiplyRGB(drawColor) * Alpha, NPC.rotation, drawOrigin, _scale * drawScale, spriteEffects, 0);

        DrawHand(spriteBatch, screenPos, drawColor);
        return false;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
    }
}
