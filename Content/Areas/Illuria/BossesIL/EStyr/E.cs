using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{


    public partial class E : ScarletBoss
    {
        private enum AIState
        {
            Intro_PreFight,
            Intro_Idle,
            Intro_SwordHold,
            Intro_HeadTurn,
            Intro_HandOut,
            Intro_DomainExpansion,
            Intro_Finish,
            Idle,

            Despawn,

            ForwardSlash_Start,
            ForwardSlash_QuickStart,
            ForwardSlash_RePosition,
            ForwardSlash,
            ForwardSlash_End,

            RippingGeyser_Start,
            RippingGeyser_Dash,
            RippingGeyser_AuraFarm,
            RippingGeyser_End,

            Grab_Start,
            Grab_Walk,
            Grab_Dash,
            Grab_Punish,
            Grab_EatDirt,
            Grab_ThrowSword,
            Grab_End,

            Tornado_Start,
            Tornado_PreSpin,
            Tornado_Spin,
            Tornado_End,

            ScreenSlash_Start,
            ScreenSlash_PreSlash,
            ScreenSlash_Slash,
            ScreenSlash_SwordPoint,
            ScreenSlash_End,

            SwordStarPlosion_Start,
            SwordStarPlosion_Charge,
            SwordStarPlosion_Swing,
            SwordStarPlosion_End,

            BlackDashStart,
            BlackDashPreDash,
            BlackDashDash,
            BlackDashEnd,

            JevilScythes_Start,
            JevilScythes_Prepare,
            JevilScythes_Loop,
            JevilScythes_Quick,
            JevilScythes_End,

            SingularBaseball_Start,
            SingularBaseball_SummonBall,
            SingularBaseball_HitBall,
            SingularBaseball_FindBall,
            SingularBaseball_End,

            Kick_Start,
            Kick_Run,
            Kick_Kick,
            Kick_Fail,
            Kick_Fly,
            Kick_SwordThrowDown,
            Kick_End,

            Special_Warn,
            Special_Warn2,
            Special_HandStab,
            Special_DripDrop,
            Special_FadeToBlack,
            Special_MakeBox,
            Special_FadeOutFromBlack,
            Special_SlashQuickStart,
            Special_Slash,
            Special_SlashReposition,
            Special_SlashEndInBlack,
            Special_SlashEndOutBlack,

            Death_Start,
            Death_FlyOff
        }

        private bool _drawDarkened;
        private bool _startedFight;
        private bool _intro;
        private bool _showNamePlate;
        private bool _contactDamage;
        private bool _inRiver;
        private bool _doneSpecial;
        private bool _doneSpecial2;
        private float _bounceAttackNumber;
        private float _attackNumber;
        private float _hoverTimer;

        private bool InPhase2 => NPC.life < NPC.lifeMax * 0.5f;
        private bool InPhase3 => NPC.life < NPC.lifeMax * 0.2f;
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get
            {
                return (AIState)NPC.ai[1];
            }
            set
            {
                NPC.ai[1] = (float)value;
            }
        }
        private Vector2 TargetVector
        {
            get
            {
                return new Vector2(NPC.ai[2], NPC.ai[3]);
            }
            set
            {
                NPC.ai[2] = value.X;
                NPC.ai[3] = value.Y;
            }
        }

        private PatternManager<AIState> _patternManagerBackingField;
        private PatternManager<AIState> PatternManager
        {
            get
            {
                if (_patternManagerBackingField == null)
                {
                    _patternManagerBackingField = new PatternManager<AIState>(
                        new Tuple<AIState, float>(AIState.ForwardSlash_Start, 1.0f),
                         new Tuple<AIState, float>(AIState.RippingGeyser_Start, 1.0f),
                          new Tuple<AIState, float>(AIState.Grab_Start, 1.0f),
                           new Tuple<AIState, float>(AIState.Tornado_Start, 1.0f),
                            new Tuple<AIState, float>(AIState.ScreenSlash_Start, 1.0f),
                             new Tuple<AIState, float>(AIState.SwordStarPlosion_Start, 1.0f),
                             new Tuple<AIState, float>(AIState.JevilScythes_Start, 1.0f),
                             new Tuple<AIState, float>(AIState.SingularBaseball_Start, 1.0f));

                    //Always start with the forward slash attack
                    _patternManagerBackingField.QueueSetPattern(AIState.ForwardSlash_Start);
                }
                return _patternManagerBackingField;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
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


        //Finally time to make a secret boss, this is going to be fun :)
        //Alright
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            //We can still keep fair hitbox sizes though
            NPC.width = 100;
            NPC.height = 100;

            //We have to upscale this boss cause he's really hard and you're not really supposed to be him lol
            NPC.damage = 1;
            NPC.defense = 42;
            NPC.lifeMax = 120000;

            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/EStyr");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/BasicMagicHit1") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_forwardVector);
            writer.WriteVector2(_boxCenter);
            writer.Write(_hoverTimer);
            writer.Write(_attackNumber);
            writer.Write(_bounceAttackNumber);
            writer.Write(_doneSpecial);
            writer.Write(_doneSpecial2);
            writer.Write(_startedFight);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _forwardVector = reader.ReadVector2();
            _boxCenter = reader.ReadVector2();
            _hoverTimer = reader.ReadSingle();
            _attackNumber = reader.ReadSingle();
            _bounceAttackNumber = reader.ReadSingle();
            _doneSpecial = reader.ReadBoolean();
            _doneSpecial2 = reader.ReadBoolean();
            _startedFight = reader.ReadBoolean();
        }

        private void EnablePlatformArena()
        {
            DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
            fallSystem.noWings = true;
            fallSystem.inSpace = true;
            fallSystem.hoveringPlatform = true;
            fallSystem.hoverPlatformY = 16000;
        }

        private void CreateNewAfterImage()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Vector2 afterImageVelocity = -NPC.velocity.SafeNormalize(Vector2.Zero) * 4;
            afterImageVelocity = afterImageVelocity.RotatedByRandom(MathHelper.TwoPi);
            string texture = Texture + Animator.GetAnimation();

            Vector2 drawOrigin = GetDrawOrigin();
            float rotation = NPC.rotation;
            Rectangle frame = NPC.frame;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (NPC.spriteDirection == -1)
                drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;

            AfterImageRenderer.New(texture, frame, NPC.Center, afterImageVelocity, NPC.rotation, _drawScale, drawOrigin, Color.White * 0.6f, spriteEffects);
        }
        private void UpdateClient()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            //Create after images
            if(Timer % 4 == 0)
            {
                CreateNewAfterImage();
            }

            if (_intro)
            {
                NPC.boss = true;
                BlackSea blackSea = ScreenShader.GetInstance<BlackSea>();
                blackSea.alpha = 1f;

                BlackSeaRenderer blackseaRenderer = ModContent.GetInstance<BlackSeaRenderer>();
                blackseaRenderer.drawBlackSea = true;
                blackseaRenderer.darkenedSingularity = _drawDarkened;
            }
            else
            {
                NPC.boss = false;
                for (int i = 0; i < Main.musicFade.Length; i++)
                {
                    Main.musicFade[i] = 0;
                }
            }
        }

        public override void AI()
        {
            base.AI();

            if (_startedFight)
            {
                BlackSeaRenderer blackseaRenderer = ModContent.GetInstance<BlackSeaRenderer>();
                blackseaRenderer.renderBlackSea = true;
                EnablePlatformArena();
            }
        
            UpdateClient();

            _contactDamage = false;
            _isGrabbing = false;
            _hoverTimer++;
            SetRiverBoxParams();
            if (State != AIState.Despawn && !NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget)
                {
                    SwitchState(AIState.Despawn);
                }
            }
            if (_inRiver)
            {
                RetargetCameraModifier.ReTargetPosition = _boxCenter;
            }
            if(InPhase2 && !_doneSpecial)
            {
                PatternManager.QueueSetPattern(AIState.Special_Warn);
                _doneSpecial = true;
            }
            if (InPhase3 && !_doneSpecial2)
            {
                PatternManager.QueueSetPattern(AIState.Special_Warn);
                _doneSpecial2 = true;
            }

            _inRiver = false;
       
            _telegraphLineAlpha = 0;
            _drawScale = Vector2.One;
            TargetOutlineColor = Color.White;
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Intro_PreFight:
                    AI_IntroPreFight();
                    break;
                case AIState.Intro_Idle:
                    AI_IntroIdle();
                    break;
                case AIState.Intro_SwordHold:
                    AI_IntroSwordHold();
                    break;
                case AIState.Intro_HeadTurn:
                    AI_IntroHeadTurn();
                    break;
                case AIState.Intro_HandOut:
                    AI_IntroHandOut();
                    break;
                case AIState.Intro_DomainExpansion:
                    AI_IntroDomainExpansion();
                    break;
                case AIState.Intro_Finish:
                    AI_IntroFinish();
                    break;

                case AIState.Despawn:
                    AI_Despawn();
                    break;

                case AIState.ForwardSlash_Start:
                    AI_ForwardSlashStart();
                    break;
                case AIState.ForwardSlash_QuickStart:
                    AI_ForwardSlashQuickStart();
                    break;
                case AIState.ForwardSlash:
                    AI_ForwardSlash();
                    break;
                case AIState.ForwardSlash_RePosition:
                    AI_ForwardSlashReposition();
                    break;
                case AIState.ForwardSlash_End:
                    AI_ForwardSlashEnd();
                    break;

                case AIState.RippingGeyser_Start:
                    AI_RippingGeysterStart();
                    break;
                case AIState.RippingGeyser_Dash:
                    AI_RippingGeyserDash();
                    break;
                case AIState.RippingGeyser_AuraFarm:
                    AI_RippingGeyserAuraFarm();
                    break;
                case AIState.RippingGeyser_End:
                    AI_RippingGeyserEnd();
                    break;

                case AIState.Grab_Start:
                    AI_GrabStart();
                    break;
                case AIState.Grab_Walk:
                    AI_GrabWalk();
                    break;
                case AIState.Grab_Dash:
                    AI_GrabDash();
                    break;
                case AIState.Grab_Punish:
                    AI_GrabDunk();
                    break;
                case AIState.Grab_EatDirt:
                    AI_GrabEatDirt();
                    break;
                case AIState.Grab_ThrowSword:
                    AI_GrabThrowSword();
                    break;
                case AIState.Grab_End:
                    AI_GrabEnd();
                    break;

                case AIState.Tornado_Start:
                    AI_TornadoStart();
                    break;
                case AIState.Tornado_PreSpin:
                    AI_TornadoPreSpin();
                    break;
                case AIState.Tornado_Spin:
                    AI_TornadoSpin();
                    break;
                case AIState.Tornado_End:
                    AI_TornadoEnd();
                    break;

                case AIState.ScreenSlash_Start:
                    AI_ScreenSlashStart();
                    break;
                case AIState.ScreenSlash_PreSlash:
                    AI_ScreenSlashPreSlash();
                    break;
                case AIState.ScreenSlash_Slash:
                    AI_ScreenSlashSlash();
                    break;
                case AIState.ScreenSlash_SwordPoint:
                    AI_ScreenSlashSwordPoint();
                    break;
                case AIState.ScreenSlash_End:
                    AI_ScreenSlashEnd();
                    break;

                case AIState.SwordStarPlosion_Start:
                    AI_SwordStarPlosionStart();
                    break;
                case AIState.SwordStarPlosion_Charge:
                    AI_SwordStarPlosionCharge();
                    break;
                case AIState.SwordStarPlosion_Swing:
                    AI_SwordStarPlosionSwing();
                    break;
                case AIState.SwordStarPlosion_End:
                    AI_SwordStarPlosion_End();
                    break;

                case AIState.BlackDashStart:
                    AI_BlackDashStart();
                    break;
                case AIState.BlackDashPreDash:
                    AI_BlackDashPreDash();
                    break;
                case AIState.BlackDashDash:
                    AI_BlackDashDash();
                    break;
                case AIState.BlackDashEnd:
                    AI_BlackDashEnd();
                    break;

                case AIState.JevilScythes_Start:
                    AI_JevilScythesStart();
                    break;
                case AIState.JevilScythes_Prepare:
                    AI_JevilScythesPrepare();
                    break;
                case AIState.JevilScythes_Loop:
                    AI_JevilScythesLoop();
                    break;
                case AIState.JevilScythes_Quick:
                    AI_JevilScythesQuick();
                    break;
                case AIState.JevilScythes_End:
                    AI_JevilScythesEnd();
                    break;

                case AIState.SingularBaseball_Start:
                    AI_SingularBaseballStart();
                    break;
                case AIState.SingularBaseball_SummonBall:
                    AI_SinuglarBaseballSummonBall();
                    break;
                case AIState.SingularBaseball_HitBall:
                    AI_SingularBaseballHitBall();
                    break;
                case AIState.SingularBaseball_FindBall:
                    AI_SingularBaseballFindBall();
                    break;
                case AIState.SingularBaseball_End:
                    AI_SingularBaseballEnd();
                    break;

                case AIState.Kick_Start:
                    AI_KickStart();
                    break;
                case AIState.Kick_Run:
                    AI_KickRun();
                    break;
                case AIState.Kick_Kick:
                    AI_KickKick();
                    break;
                case AIState.Kick_Fail:
                    AI_KickFail();
                    break;
                case AIState.Kick_Fly:
                    AI_KickFly();
                    break;
                case AIState.Kick_SwordThrowDown:
                    AI_KickSwordThrowDown();
                    break;
                case AIState.Kick_End:
                    AI_KickEnd();
                    break;

                case AIState.Special_Warn:
                    AI_SpecialWarn();
                    break;
                case AIState.Special_Warn2:
                    AI_SpecialWarn2();
                    break;
                case AIState.Special_HandStab:
                    AI_SpecialHandStab();
                    break;
                case AIState.Special_DripDrop:
                    AI_SpecialDripDrop();
                    break;
                case AIState.Special_FadeToBlack:
                    AI_SpecialFadeToBlack();
                    break;
                case AIState.Special_MakeBox:
                    AI_SpecialMakeBox();
                    break;
                case AIState.Special_FadeOutFromBlack:
                    AI_SpecialFadeOutFromBlack();
                    break;
                case AIState.Special_SlashQuickStart:
                    AI_SpecialSlashQuickStart();
                    break;
                case AIState.Special_Slash:
                    AI_SpecialSlash();
                    break;
                case AIState.Special_SlashReposition:
                    AI_SpecialSlashReposition();
                    break;
                case AIState.Special_SlashEndInBlack:
                    AI_SpecialSlashEndP1();
                    break;
                case AIState.Special_SlashEndOutBlack:
                    AI_SpecialSlashEndP2();
                    break;

                case AIState.Death_Start:
                    AI_DeathStart();
                    break;
                case AIState.Death_FlyOff:
                    AI_DeathFlyOff();
                    break;
            }

            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.3f);
        
            for (int i = OldFrame.Length - 1; i > 0; i--)
            {
                OldFrame[i] = OldFrame[i - 1];
            }
            OldFrame[0] = NPC.frame;


            for (int i = OldTexture.Length - 1; i > 0; i--)
            {
                OldTexture[i] = OldTexture[i - 1];
            }
            string texture = Texture + Animator.GetAnimation();
            OldTexture[0] = texture;
            NPC.spriteDirection = -NPC.direction;
        }
        public void GetSword()
        {
            SwitchState(AIState.Intro_SwordHold);
            NPC.netUpdate = true;
        }
        public void GetSingularity()
        {
            _startedFight = true;
            SwitchState(AIState.Intro_HandOut);
            NPC.netUpdate = true;
        }
        public void StartFight()
        {
         
            SwitchState(AIState.Intro_HeadTurn);
            NPC.netUpdate = true;
        }

        private Vector2 CalculateHoverVelocity()
        {
            Vector2 hoverVelocity = Vector2.Zero;
            hoverVelocity.Y = MathF.Sin(_hoverTimer * 0.025f);
            return hoverVelocity;
        }

        private void AI_Idle()
        {
            _attackNumber = 0;
            _bounceAttackNumber = 0;
            Timer++;
            if (Timer >= 15)
            {
                ChooseAttack();
            }
        }

        private void AI_Despawn()
        {
            Timer++;
            if (Timer == 1)
            {
                ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.Black, 1f, 160);
            }
            NPC.velocity.X *= 0.2f;
            NPC.velocity.Y -= 0.2f;
            Invert invert = ScreenShader.GetInstance<Invert>();
            invert.alpha = 1f;
            if (Timer >= 150)
            {
                NPC.active = false;
            }
        }

        private void ChooseAttack()
        {
            SwitchState(PatternManager.NextPattern());
        }

        private void AI_DeathStart()
        {
            Timer++;
            if(Timer == 1)
            {
                TargetVector = NPC.velocity;
                Cutscene.StartCutscene<EPostFightCutscene>();
            }

            if(Timer < 60)
            {
                Animator.PlayAnimation(Anim_Holding);
            }
            else
            {
                Animator.PlayAnimation(Anim_BattleIdle);
            }

            Vector2 hoverVelocity = CalculateHoverVelocity();
            NPC.velocity = Vector2.Lerp(NPC.velocity, hoverVelocity, EasingFunction.InOutSine(Timer / 60f));
            //Face away the player
            NPC.direction = TargetDirection;
            Main.windSpeedCurrent = 0;
            if(Timer >= 120 && !SequencerPlayer.IsActive())
            {
                SwitchState(AIState.Death_FlyOff);
            }
        }

        private void AI_DeathFlyOff()
        {
            Timer++;
            NPC.velocity.X *= 0.9f;
            NPC.velocity.Y -= 0.5f;
            if(Timer >= 100)
            {
                NPC.Kill();
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            if(State != AIState.Death_Start && State != AIState.Death_FlyOff && NPC.life <= 0)
            {
                SwitchState(AIState.Death_Start);
            }

            if (NPC.life <= 0)
                NPC.life = 1;
        }
    }
}
