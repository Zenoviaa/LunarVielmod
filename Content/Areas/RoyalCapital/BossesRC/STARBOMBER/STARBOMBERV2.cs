
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles;
using Stellamod.Content.Buffs;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.InverseKinematics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.STARBOMBER.Projectiles;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER
{
    public enum RotationStyle : byte
    {
        Inverse = 0,
        ForwardWalk = 1,
        InverseLerp = 2,
        Forward = 3
    }
    public class STARBOMBERLegs
    {
        private Armature _leg1;
        private Armature _leg2;

        public Armature LeftLeg
        {
            get
            {
                if (_leg1 == null)
                {
                    _leg1 = new Armature();
                    SetConstraints();
                    SetInitialAngles();
                }

                return _leg1;
            }
        }

        public Armature RightLeg
        {
            get
            {
                if (_leg2 == null)
                {
                    _leg2 = new Armature();
                    SetConstraints();
                    SetInitialAngles();
                }


                return _leg2;
            }
        }


        public LegData leftLegData;
        public LegData rightLegData;
        private void SetInitialAngles()
        {
            LeftLeg.SetDefaults();
            RightLeg.SetDefaults();
        }

        private void SetConstraints()
        {
            LeftLeg.segments[0].rootDirection = -Vector2.UnitY.RotatedBy(-MathHelper.ToRadians(30));
            LeftLeg.segments[0].rangeOfMotion = 0f;

            RightLeg.segments[0].rootDirection = -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(30));
            RightLeg.segments[0].rangeOfMotion = 0f;


            float downRangeOfMotion = -0.5f;
            LeftLeg.segments[1].rootDirection = Vector2.UnitY;
            LeftLeg.segments[1].rangeOfMotion = downRangeOfMotion;

            RightLeg.segments[1].rootDirection = Vector2.UnitY;
            RightLeg.segments[1].rangeOfMotion = downRangeOfMotion;

        }

        public bool CanMoveFoot()
        {
            return leftLegData.rotationStyle != RotationStyle.ForwardWalk && rightLegData.rotationStyle != RotationStyle.ForwardWalk;
        }
        public void MoveFoot(ref LegData legData, Vector2 targetFootPosition)
        {
            SoundStyle walkingStartSound = AssetRegistry.Sounds.STARBOMBER.STARWALK;
            walkingStartSound.PitchVariance = 0.3f;
            walkingStartSound.Volume = 0.25f;
            SoundEngine.PlaySound(walkingStartSound, targetFootPosition);

            legData.footPosition = targetFootPosition;
            legData.timer = 0f;
            legData.duration = 22;
            legData.startWalkPosition = legData.footPosition;
            legData.endWalkPosition = targetFootPosition;
            legData.rotationStyle = RotationStyle.ForwardWalk;
        }
        public void MoveFootBeeline(ref LegData legData, Vector2 targetFootPosition, float duration)
        {
            SoundStyle walkingStartSound = AssetRegistry.Sounds.STARBOMBER.STARWALK;
            walkingStartSound.PitchVariance = 0.3f;
            walkingStartSound.Volume = 0.25f;
            SoundEngine.PlaySound(walkingStartSound, targetFootPosition);

    
            legData.timer = 0f;
            legData.duration = duration;
            legData.startWalkPosition = legData.footPosition;
            legData.endWalkPosition = targetFootPosition;
            legData.rotationStyle = RotationStyle.InverseLerp;
    
        }

        public void CalculateWalkAngles(ref LegData legData, Armature leg)
        {
            legData.timer++;
            float progress = legData.timer / legData.duration;

            //Maybe some easing to help it out
            progress = Easing.InOutSine(progress);

            //Lerp to the default angles
            float newAngle = leg.segments[0].GetDefaultAngle();
            leg.segments[0].angle = Utils.AngleLerp(leg.segments[0].oldAngle, newAngle, progress);

            newAngle = leg.segments[1].GetDefaultAngle();
            leg.segments[1].angle = Utils.AngleLerp(leg.segments[1].oldAngle, newAngle, progress);


            if (legData.timer >= legData.duration)
            {
                legData.timer = 0f;
                legData.startWalkPosition = leg.GetEndEffector();
                legData.rotationStyle = RotationStyle.InverseLerp;
            }
        }


        public void ConstantLerpToDefaultAngles(Armature leg)
        {
            //Lerp to the default angles
            float newAngle = leg.segments[0].GetDefaultAngle();
            leg.segments[0].angle = Utils.AngleLerp(leg.segments[0].angle, newAngle, 0.1f);

            newAngle = leg.segments[1].GetDefaultAngle();
            leg.segments[1].angle = Utils.AngleLerp(leg.segments[1].angle, newAngle, 0.1f);
        }

        public void ConstantLerpToStraightAngles(Armature leg)
        {
            //Lerp to the default angles
            float newAngle = leg.segments[0].GetDefaultAngle();
            leg.segments[0].angle = Utils.AngleLerp(leg.segments[0].angle, Vector2.UnitY.ToRotation(), 0.2f);

            newAngle = leg.segments[1].GetDefaultAngle();
            leg.segments[1].angle = Utils.AngleLerp(leg.segments[1].angle, Vector2.UnitY.ToRotation(), 0.2f);
        }

        public void ConstantLerpAngles(Armature leg, float thighAngle, float kneeAngle)
        {
            //Lerp to the default angles
            float newAngle = leg.segments[0].GetDefaultAngle();
            leg.segments[0].angle = Utils.AngleLerp(leg.segments[0].angle, thighAngle, 0.05f);
            newAngle = leg.segments[1].GetDefaultAngle();
            leg.segments[1].angle = Utils.AngleLerp(leg.segments[1].angle, kneeAngle, 0.05f);
        }

        public void UpdateLeg(ref LegData legData, Armature leg)
        {
            switch (legData.rotationStyle)
            {
                case RotationStyle.InverseLerp:
                    legData.timer++;
    
                    float time = legData.duration;
                    float completionRatio = legData.timer / time;
                    completionRatio = Easing.InOutSine(completionRatio);
                    Vector2 position = Vector2.Lerp(legData.startWalkPosition, legData.footPosition, completionRatio);
                    leg.IK(legData.rootPosition, position);
                    if (legData.timer >= time)
                    {

                        legData.timer = 0;
                        legData.rotationStyle = RotationStyle.Inverse;

                        SoundStyle walkingStartSound = AssetRegistry.Sounds.STARBOMBER.STARSTEP;
                        walkingStartSound.PitchVariance = 0.3f;
                        walkingStartSound.Pitch = -0.5f;
                        walkingStartSound.Volume = 0.25f;
                        SoundEngine.PlaySound(walkingStartSound, position);
                    }
                    break;

                case RotationStyle.Inverse:
                    leg.IK(legData.rootPosition, legData.footPosition);
                    break;

                case RotationStyle.ForwardWalk:
                    CalculateWalkAngles(ref legData, leg);
                    leg.FK(legData.rootPosition);
                    break;
                case RotationStyle.Forward:
                    leg.FK(legData.rootPosition);
                    break;
            }
        }

        public bool IsValidFootPosition(ref LegData legData, float centerX, float standRange)
        {
            float xDist = MathF.Abs(legData.footPosition.X - centerX);
            float fluff = -64;
            return xDist <= standRange + fluff;
        }

        public void Update()
        {
            UpdateLeg(ref leftLegData, LeftLeg);
            UpdateLeg(ref rightLegData, RightLeg);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D[] textures, Color drawColor)
        {
            //Debug drawing
            LeftLeg.DrawLikeLeg(spriteBatch, textures, drawColor);
            RightLeg.DrawLikeLeg(spriteBatch, textures, drawColor);
        }
        public void DrawOutlines(SpriteBatch spriteBatch, Texture2D[] textures, Color drawColor)
        {
            //Debug drawing
            LeftLeg.DrawLikeLegOutlines(spriteBatch, textures, drawColor);
            RightLeg.DrawLikeLegOutlines(spriteBatch, textures, drawColor);
        }
    }

    public class STARBOMBERV2 : ScarletBoss,
        IDrawOutlines
    {
        private float _gunHoldInterpolant;
        private float _realSpinSpeed;
        private float _afterImageTime;
        private float _oscTimer;
        private float _deathLerp;
        private float _spinTelegraphLerp;

        private bool _contactDamage;
        private bool _namePlate;
        private bool _playedSound;
        private bool[] _requestLegMovement = new bool[4];
        private bool _aggroed;
        private LegsState _legsState;
        private int _starFieldFrameCounter;
        private int _starFieldFrameTick;

        private Color _outlineColor;
        private Color _gunOutlineColor;
        private Vector2 _shakeOffset;
        private Vector2 _startFootPosition;
        private Vector2 _impactFootPosition;
        private Vector2 _squishScale;
        private Vector2[] _lightningPos;
        private Vector2[] LightningPos
        {
            get
            {
                _lightningPos ??= new Vector2[32];
                for (int i = 0; i < _lightningPos.Length; i++)
                {
                    float f = i;
                    float length = _lightningPos.Length;
                    float completionRatio = f / length;
                    _lightningPos[i] = Vector2.Lerp(NPC.Center, GunHoistPosition + Vector2.UnitY * 48, completionRatio);
                }
                return _lightningPos;
            }
        }
        private Vector2 _targetWalkPosition;
        private PatternManager<AIState> _patternManager;

        private STARBOMBERGUN _machineGun;
        private STARBOMBERGUN _missileLauncher;
        private STARBOMBERGUN _whistleGun;
        private STARBOMBERGUN _wingSniper;
        private STARBOMBERLegs _legs;
        private STARBOMBERLegs _legs2;
        private int _frame;

        private Color _gunSilhouetteColor;

        private enum AIState
        {
            Spawn,
            Idle,
            MachineGun_Start,
            MachineGun_Loop,
            MachineGun_End,

            LegUpSpin_Start,
            LegUpSpin_Loop,
            LegUpSpin_End,

            WalkUpStomp_Start,
            WalkUpStomp_Stomp,
            WalkUpStomp_End,

            MissileLauncher_Start,
            MissileLauncher_Loop,
            MissileLauncher_End,

            SteamWhistle_Start,
            SteamWhistle_Loop,
            SteamWhistle_End,

            WingTimeSnipe_Start,
            WingTimeSnipe_End,

            CrashJump_Start,
            CrashJump_Loop,
            CrashJump_Crash,
            Despawn,
            Death
        }

        private STARBOMBERLegs Legs
        {
            get
            {
                _legs ??= new STARBOMBERLegs();
                return _legs;
            }
        }
        private STARBOMBERLegs Legs2
        {
            get
            {
                _legs2 ??= new STARBOMBERLegs();
                return _legs2;
            }
        }
        private float LegRadius => 80;

        private Vector2 _gunShootTargetPosition;
        private Vector2 _gunShootTrackingVelocity;
        private Vector2 _leftLegOffset;
        private Vector2 _rightLegOffset;
        private Vector2 LeftLegRootPosition
        {
            get
            {
                return NPC.Center - Vector2.UnitX * LegRadius + _leftLegOffset;
            }
        }
        private Vector2 RightLegRootPosition
        {
            get
            {
                return NPC.Center + Vector2.UnitX * LegRadius + _rightLegOffset;
            }
        }
        private float SpinSpeed;

        private int WalkUpStompDamage => 100;
        private int SteamLaserDamage => 150;
        private int CrashDamage => 70;
        private int StarMissileDamage => 30;
        private int MachineGunDamage => 30;
        private int WingSnipeDamage => 150;
        private ref float Timer => ref NPC.ai[0];
        private Color TargetOutlineColor;
        private Color TargetGunOutlineColor;
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackCycle => ref NPC.ai[2];
        public STARBOMBERGUN MachineGun
        {
            get
            {
                _machineGun ??= new STARBOMBERGUN(Texture + "_MachineGun");
                return _machineGun;
            }
        }

        public STARBOMBERGUN MissileLauncher
        {
            get
            {
                _missileLauncher ??= new STARBOMBERGUN(Texture + "_MissileLauncher");
                return _missileLauncher;
            }
        }
        public STARBOMBERGUN WhistleGun
        {
            get
            {
                _whistleGun ??= new STARBOMBERGUN(Texture + "_WhistleGun");
                return _whistleGun;
            }
        }

        public STARBOMBERGUN WingSniper
        {
            get
            {
                _wingSniper ??= new STARBOMBERGUN(Texture + "_WingSniper");
                return _wingSniper;
            }
        }

        public STARBOMBERGUN HeldGun;
        public Vector2 GunHoistPosition
        {
            get
            {
                return NPC.Center + Vector2.UnitY * 170 * GunVDirection;
            }
        }
        public Vector2 GunMuzzlePosition
        {
            get
            {
                return HeldGun.GetMuzzlePosition(GunPosition, GunDirection);
            }
        }

        public Vector2 LeftFootPosition;
        public Vector2 RightFootPosition;
        public Vector2 GunPosition;
        public Vector2 GunDirection;
        public float GunVDirection;
        public float StandHeight;
        public float StandRange;

        public Texture2D[] LegTextures;
        public Asset<Texture2D> ThighTexture => ModContent.Request<Texture2D>(Texture + "_Thigh");
        public Asset<Texture2D> KneeTexture => ModContent.Request<Texture2D>(Texture + "_Knee");
        public Asset<Texture2D> LegTexture => ModContent.Request<Texture2D>(Texture + "_Leg");
        public Asset<Texture2D> FootTexture => ModContent.Request<Texture2D>(Texture + "_Foot");
        public Asset<Texture2D> StarfieldTexture => ModContent.Request<Texture2D>(Texture + "_Starfield");
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 60;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _squishScale = Vector2.One;
            NPC.width = 128;
            NPC.height = 200;
            NPC.damage = 100;
            NPC.defense = 20;
            NPC.lifeMax = 18000;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Boss6");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f};
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += _realSpinSpeed;
            while (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter -= 1f;
                if (_frame >= Main.npcFrameCount[Type])
                {
                    _frame = 0;
                }
            }
     

            NPC.frame.Y = frameHeight * _frame;
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

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_targetWalkPosition);
            writer.Write(_contactDamage);
            writer.Write((byte)_legsState);
            writer.Write(_aggroed);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _targetWalkPosition = reader.ReadVector2();
            _contactDamage = reader.ReadBoolean();
            _legsState = (LegsState)reader.ReadByte();
            _aggroed = reader.ReadBoolean();
        }

        public override void AI()
        {
         
            _gunSilhouetteColor = Color.Lerp(Color.White, Color.Black, 0.75f);
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            _gunOutlineColor = Color.Lerp(_gunOutlineColor, TargetGunOutlineColor, 0.1f);
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget && State != AIState.Despawn)
                {
                    SwitchState(AIState.Despawn);
                }
            }
            _shakeOffset *= 0.9f;
            _realSpinSpeed = MathHelper.Lerp(_realSpinSpeed, SpinSpeed, 0.1f);
            _afterImageTime *= 0.9f;
            HeldGun?.Update();
            _oscTimer++;
            float osc = _oscTimer * 0.05f;
            float i = (MathF.Sin(osc) + 0.5f) / 0.5f;
            StandHeight = MathHelper.Lerp(270, 300, i);
            MyTarget.AddBuff(ModContent.BuffType<BurnedWings>(), 2);
            if (NPC.collideX)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            }
            switch (State)
            {
                case AIState.Despawn:
                    AI_Despawn();
                    break;
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.MachineGun_Start:
                    AI_MachineGunStart();
                    break;
                case AIState.MachineGun_Loop:
                    AI_MachineGunLoop();
                    break;
                case AIState.MachineGun_End:
                    AI_MachineGunEnd();
                    break;
                case AIState.LegUpSpin_Start:
                    AI_LegUpSpinStart();
                    break;
                case AIState.LegUpSpin_Loop:
                    AI_LegUpSpinLoop();
                    break;
                case AIState.LegUpSpin_End:
                    AI_LegUpSpinEnd();
                    break;
                case AIState.CrashJump_Start:
                    AI_CrashJumpStart();
                    break;
                case AIState.CrashJump_Loop:
                    AI_CrashJumpLoop();
                    break;
                case AIState.CrashJump_Crash:
                    AI_CrashJumpCrash();
                    break;
                case AIState.WalkUpStomp_Start:
                    AI_WalkUpStompStart();
                    break;
                case AIState.WalkUpStomp_Stomp:
                    AI_WalkUpStompStomp();
                    break;
                case AIState.WalkUpStomp_End:
                    AI_WalkUpStompEnd();
                    break;
                case AIState.SteamWhistle_Start:
                    AI_SteamWhistleStart();
                    break;
                case AIState.SteamWhistle_Loop:
                    AI_SteamWhistleLoop();
                    break;
                case AIState.SteamWhistle_End:
                    AI_SteamWhistleEnd();
                    break;
                case AIState.MissileLauncher_Start:
                    AI_MissileLauncherStart();
                    break;
                case AIState.MissileLauncher_Loop:
                    AI_MissileLauncherLoop();
                    break;
                case AIState.MissileLauncher_End:
                    AI_MissileLauncherEnd();
                    break;
                case AIState.WingTimeSnipe_Start:
                    AI_WingTimeSnipeStart();
                    break;
                case AIState.WingTimeSnipe_End:
                    AI_WingTimeSnipeEnd();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;
            }

            UpdateLegs();
            base.AI();
        }



        private void ChooseAttack()
        {
            if (!MultiplayerHelper.IsHost)
                return;
            if (_patternManager == null)
            {
                _patternManager = new PatternManager<AIState>(
                    new Tuple<AIState, float>(AIState.MachineGun_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.LegUpSpin_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.WalkUpStomp_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.MissileLauncher_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.SteamWhistle_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.CrashJump_Start, 1.0f));

            }

            SwitchState(_patternManager.NextPattern());
          //  SwitchState(AIState.LegUpSpin_Start);
        }

        private void SpawnSteamParticle()
        {
            Vector2 spawnPosition = NPC.Top;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.Zero;
            spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            var steamParticle = Particle.NewParticle<BlackSmokeParticle>(spawnPosition, spawnVelocity, Scale: spawnScale);
            steamParticle.innerColor = Color.DarkGray;
            steamParticle.outerColor = Color.Black;
            steamParticle.fadeToColor = Color.Black;
        }

        #region Walking Code

        private void StretchLegs()
        {
            int oldDirection = NPC.direction;
            float oldStandRange = StandRange;
            StandRange *= 0.5f;
            NPC.direction = 0;

            _requestLegMovement[0] = Legs.leftLegData.rotationStyle == RotationStyle.Inverse ? true : false;
            _requestLegMovement[1] = Legs.rightLegData.rotationStyle == RotationStyle.Inverse ? true : false;
            _requestLegMovement[2] = Legs2.leftLegData.rotationStyle == RotationStyle.Inverse ? true : false;
            _requestLegMovement[3] = Legs2.rightLegData.rotationStyle == RotationStyle.Inverse ? true : false;

            NPC.direction = oldDirection;
            StandRange = oldStandRange;
        }

        private bool NeedToMoveLeg(STARBOMBERLegs legs, ref LegData legData, float xCenter, ref bool stretchLegs)
        {
            if (legs.IsValidFootPosition(ref legData, xCenter, StandRange) && !stretchLegs)
                return false;
            if (legData.rotationStyle != RotationStyle.Inverse)
                return false;
            if (LegsInMotionCount() >= 3)
                return false;
            return true;
        }

        private int LegsInMotionCount()
        {
            int count = 0;
            count += Legs.leftLegData.rotationStyle == RotationStyle.ForwardWalk ? 1 : 0;
            count += Legs.rightLegData.rotationStyle == RotationStyle.ForwardWalk ? 1 : 0;
            count += Legs2.leftLegData.rotationStyle == RotationStyle.ForwardWalk ? 1 : 0;
            count += Legs2.rightLegData.rotationStyle == RotationStyle.ForwardWalk ? 1 : 0;
            return count;
        }

        private void UpdateLegs()
        {
            Legs.leftLegData.rootPosition = LeftLegRootPosition;
            Legs.rightLegData.rootPosition = RightLegRootPosition;

            Legs2.leftLegData.rootPosition = LeftLegRootPosition;
            Legs2.rightLegData.rootPosition = RightLegRootPosition;


            switch (_legsState)
            {
                case LegsState.Walk:
                    if(Legs.leftLegData.rotationStyle == RotationStyle.Forward)
                    {

  
                        Legs.leftLegData.rotationStyle = RotationStyle.Inverse;
                        Legs.rightLegData.rotationStyle = RotationStyle.Inverse;
                        Legs2.leftLegData.rotationStyle = RotationStyle.Inverse;
                        Legs2.rightLegData.rotationStyle = RotationStyle.Inverse;

                    }


                    if (NeedToMoveLeg(Legs, ref Legs.leftLegData, LeftLegRootPosition.X, ref _requestLegMovement[0]))
                    {
                 
                        Legs.MoveFoot(ref Legs.leftLegData, FindNewLeftFoot());
                        _requestLegMovement[0] = false;
                    }
                    if (NeedToMoveLeg(Legs, ref Legs.rightLegData, RightLegRootPosition.X, ref _requestLegMovement[1]))
                    {
                        Legs.MoveFoot(ref Legs.rightLegData, FindNewRightFoot());
                        _requestLegMovement[1] = false;
                    }
                    if (NeedToMoveLeg(Legs2, ref Legs2.leftLegData, LeftLegRootPosition.X, ref _requestLegMovement[2]))
                    {
                        Legs2.MoveFoot(ref Legs2.leftLegData, FindNewLeftFoot2());
                        _requestLegMovement[2] = false;
                    }
                    if (NeedToMoveLeg(Legs2, ref Legs2.rightLegData, RightLegRootPosition.X, ref _requestLegMovement[3]))
                    {
                        Legs2.MoveFoot(ref Legs2.rightLegData, FindNewRightFoot2());
                        _requestLegMovement[3] = false;
                    }
                    break;

                case LegsState.LegsUp:
                    Legs.leftLegData.rotationStyle = RotationStyle.Forward;
                    Legs.rightLegData.rotationStyle = RotationStyle.Forward;
                    Legs2.leftLegData.rotationStyle = RotationStyle.Forward;
                    Legs2.rightLegData.rotationStyle = RotationStyle.Forward;

                    Legs.leftLegData.footPosition = Legs.LeftLeg.GetEndEffector();
                    Legs.rightLegData.footPosition = Legs.RightLeg.GetEndEffector();
                    Legs2.leftLegData.footPosition = Legs2.LeftLeg.GetEndEffector();
                    Legs2.rightLegData.footPosition = Legs2.RightLeg.GetEndEffector();

                    LeftFootPosition = Legs.leftLegData.footPosition;
                    RightFootPosition = Legs.rightLegData.footPosition;

                    float leftThighAngle = MathHelper.ToRadians(-100);
                    float leftKneeAngle = MathHelper.ToRadians(-10);

                    float rightThighAngle =  MathHelper.ToRadians(-70);
                    float rightKneeAngle = MathHelper.ToRadians(-130);

                    Legs.ConstantLerpAngles(Legs.LeftLeg, leftThighAngle, leftKneeAngle);
                    Legs.ConstantLerpAngles(Legs.RightLeg, rightThighAngle, rightKneeAngle);

                    Legs2.ConstantLerpAngles(Legs2.LeftLeg, leftThighAngle, leftKneeAngle);
                    Legs2.ConstantLerpAngles(Legs2.RightLeg, rightThighAngle, rightKneeAngle);
                    break;

                case LegsState.Limp:
                    Legs.ConstantLerpToStraightAngles(Legs.LeftLeg);
                    Legs.ConstantLerpToStraightAngles(Legs.RightLeg);
                    Legs2.ConstantLerpToStraightAngles(Legs2.LeftLeg);
                    Legs2.ConstantLerpToStraightAngles(Legs2.RightLeg);
                    break;
            }
            Legs.Update();

            Legs2.Update();
        }


        private Vector2 FindNewLeftFoot()
        {
            Vector2 groundPoint = FindGround();
            if (NPC.direction == 1)
            {
                return groundPoint - Vector2.UnitX * StandRange + new Vector2(NPC.direction * StandRange, 0);
            }
            else
            {
                return groundPoint - Vector2.UnitX * StandRange;
            }
        }

        private Vector2 FindNewRightFoot()
        {
            Vector2 groundPoint = FindGround();
            if (NPC.direction == -1)
            {
                return groundPoint + Vector2.UnitX * StandRange + new Vector2(NPC.direction * StandRange, 0);
            }
            else
            {
                return groundPoint + Vector2.UnitX * StandRange;

            }

        }
        private Vector2 FindNewLeftFoot2()
        {
            Vector2 groundPoint = FindGround();
            float range = StandRange * 0.75f;
            groundPoint.Y -= 16;
            if (NPC.direction == 1)
            {
                return groundPoint - Vector2.UnitX * range + new Vector2(NPC.direction * StandRange, 0);
            }
            else
            {
                return groundPoint - Vector2.UnitX * range;
            }
        }

        private Vector2 FindNewRightFoot2()
        {
            Vector2 groundPoint = FindGround();
            float range = StandRange * 0.75f;
            groundPoint.Y -= 16;
            if (NPC.direction == -1)
            {
                return groundPoint + Vector2.UnitX * range + new Vector2(NPC.direction * StandRange, 0);
            }
            else
            {
                return groundPoint + Vector2.UnitX * range;
            }

        }

        private Vector2 FindGround()
        {
            Vector2 groundPoint = CollisionHelper.RayCast(NPC.Top, Vector2.UnitY, 2000, 3);
            return groundPoint;
        }


        #endregion

        private Vector2 AimGun()
        {
            Vector2 aimDirection = (MyTarget.Center - GunPosition).SafeNormalize(Vector2.Zero);
            return aimDirection;
        }

        private void PrimeReticle()
        {
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, GunHoistPosition + GunDirection * 1200, Vector2.Zero,
                    ModContent.ProjectileType<AimingReticle>(), 1, 0, Main.myPlayer);
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

        private void AI_Spawn()
        {

            Timer++;
            if (Timer == 1)
            {
                StretchLegs();
            }

            if(Timer % 10 == 0)
            {
                SpawnSteamParticle();
            }

            _spinTelegraphLerp *= 0.5f;
            _legsState = LegsState.Walk;
            StandRange = MathHelper.Lerp(StandRange, 290, 0.1f);
            NPC.velocity.X *= 0.9f;
            NPC.rotation = 0;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.boss = false;
            //ApplyStandingYVelocity();
            if (_aggroed)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void ApplyStandingYVelocity()
        {
            Vector2 groundPoint = FindGround();
            float distanceToGround = Vector2.Distance(NPC.Center, groundPoint);
            if (distanceToGround > StandHeight)
            {
                NPC.velocity.Y += 0.5f;
            }
            else if (distanceToGround < StandHeight / 2f)
            {
                NPC.velocity.Y -= 0.5f;
            }
            else
            {
                float yOscVelocity = MathF.Sin(_oscTimer * 0.02f) * 0.5f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, yOscVelocity, 0.1f);
            }

        }
        private void AI_Idle()
        {
            NPC.boss = true;
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();

            }
        
            if(Timer == 5)
            {
                StretchLegs();
            }
            NPC.direction = TargetDirection;

            if (!_namePlate)
            {
                ShowNamePlate();
                _namePlate = true;
            }
            HeldGun = null;
            _legsState = LegsState.Walk;
            _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);

            //Set some defaults
            GunVDirection = -1;
            _contactDamage = false;
            AttackCycle = 0;
            SpinSpeed = 0.5f;
            _spinTelegraphLerp *= 0.5f;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            float targetRotation = NPC.velocity.X * 0.05f;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.03f);
            NPC.rotation = MathHelper.WrapAngle(NPC.rotation);
            StandRange = MathHelper.Lerp(StandRange, 290, 0.1f);
            TargetGunOutlineColor = Color.Transparent;
            TargetOutlineColor = Color.Transparent;

            //Fun hover code
            //Also make sure we get bro to the ground
            float timeToWait = 300;
            ApplyStandingYVelocity();

            //Walk?
            float xDist = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
            if (xDist > StandRange)
            {
                float speed = MathHelper.Lerp(2, 3, MathHelper.Clamp(xDist / 16f, 0f, 1f));
                float targetX = NPC.direction * speed;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetX, 0.1f);

            }
            else
            {
                float targetX = NPC.direction * 2f;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetX, 0.1f);
            }


            if (Timer >= timeToWait)
            {
                ChooseAttack();
            }
        }
        private void AI_Death()
        {
            Timer++;
            if(Timer == 1)
            {
                _playedSound = false;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/STARDEATH"));
            }

            if(Timer % 5 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<TSmokeDust>(), newColor: Color.Gray);
                NPC.rotation = _shakeOffset.ToRotation();
            }

            Vector2 velocity = Main.rand.NextVector2Circular(64, 64);
            _shakeOffset = Main.rand.NextVector2Circular(24 * _deathLerp, 24 * _deathLerp);
            _deathLerp = Timer / 462f;
            SpinSpeed = MathHelper.Lerp(1f, 16, _deathLerp);
            TargetOutlineColor = Color.Lerp(Color.Transparent, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 4));

            if (Timer >= 462)
            {
                MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                myPlayer.ShakeAtPosition(NPC.position, 6000, 128);
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<STARBOMBERBOOM>(), 50, 2, Main.myPlayer);
                }

                for (int i = 0; i < 16; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(),
                        (Vector2.One * Main.rand.Next(5, 15)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }
                for (float f = 0; f < 12; f++)
                {
                    Vector2 v = Main.rand.NextVector2Circular(128, 128);
                    FXUtil.GlowStretch(NPC.Center, velocity);
                }
                FXUtil.ShakeCamera(NPC.position, 1024, 8);
                ShakeModSystem.Shake = 8;
                var p = FXUtil.GlowCircleBoom(NPC.Center, Color.Pink, Color.Purple, Color.Blue);
                p.Scale *= 12;
                //Death Effect here
                NPC.Kill();
            }

            _legsState = LegsState.LegsUp;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.velocity.X *= 0.8f;
            NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);

            if (_playedSound)
            {
                //Lets make them legs fall off
                float lerp = MathHelper.Clamp(Timer / 240f, 0f, 1f);
                _leftLegOffset.Y += MathHelper.SmoothStep(-10, 25, lerp);
                _leftLegOffset.X -= 0.1f;
                _rightLegOffset.Y += MathHelper.SmoothStep(-10, 25, lerp);
                _rightLegOffset.X += 0.1f;
            }
            if (NPC.collideY && !_playedSound)
            {
                SoundStyle clanker = AssetRegistry.Sounds.STARBOMBER.HeavyCrush;
                clanker.PitchVariance = 0.5f;
                SoundEngine.PlaySound(clanker, NPC.position);
                FXUtil.ShakeCamera(NPC.position, 1024, 8);
                _playedSound = true;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            if (!_aggroed)
            {
                _aggroed = true;
                NPC.netUpdate = true;
            }
     
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

        #region MachineGunAttack
        private void AI_MachineGunStart()
        {
            //Pulls out a machine gun and starts shootin,
            //you have to dodge through it pretty often and run away

            //I think i'll make this attack just like sweeping
            Timer++;
            //We using the machine gun brah
            HeldGun = MachineGun;
            TargetGunOutlineColor = Color.Yellow;
            TargetOutlineColor = Color.Yellow;
            if (Timer == 1)
            {
                StretchLegs();
                NPC.TargetClosest();
                NPC.direction = TargetDirection;

                SoundStyle starGun = new SoundStyle("Stellamod/Assets/Sounds/STARGUN");
                starGun.PitchVariance = 0.1f;
                SoundEngine.PlaySound(starGun, NPC.Center);

            }

            if(Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            float prepTime = 90f;

            //Summon the gun        
            //I think it's easier if all of star bombers guns just come from him and aren't projectiles                 
            //Yeah that'll be better, let's represent them
            float interpolant = Timer / prepTime;
            _gunHoldInterpolant = EasingFunction.InOutSine(interpolant);
            Vector2 gunHoistPosition = Vector2.Lerp(NPC.Center, GunHoistPosition, _gunHoldInterpolant);
            GunPosition = gunHoistPosition;
            GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.01f);
            GunVDirection = 1;

            HeldGun.drawColor = Color.Lerp(Color.Transparent, Color.White, _gunHoldInterpolant);
            NPC.velocity.X *= 0.9f;

            ApplyStandingYVelocity();
            if (Timer >= prepTime)
            {
                SwitchState(AIState.MachineGun_Loop);
            }
        }

        private void AI_MachineGunLoop()
        {
            Timer++;
            if (Timer == 1)
            {
                _gunShootTrackingVelocity = -Vector2.UnitY * 5;
                _gunShootTargetPosition = GunMuzzlePosition;
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }
            HeldGun = MachineGun;
            HeldGun.muzzleOffset = 140;
            GunPosition = GunHoistPosition;
            TargetOutlineColor = Color.Yellow;

            Vector2 newVelocity = ProjectileHelper.SimpleHomingVelocity(_gunShootTargetPosition, MyTarget.Center, _gunShootTrackingVelocity, degreesToRotate: 3);
            _gunShootTrackingVelocity = newVelocity;
            _gunShootTargetPosition += _gunShootTrackingVelocity;

            Vector2 directionToTarget = (MyTarget.Center - GunPosition).SafeNormalize(Vector2.Zero);
            float dp = Vector2.Dot(GunDirection, directionToTarget);
            if (Timer % 6 == 0 && Timer < 120 && dp > 0.8f)
            {

                TargetGunOutlineColor = Color.Red;
                NPC.velocity.X = -NPC.direction * 0.5f;
                if (MultiplayerHelper.IsHost)
                {
                    float distanceToShoot = Vector2.Distance(GunMuzzlePosition, _gunShootTargetPosition) + 64;
                    int type = ModContent.ProjectileType<MachineGunBullet>();
                    Projectile.NewProjectile(SourceFromThis, GunMuzzlePosition, GunDirection * 7,
                     type, MachineGunDamage, 1, Main.myPlayer, ai1: distanceToShoot);
                }

                HeldGun.Recoil();
            }


            if (Timer == 120)
            {
                StretchLegs();
                SoundStyle tireSound = new SoundStyle("Stellamod/Assets/Sounds/STARWAVE");
                tireSound.PitchVariance = 0.15f;
                SoundEngine.PlaySound(tireSound, NPC.position);
            }

            if (Timer >= 120)
            {
                _legsState = LegsState.Walk;
                TargetGunOutlineColor = Color.Yellow;
                SpinSpeed = 1;

                if (Timer >= 150)
                {
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 4, 0.1f);
                }
                else
                {
                    NPC.velocity.X *= 0.9f;
                }

                GunDirection = Vector2.Lerp(GunDirection, AimGun(), 0.005f);
                GunVDirection = 1;

                HeldGun.drawColor = Color.Lerp(HeldGun.drawColor, _gunSilhouetteColor, 0.1f);

                if (Timer % 5 == 0)
                {
                    SpawnSteamParticle();
                }

                if (Timer % 16 == 0)
                {
                    Dust.NewDust(GunPosition, 4, 4, ModContent.DustType<TSmokeDust>(), newColor: Color.DarkGray,
                        Scale: Main.rand.NextFloat(0.5f, 1f));
                }

                if (Timer % 16 == 0)
                {
                    Particle.NewParticle<EmberParticle>(GunPosition, Main.rand.NextVector2Circular(1, 4), newColor: Color.Red);
                    if (Main.rand.NextBool(2))
                    {
                        Particle.NewParticle<ZapParticle>(GunPosition + Main.rand.NextVector2Circular(8, 8), Main.rand.NextVector2Circular(1, 1), newColor: Color.Pink, Scale: Main.rand.NextFloat(0.5f, 0.66f));
                    }
                }
            }
            else
            {
                _legsState = LegsState.Limp;
                SpinSpeed = 0.25f;
                NPC.velocity.X *= 0.99f;
                GunDirection = Vector2.Lerp(GunDirection, AimGun(), 0.05f);
                GunVDirection = 1;

                HeldGun.drawColor = Color.Lerp(HeldGun.drawColor, Color.White, 0.1f);
            }

            NPC.rotation = NPC.velocity.X * 0.025f;

            ApplyStandingYVelocity();
            if (Timer >= 240)
            {
                AttackCycle++;
                if (AttackCycle >= 3)
                {
                    SwitchState(AIState.MachineGun_End);
                }
                else
                {
                    //This looks weird, but we're just restarting the state lol
                    SwitchState(AIState.MachineGun_Loop);
                }
            }
        }

        private void AI_MachineGunEnd()
        {
            _legsState = LegsState.Walk;
            Timer++;
            HeldGun = MachineGun;
            TargetOutlineColor = Color.Transparent;
            TargetGunOutlineColor = Color.Transparent;
            if (Timer == 1)
            {
                SoundStyle tireSound = new SoundStyle("Stellamod/Assets/Sounds/STARWAVE");
                tireSound.PitchVariance = 0.15f;
                tireSound.Pitch = -0.5f;
                SoundEngine.PlaySound(tireSound, NPC.position);
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }

            float prepTime = 90f;

            //Summon the gun        
            //I think it's easier if all of star bombers guns just come from him and aren't projectiles                 
            //Yeah that'll be better, let's represent them
            float interpolant = Timer / prepTime;
            _gunHoldInterpolant = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(interpolant));
            Vector2 gunHoistPosition = Vector2.Lerp(NPC.Center, GunHoistPosition, _gunHoldInterpolant);
            GunPosition = gunHoistPosition;
            GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.1f);
            HeldGun.drawColor = Color.Lerp(Color.Transparent, Color.White, _gunHoldInterpolant);
            if (Timer == prepTime)
            {
                FXUtil.GlowCircleBoom(gunHoistPosition + Vector2.UnitY * 40, Color.White, Color.Pink, Color.Blue);
            }

            NPC.velocity.X *= 0.9f;
            ApplyStandingYVelocity();
            if (Timer >= prepTime)
            {

                SwitchState(AIState.Idle);
            }
        }

        #endregion


        #region LegUpSpin
        private void AI_LegUpSpinStart()
        {
            /*
             * 
             * Legs go up and his head drops to the ground, 
             * starts spinning and spins across the floor really fast
             */

            TargetOutlineColor = Color.Yellow;
            Timer++;
            if (Timer == 1)
            {
                _playedSound = false;
                SoundStyle tireSound = new SoundStyle("Stellamod/Assets/Sounds/STARBOMBERWAKE");
                tireSound.PitchVariance = 0.15f;
                SoundEngine.PlaySound(tireSound, NPC.position);
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }
            _legsState = LegsState.LegsUp;
    
            NPC.velocity.X *= 0.9f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;

            float prepTime = 90f;
            SpinSpeed = MathHelper.Lerp(0f, 2f, Timer / prepTime);
            if (NPC.collideY && !_playedSound)
            {
                _playedSound = true;
                SoundStyle impact = AssetRegistry.Sounds.STARBOMBER.Ommove3;
                SoundEngine.PlaySound(impact, NPC.position);
                FXUtil.ShakeCamera(NPC.position, 2000, 8);
                FXUtil.PunchCamera(NPC.position, Vector2.UnitY, 8, 8, 8);
            }
            if (Timer >= prepTime && NPC.collideY)
            {
               
                SwitchState(AIState.LegUpSpin_Loop);
            }
        }

        private void AI_LegUpSpinLoop()
        {
            _legsState = LegsState.LegsUp;
            _afterImageTime = MathHelper.Lerp(_afterImageTime, 1, 0.3f);
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            Timer++;
            if (Timer == 1)
            {
              
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
                SoundStyle spin = AssetRegistry.Sounds.STARBOMBER.Heavyspin;
                SoundEngine.PlaySound(spin, NPC.position);
            }
            _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);

            float spinTime = 150;
            float spinSpeed = 35;
            float targetSpinVelocity = NPC.direction * spinSpeed;

            SpinSpeed = MathHelper.Lerp(3, 0.2f, Timer / spinTime);
            NPC.rotation = NPC.velocity.X * 0.015f;
            if(Timer == 25)
            {
                SoundStyle spin = AssetRegistry.Sounds.STARBOMBER.Ommove1;
                spin.PitchVariance = 0.2f;
                SoundEngine.PlaySound(spin, NPC.position);
            }
            if (Timer >= spinTime / 4f)
            {
                NPC.velocity.X *= 0.9f;
                _afterImageTime *= 0.9f;
            }
            else
            {
                _afterImageTime = MathHelper.Lerp(_afterImageTime, 1f, 0.1f);
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetSpinVelocity, 0.3f);
                if(Timer % 2 == 0)
                {
                    var part = FXUtil.GlowCircleDetailedBoom1(NPC.Bottom, Color.Yellow, Color.Orange, Color.DarkRed);
                    part.Scale *= 0.5f;
                    part.Rotation = Main.rand.NextFloat(-1f, 1f);
                }
            }

            if(Timer == 100)
            {
                _squishScale = new Vector2(0.9f, 1.2f);
                NPC.velocity.Y = -5;
            }
            if(Timer >= spinTime / 1.5f)
            {
                float normTime = Timer - spinTime / 1.5f;
                float denom = spinTime - spinTime / 1.5f;
                float prog = normTime / denom;
                _spinTelegraphLerp = prog;
            }
            else
            {
                _spinTelegraphLerp *= 0.5f;
            }
              
            NPC.velocity.Y -= 0.1f;

            if (Timer >= spinTime && NPC.collideY || Timer >= 200)
            {
                _squishScale = new Vector2(1.5f, 0.9f);
                SoundStyle impact = AssetRegistry.Sounds.STARBOMBER.HeavyCrush;
                SoundEngine.PlaySound(impact, NPC.position);
                FXUtil.PunchCamera(NPC.position, Vector2.UnitY, 8, 8, 8);
                var donut = Particle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
                Timer = 0;
                AttackCycle++;
                if (AttackCycle >= 5)
                {
                    SwitchState(AIState.LegUpSpin_End);
                }
            }
        }

        private void AI_LegUpSpinEnd()
        {
            _afterImageTime *= 0.1f;
            _contactDamage = false;
            _legsState = LegsState.Walk;
            _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
            SpinSpeed = 1f;

            Timer++;
            if(Timer == 5)
            {
                StretchLegs();
            }
            TargetOutlineColor = Color.Transparent;
            TargetGunOutlineColor = Color.Transparent;

            NPC.velocity.X *= 0.9f;
            if (Timer >= 60)
            {
                SwitchState(AIState.Idle);
            }
        }
        #endregion


        #region Crash Jump
        private void AI_CrashJumpStart()
        {
            TargetOutlineColor = Color.Yellow;
            Timer++;
            _legsState = LegsState.LegsUp;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;

                SoundStyle jumpPrepare = AssetRegistry.Sounds.STARBOMBER.Ommove5;
                jumpPrepare.PitchVariance = 0.2f;
                SoundEngine.PlaySound(jumpPrepare, NPC.position);
                _playedSound = false;
            }

            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            if (!_playedSound && NPC.collideY)
            {
                SoundStyle jumpPrepare2 = AssetRegistry.Sounds.STARBOMBER.Ommove3;
                jumpPrepare2.PitchVariance = 0.2f;
                SoundEngine.PlaySound(jumpPrepare2, NPC.position);
                FXUtil.ShakeCamera(NPC.position, 1024, 8);
                FXUtil.PunchCamera(NPC.position, Vector2.UnitY, 8, 8, 8);

                _playedSound = true;
            }

            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.velocity.X *= 0.8f;
            NPC.rotation = NPC.velocity.X * 0.02f;
            if (Timer >= 60 && NPC.collideY)
            {
    
                SwitchState(AIState.CrashJump_Loop);
            }
         
        }

        private void AI_CrashJumpLoop()
        {
            TargetOutlineColor = Color.Yellow;
            Timer++;

      
            _afterImageTime = MathHelper.Lerp(_afterImageTime, 0.3f, 0.1f);
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
                NPC.velocity.Y = -22;

                SoundStyle jumpSound = AssetRegistry.Sounds.STARBOMBER.Ommove2;
                jumpSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(jumpSound, NPC.position);

                SoundStyle fallSound = AssetRegistry.Sounds.Bishinine.BishinineFastfall;
                fallSound.PitchVariance = 0.1f;
                fallSound.Pitch = -0.5f;
                SoundEngine.PlaySound(fallSound, NPC.position);
            }


            if(Timer < 80)
            {
                _legsState = LegsState.Limp;
                NPC.noGravity = false;
                OffsetCameraModifier.FocusTargetOffset = -Vector2.UnitY * 400;
            }
            else if (NPC.velocity.Y > 0)
            {
  
                _legsState = LegsState.LegsUp;
                NPC.velocity.Y *= 1.07f;
                NPC.noGravity = true;

                if(Timer % 5 == 0)
                {
                    var p = Particle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
                    var p2 = Particle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY * 4);
                    p2.Scale *= 0.5f;
                }
                if(Timer % 2 == 0)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height
                        , ModContent.DustType<GlyphDust>(), newColor: Color.Pink, Scale: Main.rand.NextFloat(0.5f, 1f));
                }
            }
             
            NPC.noTileCollide = false;
 
            float targetVelocity = NPC.direction * 5;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity, 0.01f);
            if (NPC.collideY && Timer >= 10)
            {
                SwitchState(AIState.CrashJump_Crash);
            }
        }

        private void AI_CrashJumpCrash()
        {
            _contactDamage = true;
            _afterImageTime = MathHelper.Lerp(_afterImageTime, 0.8f, 0.1f);
            Timer++;
            if (Timer == 1)
            {
                _squishScale = new Vector2(1f, 0.75f);
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
                SoundStyle crashSoun = AssetRegistry.Sounds.STARBOMBER.HeavyCrush;
                crashSoun.PitchVariance = 0.2f;
                SoundEngine.PlaySound(crashSoun, NPC.position);


                var d1 = Particle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
                d1.Scale *= 4;
                var d2 = Particle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
                d2.Scale *= 8;


                var part = FXUtil.GlowCircleBoom(NPC.Center, Color.Pink, Color.Purple, Color.Blue);
                part.Scale *= 8f;

                part = FXUtil.GlowCircleBoom(NPC.Center, Color.Pink, Color.Purple, Color.Blue);
                part.Scale *= 3f;
                ShakeModSystem.Shake = 6;

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
                    var p = Particle.NewBlackParticle<BlackSmokeParticle>(NPC.Bottom + offset, velocity, Color.DarkGray);
                    p.Scale *= 0.25f;
                    p.color *= 0.5f;
                    p.fadeToColor = Color.Black;
                    p.innerColor = Color.DarkGray;
                    p.outerColor = Color.Black;
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

                FXUtil.ShakeCamera(NPC.position, 2000, 100);
                FXUtil.PunchCamera(NPC.position, Vector2.UnitY, 32, 8, 8);
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Bottom, Vector2.Zero,
                        ModContent.ProjectileType<StompCrashBoom>(), CrashDamage, 1, Main.myPlayer);
                }
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 velocity = Vector2.UnitX * 5;
                    Projectile.NewProjectile(SourceFromThis, NPC.Bottom, velocity,
                        ModContent.ProjectileType<STARSHOTT>(), StarMissileDamage, 1, Main.myPlayer);
                    Projectile.NewProjectile(SourceFromThis, NPC.Bottom, -velocity,
                        ModContent.ProjectileType<STARSHOTT>(), StarMissileDamage, 1, Main.myPlayer);

                    Projectile.NewProjectile(SourceFromThis, NPC.Bottom, Vector2.Zero,
                        ModContent.ProjectileType<StarMissileBoom>(), StarMissileDamage, 1, Main.myPlayer);
                }

                for (int i = 0; i < 2; i++)
                {
                    Vector2 rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));
                    rvelocity *= 2;

                    Gore.NewGore(SourceFromThis, NPC.Bottom, rvelocity,
                        ModContent.GoreType<FableRock1>());

                    rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(SourceFromThis, NPC.Bottom, rvelocity,
                        ModContent.GoreType<FableRock2>());

                    rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(SourceFromThis, NPC.Bottom, rvelocity,
                        ModContent.GoreType<FableRock3>());

                    rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(SourceFromThis, NPC.Bottom, rvelocity,
                        ModContent.GoreType<FableRock4>());
                }
                for(float f = 0; f < 3; f++)
                {
                    float completionRatio = f / 4f;
                    var sear = Particle.NewParticle<SearParticle>(NPC.Bottom, Vector2.Zero);
                    sear.Scale *= MathHelper.Lerp(2f, 4f, completionRatio);
                }
         
            }

            if (Timer % 4 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>(), Scale: Main.rand.NextFloat(0.5f, 1f));
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<TSmokeDust>(), Scale: Main.rand.NextFloat(0.5f, 1f));
                Particle.NewParticle<ZapParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), Main.rand.NextVector2Circular(4, 4), newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            TargetOutlineColor = Color.Red;
            _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
            NPC.rotation += NPC.velocity.X * 0.025f;
            NPC.velocity.X = MathHelper.Lerp(NPC.direction * 32, 0, Timer / 100f);


            if (MathF.Abs(NPC.velocity.X) <= 1f && Timer >= 60)
            {
                SwitchState(AIState.Idle);
            }
           
        }
        #endregion


        #region Walk Up Stomp
        private void AI_WalkUpStompStart()
        {
            Timer++;
            TargetOutlineColor = Color.Transparent;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
                SoundStyle ohMove5 = AssetRegistry.Sounds.STARBOMBER.Ommove5;
                ohMove5.PitchVariance = 0.2f;
                SoundEngine.PlaySound(ohMove5, NPC.position);
            }

            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }
            float speed = 5f;
            float targetVelocityX = TargetDirection * speed;
            float xDiff = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
            if (xDiff > speed)
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocityX, 0.1f);
            }
            else
            {
                NPC.velocity.X *= 0.95f;
                AttackCycle = 1;
            }

            ApplyStandingYVelocity();
            if (AttackCycle >= 1 || Timer >= 120)
            {
                SwitchState(AIState.WalkUpStomp_Stomp);
            }
        }

        private void AI_WalkUpStompStomp()
        {
            Timer++;
            TargetOutlineColor = Color.Yellow;
            TargetGunOutlineColor = Color.Transparent;
            _legsState = LegsState.Walk;
            if (Timer == 1)
            {

                SoundStyle wake = new SoundStyle("Stellamod/Assets/Sounds/STARBOMBERWAKE");
                SoundEngine.PlaySound(wake, NPC.position);
                NPC.TargetClosest();
                NPC.direction = TargetDirection;

                //Move 1 leg
                Legs.MoveFoot(ref Legs.rightLegData, NPC.Center + new Vector2(200, 0));
            }

            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -64);
            _afterImageTime = MathHelper.Lerp(_afterImageTime, 0.5f, 0.1f);
            NPC.velocity.X *= 0.9f;
            ApplyStandingYVelocity();

            if(Timer % 5 == 0)
            {
                var p = Particle.NewParticle<ZapParticle>(Legs.rightLegData.footPosition, Main.rand.NextVector2Circular(1, 1));
                p.Scale *= Main.rand.NextFloat(0.5f, 1f);
            }
            if (Timer >= 90f)
            {
                SwitchState(AIState.WalkUpStomp_End);
            }
        }


        private void AI_WalkUpStompEnd()
        {
            Timer++;

            TargetOutlineColor = Color.Red;
            TargetGunOutlineColor = Color.Transparent;

            _legsState = LegsState.Freeze;
            _afterImageTime *= 0.9f;

            float time = 24f;
            float progress = Timer / time;

            float inEasing = EasingFunction.InExpo(progress);
            progress = EasingFunction.InOutSine(progress);
            if(Timer == 1)
            {
                Vector2 groundPoint = FindGround();
                groundPoint += new Vector2(200, 0);
                _impactFootPosition = groundPoint;


                Legs.rightLegData.startWalkPosition = Legs.rightLegData.footPosition;
            }
            if(Timer % 6 == 0)
            {
                var p = Particle.NewParticle<GlowDonutParticle>(Legs.rightLegData.footPosition, Vector2.UnitY);
                var p2 = Particle.NewParticle<GlowDonutParticle>(Legs.rightLegData.footPosition, Vector2.UnitY * 4);
                p2.Scale *= 0.5f;
            }
            SpinSpeed = MathHelper.Lerp(1f, 3f, progress);

            Legs.rightLegData.rotationStyle = RotationStyle.Inverse;
            Legs.rightLegData.footPosition = Vector2.Lerp(Legs.rightLegData.startWalkPosition, _impactFootPosition, inEasing);
            ShakeModSystem.Shake = 4;
            OffsetCameraModifier.FocusTargetOffset = new Vector2(0, 100);
            if (Timer == time)
            {
                ShakeModSystem.Shake = 16;
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 velocity = Vector2.UnitX * 22;
                    for(float f = 0; f < 4; f++)
                    {
                        float completionRatio = f / 4f;
                        Projectile.NewProjectile(SourceFromThis, _impactFootPosition, velocity * completionRatio,
                            ModContent.ProjectileType<STARSHOTT>(), StarMissileDamage, 1, Main.myPlayer);
                        Projectile.NewProjectile(SourceFromThis, _impactFootPosition, -velocity * completionRatio,
                            ModContent.ProjectileType<STARSHOTT>(), StarMissileDamage, 1, Main.myPlayer);
                    }

                    Projectile.NewProjectile(SourceFromThis, _impactFootPosition, Vector2.Zero,
                        ModContent.ProjectileType<StarMissileBoom>(), StarMissileDamage, 1, Main.myPlayer);
                }
                for (int i = 0; i < 1; i++)
                {
                    Vector2 rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));
                    rvelocity *= 2;

                    Gore.NewGore(SourceFromThis, _impactFootPosition, rvelocity,
                        ModContent.GoreType<FableRock1>());

                    rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(SourceFromThis, _impactFootPosition, rvelocity,
                        ModContent.GoreType<FableRock2>());

                    rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(SourceFromThis, _impactFootPosition, rvelocity,
                        ModContent.GoreType<FableRock3>());

                    rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(SourceFromThis, _impactFootPosition, rvelocity,
                        ModContent.GoreType<FableRock4>());
                }
                var sear = Particle.NewParticle<SearParticle>(_impactFootPosition, Vector2.Zero);

                for (int i = 0; i < 16; i++)
                {
                    float radius = 150;
                    Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                    offset *= Main.rand.NextFloat(1f, radius);
                    offset += new Vector2(radius / 2, 0);

                    Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                    velocity *= Main.rand.NextFloat(1f, 2f);
                    Dust.NewDustPerfect(NPC.Bottom + offset, ModContent.DustType<Dusts.TSmokeDust>(), velocity, 0, Color.Black * 0.5f,
                        Main.rand.NextFloat(0.3f, 0.7f));
                }
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(_impactFootPosition, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
                }

                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(_impactFootPosition, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
                }
                var circleFlare = Particle.NewParticle<GlowDonutParticle>(Legs.rightLegData.footPosition, Vector2.UnitY);
                circleFlare.noStretch = true;
                circleFlare.Scale *= 4;
                circleFlare.shrink = true;
                FXUtil.GlowCircleBoom(_impactFootPosition,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);
                FXUtil.ShakeCamera(_impactFootPosition, 1024, 8);
                FXUtil.PunchCamera(NPC.position, Vector2.UnitY, 8, 8, 8);
                SoundStyle crush = AssetRegistry.Sounds.STARBOMBER.HeavyCrush;
                crush.PitchVariance = 0.3f;
                SoundEngine.PlaySound(crush, NPC.position);


                var p = Particle.NewParticle<GlowDonutParticle>(Legs.rightLegData.footPosition, Vector2.UnitY);
                p.Scale *= 5;
            }

            StandHeight *= 0.5f;
            NPC.velocity.X *= 0.9f;
            ApplyStandingYVelocity();
            if (Timer >= time)
            {
                SwitchState(AIState.Idle);
            }
        }

        #endregion


        #region Steam Whistle
        private void AI_SteamWhistleStart()
        {
            /*
             * Steam whistle attack where he charges a big beam and
             * blasts it at you with high precision (sniper and one shot)
             */

            Timer++;
            //We using the machine gun brah
            HeldGun = WhistleGun;
            TargetOutlineColor = Color.Yellow;
            if (Timer == 1)
            {
                StretchLegs();
                NPC.TargetClosest();
                NPC.direction = TargetDirection;

                SoundStyle starGun = new SoundStyle("Stellamod/Assets/Sounds/STARLAUGH");
                starGun.PitchVariance = 0.1f;
                SoundEngine.PlaySound(starGun, NPC.Center);

            }


            float prepTime = 90f;

            //Summon the gun        
            //I think it's easier if all of star bombers guns just come from him and aren't projectiles                 
            //Yeah that'll be better, let's represent them
            float interpolant = Timer / prepTime;
            _gunHoldInterpolant = EasingFunction.InOutSine(interpolant);
            Vector2 gunHoistPosition = Vector2.Lerp(NPC.Center, GunHoistPosition, _gunHoldInterpolant);
            GunPosition = gunHoistPosition;
            GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.01f);
            GunVDirection = 1;

            NPC.velocity.X *= 0.9f;
            NPC.rotation *= 0.9f;
            SpinSpeed = 2;

            ApplyStandingYVelocity();
            if (Timer >= prepTime)
            {
                HeldGun.aimingReticleColor = Color.Red;
                HeldGun.aimingReticle = MathHelper.Lerp(0f, 1f, (Timer - prepTime) / prepTime);
                HeldGun.drawColor = Color.Lerp(HeldGun.drawColor, Color.White, 0.1f);
                GunDirection = Vector2.Lerp(GunDirection, AimGun(), 0.3f);
                GunVDirection = 1;
            }
            else
            {
                GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.01f);
                GunVDirection = 1;


            }

            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            if (Timer % 8 == 0)
            {
                Vector2 randOffset = Main.rand.NextVector2CircularEdge(64, 64);
                Vector2 spawnPos = GunPosition + randOffset;
                Vector2 velocity = (GunPosition - spawnPos).SafeNormalize(Vector2.Zero);
                Particle.NewParticle<EmberParticle>(spawnPos, velocity, Scale: 0.5f);
            }

            if (Timer % 16 == 0)
            {
                Vector2 randOffset = Main.rand.NextVector2CircularEdge(64, 64);
                Vector2 spawnPos = GunPosition + randOffset;
                Vector2 velocity = (GunPosition - spawnPos).SafeNormalize(Vector2.Zero);
                Particle.NewParticle<ZapParticle>(spawnPos, velocity, Scale: 0.5f);
                SoundStyle zapSound = SoundID.DD2_LightningBugZap;
                SoundEngine.PlaySound(zapSound, GunPosition);
            }
            if (Timer >= prepTime * 2f)
            {
                SwitchState(AIState.SteamWhistle_Loop);
            }
        }
        private void AI_SteamWhistleLoop()
        {
            Timer++;
            HeldGun = WhistleGun;
            GunPosition = GunHoistPosition;
            if (Timer == 1)
            {
                PrimeReticle();
                HeldGun.Prime();
            }
            HeldGun.aimingReticle = MathHelper.Lerp(1f, 0f, Timer / 60f);
            if(Timer < 60)
            {
                if (Timer % 5 == 0)
                {
                    SpawnSteamParticle();
                }
            }

            if (Timer == 60)
            {
                NPC.velocity.X = -NPC.direction * 5;
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, GunMuzzlePosition, GunDirection * 1200,
                        ModContent.ProjectileType<SteamLaser>(), SteamLaserDamage, 1, Main.myPlayer);
                }
                SoundStyle tireSound = new SoundStyle("Stellamod/Assets/Sounds/STARWAVE");
                tireSound.PitchVariance = 0.15f;
                SoundEngine.PlaySound(tireSound, NPC.position);
                HeldGun.Recoil();
            }

            if (Timer >= 60)
            {
                SpinSpeed = 1;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 4, 0.1f);
                GunDirection = Vector2.Lerp(GunDirection, AimGun(), 0.005f);
                GunVDirection = 1;

                HeldGun.drawColor = Color.Lerp(HeldGun.drawColor, _gunSilhouetteColor, 0.1f);
                if (Timer % 8 == 0)
                {
                    Dust.NewDust(GunPosition, 4, 4, ModContent.DustType<TSmokeDust>(), newColor: Color.DarkGray,
                        Scale: Main.rand.NextFloat(0.5f, 1f));
                }

                if (Timer % 16 == 0)
                {
                    Particle.NewParticle<EmberParticle>(GunPosition, Main.rand.NextVector2Circular(1, 4), newColor: Color.Red);
                    if (Main.rand.NextBool(2))
                    {
                        Particle.NewParticle<ZapParticle>(GunPosition + Main.rand.NextVector2Circular(8, 8), Main.rand.NextVector2Circular(1, 1), newColor: Color.Pink, Scale: Main.rand.NextFloat(0.5f, 0.66f));
                    }
                }


            }

            NPC.velocity.X *= 0.9f;

            ApplyStandingYVelocity();
            if (Timer >= 120)
            {
                SwitchState(AIState.SteamWhistle_End);
            }
        }
        private void AI_SteamWhistleEnd()
        {
            SpinSpeed = 0.25f;
            Timer++;
            TargetOutlineColor = Color.Transparent;
            if (Timer == 1)
            {
                SoundStyle tireSound = new SoundStyle("Stellamod/Assets/Sounds/STARWAVE");
                tireSound.PitchVariance = 0.15f;
                tireSound.Pitch = -0.5f;
                SoundEngine.PlaySound(tireSound, NPC.position);
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }
            float prepTime = 90f;

            //Summon the gun        
            //I think it's easier if all of star bombers guns just come from him and aren't projectiles                 
            //Yeah that'll be better, let's represent them
            float interpolant = Timer / prepTime;
            _gunHoldInterpolant = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(interpolant));
            Vector2 gunHoistPosition = Vector2.Lerp(NPC.Center, GunHoistPosition, _gunHoldInterpolant);
            GunPosition = gunHoistPosition;
            GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.1f);
            HeldGun.drawColor = Color.Lerp(Color.Transparent, Color.White, _gunHoldInterpolant);
            if (Timer == prepTime)
            {
                FXUtil.GlowCircleBoom(gunHoistPosition + Vector2.UnitY * 40, Color.White, Color.Pink, Color.Blue);
            }

            NPC.velocity.X *= 0.9f;
            ApplyStandingYVelocity();
            if (Timer >= 90)
            {
                SwitchState(AIState.Idle);
            }
        }
        #endregion


        #region Missile Launcher

        private void AI_MissileLauncherStart()
        {
            /*
             * Pulls out a missile launcher that shoots slightly homing missiles
             */
            Timer++;
            //We using the machine gun brah
            HeldGun = MissileLauncher;
            TargetGunOutlineColor = Color.Yellow;
            TargetOutlineColor = Color.Yellow;
            if (Timer == 1)
            {
                StretchLegs();
                NPC.TargetClosest();
                NPC.direction = TargetDirection;

                SoundStyle starGun = new SoundStyle("Stellamod/Assets/Sounds/STARGUN");
                starGun.PitchVariance = 0.1f;
                SoundEngine.PlaySound(starGun, NPC.Center);
            }

            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            float prepTime = 90f;

            //Summon the gun        
            //I think it's easier if all of star bombers guns just come from him and aren't projectiles                 
            //Yeah that'll be better, let's represent them
            float interpolant = Timer / prepTime;
            _gunHoldInterpolant = EasingFunction.InOutSine(interpolant);
            Vector2 gunHoistPosition = Vector2.Lerp(NPC.Center, GunHoistPosition, _gunHoldInterpolant);
            GunPosition = gunHoistPosition;
            GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.01f);
            GunVDirection = 1;

            HeldGun.drawColor = Color.Lerp(Color.Transparent, Color.White, _gunHoldInterpolant);
            NPC.velocity.X *= 0.9f;
            ApplyStandingYVelocity();
            if (Timer >= prepTime)
            {
                SwitchState(AIState.MissileLauncher_Loop);
            }
        }

        private void AI_MissileLauncherLoop()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }
            HeldGun = MissileLauncher;
            HeldGun.muzzleOffset = 140;
            GunPosition = GunHoistPosition;
            TargetOutlineColor = Color.Yellow;

            if (AttackCycle == 2)
            {
                if (Timer % 24 == 0 && Timer < 120)
                {
                    TargetGunOutlineColor = Color.Red;
                    if (MultiplayerHelper.IsHost)
                    {
                        Projectile.NewProjectile(SourceFromThis, GunMuzzlePosition, GunDirection * 7,
                            ModContent.ProjectileType<StarMissile>(), StarMissileDamage, 1, Main.myPlayer);
                    }
                    NPC.velocity.X = -NPC.direction * 0.5f;
                    HeldGun.Recoil();
                }

            }
            else
            {
                if (Timer % 48 == 0 && Timer < 120)
                {
                    TargetGunOutlineColor = Color.Red;
                    if (MultiplayerHelper.IsHost)
                    {
                        Projectile.NewProjectile(SourceFromThis, GunMuzzlePosition, GunDirection * 7,
                            ModContent.ProjectileType<StarMissile>(), StarMissileDamage, 1, Main.myPlayer);
                    }
                    NPC.velocity.X = -NPC.direction * 0.5f;
                    HeldGun.Recoil();
                }

            }


            if (Timer == 120)
            {
                StretchLegs();
                SoundStyle tireSound = new SoundStyle("Stellamod/Assets/Sounds/STARWAVE");
                tireSound.PitchVariance = 0.15f;
                SoundEngine.PlaySound(tireSound, NPC.position);
            }

            if (Timer >= 120)
            {
                _legsState = LegsState.Walk;
                TargetGunOutlineColor = Color.Yellow;
                if (AttackCycle != 2)
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 4, 0.1f);
                GunDirection = Vector2.Lerp(GunDirection, AimGun(), 0.005f);
                GunVDirection = 1;

                HeldGun.drawColor = Color.Lerp(HeldGun.drawColor, _gunSilhouetteColor, 0.1f);


                if (Timer % 5 == 0)
                {
                    SpawnSteamParticle();
                }

                if (Timer % 16 == 0)
                {
                    Dust.NewDust(GunPosition, 4, 4, ModContent.DustType<TSmokeDust>(), newColor: Color.DarkGray,
                        Scale: Main.rand.NextFloat(0.5f, 1f));
                }

                if (Timer % 16 == 0)
                {
                    Particle.NewParticle<EmberParticle>(GunPosition, Main.rand.NextVector2Circular(1, 4), newColor: Color.Red);
                    if (Main.rand.NextBool(2))
                    {
                        Particle.NewParticle<ZapParticle>(GunPosition + Main.rand.NextVector2Circular(8, 8), Main.rand.NextVector2Circular(1, 1), newColor: Color.Pink, Scale: Main.rand.NextFloat(0.5f, 0.66f));
                    }
                }
            }
            else
            {
                _legsState = LegsState.Limp;
                NPC.velocity.X *= 0.9f;
                GunDirection = Vector2.Lerp(GunDirection, AimGun(), 0.1f);
                GunVDirection = 1;
                HeldGun.drawColor = Color.Lerp(HeldGun.drawColor, Color.White, 0.1f);
            }

            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.025f, 0.1f);
            ApplyStandingYVelocity();
            if (Timer >= 240)
            {
                AttackCycle++;
                if (AttackCycle >= 3)
                {
                    SwitchState(AIState.MissileLauncher_End);
                }
                else
                {
                    SwitchState(AIState.MissileLauncher_Loop);
                }

            }
        }

        private void AI_MissileLauncherEnd()
        {
            Timer++;
            HeldGun = MissileLauncher;
            TargetOutlineColor = Color.Transparent;
            if (Timer == 1)
            {
                SoundStyle tireSound = new SoundStyle("Stellamod/Assets/Sounds/STARWAVE");
                tireSound.PitchVariance = 0.15f;
                tireSound.Pitch = -0.5f;
                SoundEngine.PlaySound(tireSound, NPC.position);
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }

            float prepTime = 90f;

            //Summon the gun        
            //I think it's easier if all of star bombers guns just come from him and aren't projectiles                 
            //Yeah that'll be better, let's represent them
            float interpolant = Timer / prepTime;
            _gunHoldInterpolant = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(interpolant));
            Vector2 gunHoistPosition = Vector2.Lerp(NPC.Center, GunHoistPosition, _gunHoldInterpolant);
            GunPosition = gunHoistPosition;
            GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.1f);
            if (Timer == prepTime)
            {
                FXUtil.GlowCircleBoom(gunHoistPosition + Vector2.UnitY * 40, Color.White, Color.Pink, Color.Blue);
            }
            HeldGun.drawColor = Color.Lerp(Color.Transparent, Color.White, _gunHoldInterpolant);

            NPC.velocity.X *= 0.9f;
            ApplyStandingYVelocity();
            if (Timer >= prepTime)
            {
                SwitchState(AIState.Idle);
            }
        }


        #endregion


        #region Wing Time Snipe

        private void AI_WingTimeSnipeStart()
        {
            Timer++;
            //We using the machine gun brah
            HeldGun = WingSniper;
            TargetOutlineColor = Color.Yellow;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;

                SoundStyle starGun = new SoundStyle("Stellamod/Assets/Sounds/STARGUN");
                starGun.PitchVariance = 0.1f;
                SoundEngine.PlaySound(starGun, NPC.Center);

            }


            float prepTime = 90f;

            //Summon the gun        
            //I think it's easier if all of star bombers guns just come from him and aren't projectiles                 
            //Yeah that'll be better, let's represent them
            float interpolant = Timer / prepTime;
            float eased = EasingFunction.InOutSine(interpolant);
            Vector2 gunHoistPosition = Vector2.Lerp(NPC.Center, GunHoistPosition, eased);
            GunPosition = gunHoistPosition;
            GunDirection = AimGun();

            if (Timer >= prepTime)
            {
                SwitchState(AIState.WingTimeSnipe_End);
            }
        }

        private void AI_WingTimeSnipeEnd()
        {
            Timer++;
            HeldGun = WingSniper;
            TargetOutlineColor = Color.Yellow;
            GunPosition = GunHoistPosition;
            if (Timer == 1 && MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, GunMuzzlePosition, GunDirection,
                    ModContent.ProjectileType<WingSnipe>(), WingSnipeDamage, 1, Main.myPlayer);
            }

            if (Timer >= 120)
            {
                SwitchState(AIState.Idle);
            }
        }
        #endregion

        public override void OnKill()
        {
            base.OnKill();
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedSTARBoss, -1);
        }


        #region Draw Code

        private void DrawHeldGun(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (HeldGun == null)
                return;
            Vector2 position = GunPosition;
            position.Y += ExtraMath.Osc(-8f, 8f, speed: 2);
            Vector2 direction = GunDirection;
            HeldGun.Draw(spriteBatch, position, direction, drawColor);
            DrawHeldLightning(spriteBatch, screenPos, drawColor);
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Gray, EasingFunction.QuadraticBump(completionRatio)) * _gunHoldInterpolant;
        }

        private float WidthFunction(float completionRatio)
        {
            return 64;
        }
        private void DrawHeldLightning(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            BlackFireShader shader = BlackFireShader.Instance;
            shader.PrimaryTexture = TrailRegistry.WhispyTrail;
            shader.PrimaryTexture2 = TrailRegistry.StarTrail;
            shader.InnerColor = Color.Lerp(Color.Black, Color.Red, ExtraMath.Osc(0f, 1f, speed: 2));
            shader.OuterColor = Color.Lerp(Color.Blue, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 2));
            shader.Distortion = 0.2f;
            shader.Time = Main.GlobalTimeWrappedHourly * 4;
            TrailDrawer.Draw(spriteBatch, LightningPos, ColorFunction, WidthFunction, shader);
        }
        private void DrawBody(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos += _shakeOffset;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;

                float f = i;
                float interpolant = f / (float)NPC.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant);
                fadeColor *= _afterImageTime;
                oldDrawPos += NPC.Size / 2f;
                spriteBatch.Draw(texture, oldDrawPos, NPC.frame, fadeColor, NPC.oldRot[i], drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }


            LegTextures ??= new Texture2D[4];
            LegTextures[0] = ThighTexture.Value;
            LegTextures[1] = KneeTexture.Value;
            LegTextures[2] = LegTexture.Value;
            LegTextures[3] = FootTexture.Value;

            Legs2.Draw(spriteBatch, LegTextures, drawColor.MultiplyRGB(Color.Lerp(Color.White, Color.Black, 0.7f)));

            spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale * 2 * _squishScale, SpriteEffects.None, 0);
            Legs.Draw(spriteBatch, LegTextures, drawColor);

            if(_deathLerp > 0)
            {
                drawColor = Color.Red;
                drawColor.A = 0;
                drawColor *= ExtraMath.Osc(0f, 1f, speed: 32);

                Color startGlowColor = Color.Yellow;
                startGlowColor.A = 0;

                Color glowColor = Color.Lerp(startGlowColor, drawColor, _deathLerp);
                glowColor *= _deathLerp;
                spriteBatch.Draw(texture, drawPos, NPC.frame, glowColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, NPC.frame, glowColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0);
            }
            if (_spinTelegraphLerp > 0)
            {

                Color flashColor = Color.Yellow;
                flashColor *= ExtraMath.Osc(0f, 1f, speed: 32);

                flashColor.A = 0;
                flashColor *= _spinTelegraphLerp;
                spriteBatch.Draw(texture, drawPos, NPC.frame, flashColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, NPC.frame, flashColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0);
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            DrawBody(spriteBatch, screenPos, drawColor);
    
            DrawHeldGun(spriteBatch, screenPos, drawColor);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 position = GunPosition;
            position.Y += ExtraMath.Osc(-8f, 8f, speed: 2);

            Vector2 direction = GunDirection;
            HeldGun?.DrawOutlines(spriteBatch, position, direction, _gunOutlineColor);
            float outlineOffset = 2;
            DrawBody(spriteBatch, screenPos - Vector2.UnitX * outlineOffset, _outlineColor);
            DrawBody(spriteBatch, screenPos + Vector2.UnitX * outlineOffset, _outlineColor);
            DrawBody(spriteBatch, screenPos - Vector2.UnitY * outlineOffset, _outlineColor);
            DrawBody(spriteBatch, screenPos + Vector2.UnitY * outlineOffset, _outlineColor);
            Legs.DrawOutlines(spriteBatch, LegTextures, _outlineColor);
            Legs2.DrawOutlines(spriteBatch, LegTextures, _outlineColor);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float starFieldColorProgress = ExtraMath.Osc(0f, 0.45f);
            Texture2D texture = StarfieldTexture.Value;
            Rectangle animationFrame = texture.AnimationFrame(ref _starFieldFrameCounter, ref _starFieldFrameTick, 1, 30, true);
            Color starFieldDrawColor = Color.White;
            starFieldDrawColor.A = 0;
            starFieldDrawColor *= 0.3f;
            starFieldDrawColor *= ExtraMath.Osc(0.5f, 1f);
            float starFieldScale = 1.5f;
            float starFieldRotation = 0;

            Vector2 starFieldDrawPos = NPC.Center - screenPos;
            spriteBatch.Draw(texture, starFieldDrawPos, animationFrame, starFieldDrawColor, starFieldRotation,
                animationFrame.Size() / 2, starFieldScale, SpriteEffects.None, 0);

            starFieldDrawColor = Color.White;
            starFieldDrawColor.A = 0;

            starFieldDrawColor *= ExtraMath.Osc(0.5f, 1f);
        }
        #endregion
    }
}
