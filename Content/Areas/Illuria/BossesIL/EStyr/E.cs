using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Content.Areas.Abyss.BossesAB.VerlianSingularity;
using Stellamod.Core;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
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
            JevilScythes_Loop,
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
            
            Dismantle_Start,
            Dismantle_Slash,
            Dismantle_End
        }

        private bool _intro;
        private bool _showNamePlate;
        private bool _contactDamage;

        private float _attackNumber;
        private float _hoverTimer;
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
                if(_patternManagerBackingField == null)
                {
                    _patternManagerBackingField = new PatternManager<AIState>(new Tuple<AIState, float>(AIState.ForwardSlash_Start, 1.0f));
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
            Main.npcFrameCount[NPC.type] = 34;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
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

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/EStyr");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_forwardVector);
            writer.Write(_hoverTimer);
            writer.Write(_attackNumber);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _forwardVector = reader.ReadVector2();  
            _hoverTimer = reader.ReadSingle();
            _attackNumber = reader.ReadSingle();
        }

        private void EnablePlatformArena()
        {
            SingularityFallSystem fallSystem = ModContent.GetInstance<SingularityFallSystem>();
            fallSystem.noWings = true;
            fallSystem.inSpace = true;
            fallSystem.hoveringPlatform = true;
            fallSystem.hoverPlatformY = 16000;
        }

        private void UpdateClient()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (_intro)
            {
                NPC.boss = true;
                BlackSea blackSea = ScreenShader.GetInstance<BlackSea>();
                blackSea.alpha = 1f;

                BlackSeaRenderingEdit blackseaRenderer = ModContent.GetInstance<BlackSeaRenderingEdit>();
                blackseaRenderer.drawBlackSea = true;
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

            UpdateClient();
            _contactDamage = false;
            _isGrabbing = false;
            _hoverTimer++;
         

            if(State != AIState.Despawn && !NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget)
                {
                    SwitchState(AIState.Despawn);
                }
            }
            _telegraphLineAlpha = 0;
            _drawScale = Vector2.One;
            TargetOutlineColor = Color.White;
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
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
                case AIState.JevilScythes_Loop:
                    AI_JevilScythesLoop();
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

                case AIState.Dismantle_Start:
                    AI_DismantleStart();
                    break;
                case AIState.Dismantle_Slash:
                    AI_DismantleSlash();
                    break;
                case AIState.Dismantle_End:
                    AI_DismantleEnd();
                    break;
            }
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            for (int i = OldFrame.Length - 1; i > 0; i--)
            {
                OldFrame[i] = OldFrame[i - 1];
            }
            OldFrame[0] = NPC.frame;
            NPC.spriteDirection = NPC.direction;
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
            Timer++;
            if(Timer >= 15)
            {
                ChooseAttack();
            }
        }

        private void AI_Despawn()
        {
            Timer++;
            if(Timer == 1)
            {
                ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.Black, 1f, 160);
            }
            NPC.velocity.X *= 0.2f;
            NPC.velocity.Y -= 0.2f;
            Invert invert = ScreenShader.GetInstance<Invert>();
            invert.alpha = 1f;
            if(Timer >= 150)
            {
                NPC.active = false;
            }
        }

        private void ChooseAttack()
        {
            SwitchState(AIState.Tornado_Start);
        }
    }
}
