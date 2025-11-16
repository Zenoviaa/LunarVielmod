
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles;
using Stellamod.Core;
using Stellamod.Core.InverseKinematics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER
{
    public enum RotationStyle : byte
    {
        Inverse=0,
        Forward=1,
        InverseLerp=2
    }


    public struct LegData
    {
        public Vector2 startWalkPosition;
        public Vector2 endWalkPosition;
        public Vector2 rootPosition;
        public Vector2 footPosition;
        public float timer;
        public float duration;
        public RotationStyle rotationStyle;
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

        public void MoveFoot(ref LegData legData, Vector2 targetFootPosition)
        {
            SoundStyle walkingStartSound = AssetRegistry.Sounds.STARBOMBER.STARWALK;
            walkingStartSound.PitchVariance = 0.3f;
            walkingStartSound.Volume = 0.25f;
            SoundEngine.PlaySound(walkingStartSound, targetFootPosition);

            legData.footPosition = targetFootPosition;
            legData.timer = 0f;
            legData.duration = 24;
            legData.startWalkPosition = legData.footPosition;
            legData.endWalkPosition = targetFootPosition;
            legData.rotationStyle = RotationStyle.Forward;
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
   //             legData.duration /= 2;
                legData.startWalkPosition = leg.GetEndEffector();
                legData.rotationStyle = RotationStyle.InverseLerp;
            }
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
                    if(legData.timer >= time)
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
                case RotationStyle.Forward:
                    CalculateWalkAngles(ref legData, leg);
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
    public class STARBOMBERGUN
    {
        private float _primeTimer;
        private float _recoilTimer;
        private Vector2 _lastPosition;
        public STARBOMBERGUN(string texturePath)
        {
            TextureAsset = ModContent.Request<Texture2D>(texturePath);
            drawScale = Vector2.One;
            recoilDistance = 24;
            drawColor = Color.White;
        }
        public Asset<Texture2D> TextureAsset;
        public Vector2 drawScale;
        public float recoilDistance;
        public Color drawColor;
        public float aimingReticle;
        public Color aimingReticleColor;
        public float muzzleOffset;
        public Vector2 GetMuzzlePosition(Vector2 anchorPosition, Vector2 direction)
        {
            Vector2 muzzlePosition = anchorPosition + direction * muzzleOffset;
            muzzlePosition -= direction * recoilDistance;
            return muzzlePosition;
        }

        public Vector2 GetRecoilOffset(Vector2 direction)
        {
            float progress = _recoilTimer / 8f;
            return -direction * progress * recoilDistance;
        }
        public void Recoil()
        {
            _recoilTimer = 8;
            FXUtil.ShakeCamera(_lastPosition, 256, 8);
        }

        public void Prime()
        {
            _primeTimer = 45f;
            SoundStyle primeSound = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol2");
            primeSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(primeSound, _lastPosition); 
            var part = Particle.NewParticle<GlowDonutParticle>(_lastPosition, Vector2.Zero, Color.White);
            part.Scale *= 4;
            part.shrink = true;
            part.noStretch = true;
        }
        public void Update()
        {
            if (_primeTimer > 0)
                _primeTimer--;
            if (_recoilTimer > 0)
            {
                _recoilTimer--;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 direction, Color lightColor)
        {
            _lastPosition = position;
            Vector2 drawPosition = position - Main.screenPosition;
            drawPosition += GetRecoilOffset(direction);

            float primeProgress = _primeTimer / 45f;
            drawPosition += Main.rand.NextVector2Circular(4, 4) * primeProgress;

            float recoilAmount = _recoilTimer / 8f;
            Color finalColor = drawColor.MultiplyRGB(lightColor);
            Vector2 drawOrigin = new Vector2(0, TextureAsset.Height() / 2f);
            Vector2 finalScale = drawScale;
            finalScale += Vector2.One * recoilAmount * 0.1f;
            float rotation = direction.ToRotation();
            float angle = MathHelper.WrapAngle(rotation);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (direction.X < 0)
            {
                spriteEffects = SpriteEffects.FlipVertically;
                drawOrigin.Y = TextureAsset.Height() - drawOrigin.Y;
            }
            spriteBatch.Draw(TextureAsset.Value, drawPosition, null, finalColor, rotation, drawOrigin, finalScale, spriteEffects, 0);


            Color glowColor = Color.White;
            glowColor.A = 0;
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(TextureAsset.Value, drawPosition, null, glowColor * recoilAmount, rotation, drawOrigin, finalScale, spriteEffects, 0);

            }
            glowColor = Color.Red;
            glowColor.A = 0;
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(TextureAsset.Value, drawPosition, null, glowColor * recoilAmount, rotation, drawOrigin, finalScale, spriteEffects, 0);
            }
            if(_primeTimer > 0)
            {
                glowColor = Color.Red;
                glowColor *= primeProgress;
                glowColor.A = 0;
                for (int i = 0; i < 3; i++)
                {
                    spriteBatch.Draw(TextureAsset.Value, drawPosition, null, glowColor, rotation, drawOrigin, finalScale, spriteEffects, 0);

                }
            }


            Color aimingLineColor = aimingReticleColor;
            aimingLineColor *= aimingReticle;
            aimingReticleColor.A = 0;
            Texture2D aimingLine = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            Vector2 lineOrigin = new Vector2(aimingLine.Size().X / 2f, 0f);
            Vector2 lineScale = new Vector2(0.01f, 1f);
            spriteBatch.Draw(aimingLine, drawPosition, null, aimingLineColor, rotation - MathHelper.PiOver2, lineOrigin, lineScale, SpriteEffects.None, 0);
        }
        

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 position, Vector2 direction, Color lightColor)
        {
            Draw(spriteBatch, position - Vector2.UnitY * 2, direction, lightColor);
            Draw(spriteBatch, position + Vector2.UnitY * 2, direction, lightColor);
            Draw(spriteBatch, position - Vector2.UnitX * 2, direction, lightColor);
            Draw(spriteBatch, position + Vector2.UnitX * 2, direction, lightColor);
        }
    }


    public class STARBOMBERV2 : ScarletBoss,
        IDrawOutlines
    {
        private float _realSpinSpeed;
        private float _afterImageTime;
        private float _oscTimer;
        private bool _legsUp;
        private bool _contactDamage;
        private bool _namePlate;
        private bool _freezeWalkCycle;

        private Color _outlineColor;
        private Color _gunOutlineColor;
        private Vector2 _startFootPosition;
        private Vector2 _impactFootPosition;

        private Vector2 _targetWalkPosition;
        private PatternManager<AIState> _patternManager;

        private STARBOMBERGUN _machineGun;
        private STARBOMBERGUN _missileLauncher;
        private STARBOMBERGUN _sniperRifle;
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
        private Vector2 LeftLegRootPosition
        {
            get
            {
                return NPC.Center - Vector2.UnitX * LegRadius;
            }
        }
        private Vector2 RightLegRootPosition
        {
            get
            {
                return NPC.Center + Vector2.UnitX * LegRadius;
            }
        }
        private float SpinSpeed;

        private int WalkUpStompDamage => 100;
        private int SteamLaserDamage => 200;
        private int CrashDamage => 70;
        private int StarMissileDamage => 50;
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


        public STARBOMBERGUN SniperRifle
        {
            get
            {
                _sniperRifle ??= new STARBOMBERGUN(Texture + "_SniperRifle");
                return _sniperRifle;
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

        public Vector2 GunPosition;
        public Vector2 GunDirection;
        public float GunVDirection;
        public float StandHeight;
        public Vector2 LeftFootPosition;
        public Vector2 RightFootPosition;
        public float StandRange;

        public Texture2D[] LegTextures;
        public Asset<Texture2D> ThighTexture => ModContent.Request<Texture2D>(Texture + "_Thigh");
        public Asset<Texture2D> KneeTexture => ModContent.Request<Texture2D>(Texture + "_Knee");
        public Asset<Texture2D> LegTexture => ModContent.Request<Texture2D>(Texture + "_Leg");
        public Asset<Texture2D> FootTexture => ModContent.Request<Texture2D>(Texture + "_Foot");
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
            NPC.width = 128;
            NPC.height = 128;
            NPC.damage = 100;
            NPC.defense = 14;
            NPC.lifeMax = 6000;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Boss6");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/VoidHit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/VoidDead1") with { PitchVariance = 0.1f };
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += _realSpinSpeed;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }
            if(_frame >= Main.npcFrameCount[Type])
            {
                _frame = 0;
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
            writer.Write(_legsUp);
            writer.Write(_contactDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _targetWalkPosition = reader.ReadVector2();
            _legsUp = reader.ReadBoolean();
            _contactDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            base.AI();
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

            _realSpinSpeed = MathHelper.Lerp(_realSpinSpeed, SpinSpeed, 0.1f);
            _afterImageTime *= 0.9f;
            HeldGun?.Update();
            _oscTimer++;
            float osc = _oscTimer * 0.05f;
            float i = (MathF.Sin(osc) + 0.5f) / 0.5f;
            StandHeight = MathHelper.Lerp(300, 333,  i);

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
            SwitchState(AIState.MachineGun_Start);
        }


        #region Walking Code

        private void StretchLegs()
        {
            int oldDirection = NPC.direction;
            float oldStandRange = StandRange;
            StandRange *= 0.5f;
            NPC.direction = 0;
            Legs.MoveFoot(ref Legs.leftLegData, FindNewLeftFoot());
            Legs.MoveFoot(ref Legs.rightLegData, FindNewRightFoot());
            Legs2.MoveFoot(ref Legs2.leftLegData, FindNewLeftFoot2());
            Legs2.MoveFoot(ref Legs2.rightLegData, FindNewRightFoot2());
            NPC.direction = oldDirection;
            StandRange = oldStandRange;
        }
        private void UpdateLegs()
        {
            Legs.leftLegData.rootPosition = LeftLegRootPosition;
            Legs.rightLegData.rootPosition = RightLegRootPosition;
            Legs.Update();

            Legs2.leftLegData.rootPosition = LeftLegRootPosition;
            Legs2.rightLegData.rootPosition = RightLegRootPosition;
            Legs2.Update();
            UpdateWalkCycle();

        }


        private void UpdateWalkCycle()
        {
            if (_freezeWalkCycle)
                return;

            if (!Legs.IsValidFootPosition(ref Legs.leftLegData, LeftLegRootPosition.X, StandRange) && Legs.leftLegData.rotationStyle == RotationStyle.Inverse)
            {
                Legs.MoveFoot(ref Legs.leftLegData, FindNewLeftFoot());
            }
            if (!Legs.IsValidFootPosition(ref Legs.rightLegData, RightLegRootPosition.X, StandRange) && Legs.rightLegData.rotationStyle == RotationStyle.Inverse)
            {
                Legs.MoveFoot(ref Legs.rightLegData, FindNewRightFoot());
            }
            if (!Legs2.IsValidFootPosition(ref Legs2.leftLegData, LeftLegRootPosition.X, StandRange) && Legs2.leftLegData.rotationStyle == RotationStyle.Inverse)
            {
                Legs2.MoveFoot(ref Legs2.leftLegData, FindNewLeftFoot2());
            }
            if (!Legs2.IsValidFootPosition(ref Legs2.rightLegData, RightLegRootPosition.X, StandRange) && Legs2.rightLegData.rotationStyle == RotationStyle.Inverse)
            {
                Legs2.MoveFoot(ref Legs2.rightLegData, FindNewRightFoot2());
            }

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
            if(NPC.direction == -1)
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
            Vector2 groundPoint = CollisionHelper.RayCast(NPC.Center, Vector2.UnitY, 2000, 3);
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
                Projectile.NewProjectile(SourceFromThis, MyTarget.Center, Vector2.Zero,
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
            if (Timer >= 60)
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
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();

            }
            NPC.direction = TargetDirection;

            if (!_namePlate)
            {
                ShowNamePlate();
                _namePlate = true;
            }
            HeldGun = null;
            _freezeWalkCycle = false;

            //Set some defaults
            GunVDirection = -1;
            _contactDamage = false;
            AttackCycle = 0;
            SpinSpeed = 0.5f;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            float targetRotation = NPC.velocity.X * 0.05f;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.03f);
            NPC.rotation = MathHelper.WrapAngle(NPC.rotation);
            StandRange = MathHelper.Lerp(StandRange, 300, 0.1f);
            TargetGunOutlineColor = Color.Transparent;

            //Fun hover code
            //Also make sure we get bro to the ground
            float timeToWait = 300;
            ApplyStandingYVelocity();

            //Walk?
            float xDist = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
            if(xDist > StandRange)
            {
                float speed = MathHelper.Lerp(1, 2, MathHelper.Clamp(xDist / 16f, 0f, 1f));
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
            if(Timer >= 180)
            {
                NPC.Kill();
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


            float prepTime = 90f;

            //Summon the gun        
            //I think it's easier if all of star bombers guns just come from him and aren't projectiles                 
            //Yeah that'll be better, let's represent them
            float interpolant = Timer / prepTime;
            float eased = EasingFunction.InOutSine(interpolant);
            Vector2 gunHoistPosition = Vector2.Lerp(NPC.Center, GunHoistPosition, eased);
            GunPosition = gunHoistPosition;
            GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.01f);
            GunVDirection = 1;

            HeldGun.drawColor = Color.Lerp(Color.Transparent, Color.White, eased);
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
            if(Timer == 1)
            {
            
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }
            HeldGun = MachineGun;
            HeldGun.muzzleOffset = 140;
            GunPosition = GunHoistPosition;
            TargetOutlineColor = Color.Yellow;
   
 
            Vector2 directionToTarget = (MyTarget.Center - GunPosition).SafeNormalize(Vector2.Zero);
            float dp = Vector2.Dot(GunDirection, directionToTarget);
            if (Timer % 6 == 0 && Timer < 120 && dp > 0.8f)
            {
                TargetGunOutlineColor = Color.Red;
                NPC.velocity.X = -NPC.direction * 0.5f;
                if (MultiplayerHelper.IsHost)
                {
                    int type = ModContent.ProjectileType<MachineGunBullet>();
                    Projectile.NewProjectile(SourceFromThis, GunMuzzlePosition, GunDirection * 7,
                     type, MachineGunDamage, 1, Main.myPlayer);
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
                _freezeWalkCycle = false;
                TargetGunOutlineColor = Color.Yellow;
                SpinSpeed = 1;
         
                if(Timer >= 150)
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
            else
            {
                _freezeWalkCycle = true;
                SpinSpeed = 0.25f;
                NPC.velocity.X *= 0.99f;
                GunDirection = Vector2.Lerp(GunDirection, AimGun(), 0.1f);
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
            float eased = EasingFunction.InOutSine(interpolant);
            Vector2 gunHoistPosition = Vector2.Lerp(GunHoistPosition, NPC.Center, eased);
            GunPosition = gunHoistPosition;
            GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.1f);
            if (Timer == prepTime)
            {
                FXUtil.GlowCircleBoom(gunHoistPosition + Vector2.UnitY * 40, Color.White, Color.Pink, Color.Blue);
            }
            HeldGun.drawColor = Color.Lerp(Color.White, Color.Transparent, eased);

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
                SoundStyle tireSound = new SoundStyle("Stellamod/Assets/Sounds/STARBOMBERWAKE");
                tireSound.PitchVariance = 0.15f;
                SoundEngine.PlaySound(tireSound, NPC.position);
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }

            _legsUp = true;
            NPC.velocity.X *= 0.9f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            float prepTime = 90f;
            if (Timer >= prepTime && NPC.collideY)
            {
                SwitchState(AIState.LegUpSpin_Loop);
            }
        }

        private void AI_LegUpSpinLoop()
        {
      
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }
            _legsUp = true;


            float spinTime = 45;
            float spinSpeed = 40;
            float targetSpinVelocity = NPC.direction * spinSpeed;

            NPC.rotation = NPC.velocity.X * 0.015f;
            if (Timer >= spinTime / 2f)
            {
                NPC.velocity.X *= 0.94f;
                _afterImageTime *= 0.9f;
            }
            else
            {
                _afterImageTime = MathHelper.Lerp(_afterImageTime, 1f, 0.1f);
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetSpinVelocity, 0.1f);
            }

            if (Timer >= spinTime)
            {
                Timer = 0;
                AttackCycle++;
                if (AttackCycle >= 9)
                {
                    SwitchState(AIState.LegUpSpin_End);
                }
            }
        }

        private void AI_LegUpSpinEnd()
        {
            _contactDamage = false;
            TargetOutlineColor = Color.Transparent;
            Timer++;
            _legsUp = false;
            NPC.velocity.X *= 0.94f;
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
            _legsUp = true;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
                NPC.velocity.Y = -15;
            }


            NPC.noTileCollide = false;
            NPC.noGravity = false;
            SwitchState(AIState.CrashJump_Loop);
        }

        private void AI_CrashJumpLoop()
        {
            TargetOutlineColor = Color.Yellow;
            Timer++;
            _legsUp = true;
            _afterImageTime = MathHelper.Lerp(_afterImageTime, 0.3f, 0.1f);
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;

            }
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            float targetVelocity = NPC.direction * 5;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity, 0.01f);
            if (NPC.collideY && Timer >= 10)
            {
                SwitchState(AIState.CrashJump_Crash);
            }
        }

        private void AI_CrashJumpCrash()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.X = NPC.direction * 25;
                var part = FXUtil.GlowCircleBoom(NPC.Center, Color.Pink, Color.Purple, Color.Blue);
                part.Scale *= 8f; 
                
                part = FXUtil.GlowCircleBoom(NPC.Center, Color.Pink, Color.Purple, Color.Blue);
                part.Scale *= 3f;
                ShakeModSystem.Shake = 2;
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

                FXUtil.ShakeCamera(NPC.position, 2000, 64);
                FXUtil.PunchCamera(NPC.position, Vector2.UnitY, 8, 8, 8);
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Bottom, Vector2.Zero,
                        ModContent.ProjectileType<StompCrashBoom>(), CrashDamage, 1, Main.myPlayer);
                }
            }

            if(Timer % 8 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<TSmokeDust>(), Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            _afterImageTime = MathHelper.Lerp(_afterImageTime, 0.8f, 0.1f);
            TargetOutlineColor = Color.Red;
            _contactDamage = true;
            NPC.rotation += NPC.velocity.X * 0.025f;
            NPC.velocity.X *= 0.98f;

            if(MathF.Abs(NPC.velocity.X) <= 1f)
            {
                _legsUp = false;
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
            }

            float speed = 5f;
            float targetVelocityX = TargetDirection * speed;
            float xDiff = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
            if(xDiff > speed)
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocityX, 0.1f);
            }
            else
            {
                NPC.velocity.X *= 0.95f;
                AttackCycle = 1;
            }
                
            ApplyStandingYVelocity();
            if(AttackCycle >= 1)
            {
                SwitchState(AIState.WalkUpStomp_Stomp);
            }
        }

        private void AI_WalkUpStompStomp()
        {
            Timer++;
            TargetOutlineColor = Color.Yellow;

            if(Timer == 1)
            {

                SoundStyle wake = new SoundStyle("Stellamod/Assets/Sounds/STARBOMBERWAKE");
                SoundEngine.PlaySound(wake, NPC.position);
                NPC.TargetClosest();
                NPC.direction = TargetDirection;


           
     
            }
            _afterImageTime = MathHelper.Lerp(_afterImageTime, 0.5f, 0.1f);
            Vector2 groundPoint = FindGround();
            Vector2 offset = -Vector2.UnitY * 80;
            offset += Vector2.UnitX * NPC.direction * 164;
            if (NPC.direction == 1)
            {
              
                RightFootPosition = Vector2.Lerp(RightFootPosition, MyTarget.Top + offset, 0.1f);
                LeftFootPosition = Vector2.Lerp(LeftFootPosition, groundPoint, 0.1f);
            }
            else if (NPC.direction == -1)
            {
                RightFootPosition = Vector2.Lerp(RightFootPosition, groundPoint, 0.1f);
                LeftFootPosition = Vector2.Lerp(LeftFootPosition, MyTarget.Top + offset, 0.1f);
            }
            NPC.velocity.X *= 0.9f;
            ApplyStandingYVelocity();
        
            if(Timer >= 90f)
            {
                SwitchState(AIState.WalkUpStomp_End);
            }
        }


        private void AI_WalkUpStompEnd()
        {
            Timer++;

            TargetOutlineColor = Color.Red;

            _afterImageTime *= 0.9f;
            float time = 14f;
            float progress = Timer / time;
            progress = EasingFunction.InOutSine(progress);
            if (NPC.direction == 1)
            {
                if(Timer == 1)
                {
                    _startFootPosition = RightFootPosition;
                    _impactFootPosition = MyTarget.Center;
                }
                RightFootPosition = Vector2.Lerp(_startFootPosition, _impactFootPosition, progress);
            }
            else if (NPC.direction == -1)
            {
                if (Timer == 1)
                {
                    _startFootPosition = LeftFootPosition;
                    _impactFootPosition = MyTarget.Center;
                }
                LeftFootPosition = Vector2.Lerp(_startFootPosition,  _impactFootPosition, progress);
            }

            if(Timer == time)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, _impactFootPosition, Vector2.Zero,
                        ModContent.ProjectileType<StarMissileBoom>(), StarMissileDamage, 1, Main.myPlayer);
                }
                FXUtil.ShakeCamera(_impactFootPosition, 1024, 8);
            }
          

            NPC.velocity.X *= 0.9f;
            ApplyStandingYVelocity();
            if(Timer >= time)
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
            float eased = EasingFunction.InOutSine(interpolant);
            Vector2 gunHoistPosition = Vector2.Lerp(NPC.Center, GunHoistPosition, eased);
            GunPosition = gunHoistPosition;
       
            NPC.velocity.X *= 0.99f;
            ApplyStandingYVelocity();
            if(Timer >= prepTime)
            {
                HeldGun.aimingReticleColor = Color.Red;
                HeldGun.aimingReticle = MathHelper.Lerp(0f, 1f, (Timer - prepTime) / prepTime);
                HeldGun.drawColor = Color.Lerp(HeldGun.drawColor, Color.White, 0.1f);
                GunDirection = Vector2.Lerp(GunDirection, AimGun(), 0.1f);
                GunVDirection = 1;
            }
            else
            {
                GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.01f);
                GunVDirection = 1;


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
            if(Timer == 1)
            {
                PrimeReticle();
                HeldGun.Prime();
            }
            HeldGun.aimingReticle = MathHelper.Lerp(1f, 0f, Timer / 60f);
            if (Timer == 60)
            {
                NPC.velocity.X = -NPC.direction * 5;
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, GunMuzzlePosition, GunDirection * Vector2.Distance(GunMuzzlePosition, MyTarget.Center),
                        ModContent.ProjectileType<SteamLaser>(), SteamLaserDamage, 1, Main.myPlayer);
                }
                SoundStyle tireSound = new SoundStyle("Stellamod/Assets/Sounds/STARWAVE");
                tireSound.PitchVariance = 0.15f;
                SoundEngine.PlaySound(tireSound, NPC.position);
                HeldGun.Recoil();
            }
 
            if (Timer >= 60)
            {
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

            ApplyStandingYVelocity();
            if (Timer >= 120)
            {
                SwitchState(AIState.SteamWhistle_End);
            }
        }
        private void AI_SteamWhistleEnd()
        {
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
            float eased = EasingFunction.InOutSine(interpolant);
            Vector2 gunHoistPosition = Vector2.Lerp(GunHoistPosition, NPC.Center, eased);
            GunPosition = gunHoistPosition;
            GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.1f);
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
            GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.01f);
            GunVDirection = 1;

            NPC.velocity.X *= 0.99f;
            ApplyStandingYVelocity();
            if (Timer >= prepTime)
            {
                SwitchState(AIState.MissileLauncher_Loop);
            }
        }

        private void AI_MissileLauncherLoop()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }
            HeldGun = MissileLauncher;
            GunPosition = GunHoistPosition;
            TargetOutlineColor = Color.Yellow;

            if(AttackCycle == 2)
            {
                if (Timer % 24 == 0 && Timer < 120)
                {
                    if (MultiplayerHelper.IsHost)
                    {
                        Projectile.NewProjectile(SourceFromThis, GunMuzzlePosition, GunDirection * 7,
                            ModContent.ProjectileType<StarMissile>(), StarMissileDamage, 1, Main.myPlayer);
                    }
                    NPC.velocity.X = -NPC.direction * 1;
                    HeldGun.Recoil();
                }

            }
            else
            {
                if (Timer % 48 == 0 && Timer < 120)
                {
                    if (MultiplayerHelper.IsHost)
                    {
                        Projectile.NewProjectile(SourceFromThis, GunMuzzlePosition, GunDirection * 7,
                            ModContent.ProjectileType<StarMissile>(), StarMissileDamage, 1, Main.myPlayer);
                    }
                    NPC.velocity.X = -NPC.direction * 1;
                    HeldGun.Recoil();
                }

            }


            if (Timer == 120)
            {
                SoundStyle tireSound = new SoundStyle("Stellamod/Assets/Sounds/STARWAVE");
                tireSound.PitchVariance = 0.15f;
                SoundEngine.PlaySound(tireSound, NPC.position);
            }

            if (Timer >= 120)
            {
                if(AttackCycle != 2)
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
            else
            {
                NPC.velocity.X *= 0.99f;
                GunDirection = Vector2.Lerp(GunDirection, AimGun(), 0.1f);
                GunVDirection = 1;

                HeldGun.drawColor = Color.Lerp(HeldGun.drawColor, Color.White, 0.1f);
            }
            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
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
            float eased = EasingFunction.InOutSine(interpolant);
            Vector2 gunHoistPosition = Vector2.Lerp(GunHoistPosition, NPC.Center, eased);
            GunPosition = gunHoistPosition;
            GunDirection = Vector2.Lerp(GunDirection, Vector2.UnitY, 0.1f);
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

        private void DrawHeldGun(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (HeldGun == null)
                return;
            Vector2 position = GunPosition;
            Vector2 direction = GunDirection;
            HeldGun.Draw(spriteBatch, position, direction, drawColor);
        }

        private void DrawBody(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;

                float f = i;
                float interpolant = f / (float)NPC.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant) * 0.25f;
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
   
            spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0);
            Legs.Draw(spriteBatch, LegTextures, drawColor);

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            DrawBody(spriteBatch, screenPos, drawColor);
            DrawHeldGun(spriteBatch, screenPos, drawColor);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            if (HeldGun == null)
                return;
            Vector2 position = GunPosition;
            Vector2 direction = GunDirection;
            HeldGun.DrawOutlines(spriteBatch, position, direction, _gunOutlineColor);
            float outlineOffset = 2;
            DrawBody(spriteBatch, screenPos - Vector2.UnitX * outlineOffset, _outlineColor);
            DrawBody(spriteBatch, screenPos + Vector2.UnitX * outlineOffset, _outlineColor);
            DrawBody(spriteBatch, screenPos - Vector2.UnitY * outlineOffset, _outlineColor);
            DrawBody(spriteBatch, screenPos + Vector2.UnitY * outlineOffset, _outlineColor);
            Legs.DrawOutlines(spriteBatch, LegTextures, _outlineColor);

            Legs2.DrawOutlines(spriteBatch, LegTextures, _outlineColor);
        }
    }
}
