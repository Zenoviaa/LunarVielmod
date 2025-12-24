using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Areas.SpringHills.BossesSH.Minerva.Projectiles;
using Stellamod.Content.Gores;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.BossesSH.Minerva
{
    public class Minerva : ScarletBoss,
        IDrawOutlines
    {
        private enum AnimationState
        {
            IdleDance,
            SpinDanceStartup,
            SpinDance,
            LeafJump,
            LeafGlide,
            LeafGlideLand,
            KnifeJump,
            KnifeSpin,
            KnifeTrowAerial,
            Bow,
            GroundedKnivesWindup,
            KnifeThrowGrounded,
            Stunned
        }

        private enum AIState
        {

            Idle,


            SpinDashWindup,
            SpinDash,
            SpinDashEnd,

            LeafGlideJump,
            LeafGlideSpin,
            LeafGlideLand,

            KnifeThrowJump,
            KnifeThrowSpin,
            KnifeThrowDaggers,

            LongBow,
            BowWindup,
            BowDaggerThrow,

            Stunned,
            Death,
            Despawn

        }

        private bool _namePlate;
        private bool _resetAnimation;
        private bool _phase2;
        private float _afterImageTime;
        private float _dashXSpeed;
        private Vector2 _scale;
        private AnimationState _animation;

        private Color _outlineColor;
        private Color _haloColor;
        private Vector2 TargetScale;
        private Color TargetOutlineColor;
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float Cycle => ref NPC.ai[2];
        private ref float AttackCycle => ref NPC.ai[3];
        private Player Target => Main.player[NPC.target];

        private int LeafBoomerangDamage => 15;
        private int LeafBladeDamage => 25;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 32;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            Main.npcFrameCount[Type] = 46;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _scale = Vector2.One;
            TargetScale = Vector2.One;
            NPC.width = 64;
            NPC.height = 100;
            NPC.damage = 32;
            NPC.defense = 10;
            NPC.lifeMax = 1500;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = Item.buyPrice(silver: 50);
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.boss = true;
            NPC.npcSlots = 10f;

            //Setup the music and boss bar
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Minerva");
            NPC.aiStyle = -1;
        }

        private int _frame;
        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            if (_resetAnimation)
            {
                _frame = 0;
                _resetAnimation = false;
            }

            NPC.frameCounter += 0.2f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            switch (_animation)
            {
                case AnimationState.IdleDance:
                    if (_frame >= 10)
                    {
                        _frame = 0;
                    }
                    break;
                case AnimationState.SpinDanceStartup:
                    if (_frame < 10)
                    {
                        _frame = 10;
                    }
                    if (_frame >= 14)
                    {
                        _frame = 13;
                    }
                    break;
                case AnimationState.SpinDance:
                    if (_frame < 14)
                    {
                        _frame = 14;
                    }
                    if (_frame >= 18)
                    {
                        _frame = 14;
                    }
                    break;
                case AnimationState.LeafJump:
                    _frame = 18;
                    break;
                case AnimationState.LeafGlide:
                    if (_frame < 19)
                    {
                        _frame = 19;
                    }
                    if (_frame >= 23)
                    {
                        _frame = 19;
                    }
                    break;
                case AnimationState.LeafGlideLand:
                    _frame = 23;
                    break;
                case AnimationState.KnifeJump:
                    _frame = 24;
                    break;
                case AnimationState.KnifeSpin:
                    _frame = 25;
                    break;
                case AnimationState.KnifeTrowAerial:
                    NPC.frameCounter += 0.1f;
                    if (_frame < 26)
                    {
                        _frame = 26;
                    }
                    if (_frame >= 33)
                    {
                        _frame = 32;
                    }
                    break;
                case AnimationState.Bow:
                    if (_frame < 33)
                    {
                        _frame = 33;
                    }
                    if (_frame >= 39)
                    {
                        _frame = 38;
                    }
                    break;
                case AnimationState.GroundedKnivesWindup:
                    if (_frame < 39)
                    {
                        _frame = 39;
                    }
                    if (_frame >= 42)
                    {
                        _frame = 41;
                    }
                    break;
                case AnimationState.KnifeThrowGrounded:
                    NPC.frameCounter += 0.1f;
                    if (_frame < 42)
                    {
                        _frame = 42;
                    }
                    if (_frame >= 45)
                    {
                        _frame = 44;
                    }
                    break;
                case AnimationState.Stunned:
                    _frame = 45;
                    break;
            }
            NPC.frame.Y = frameHeight * _frame;
        }



        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && (State == AIState.SpinDash);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_dashXSpeed);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _dashXSpeed = reader.ReadSingle();
        }
        private void SwitchState(AIState state)
        {
            _resetAnimation = true;
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                if (state == AIState.Idle)
                {
                    Cycle = 0;
                }
                NPC.netUpdate = true;
            }
        }

        public override void AI()
        {
            base.AI();
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.3f);
            _scale = Vector2.Lerp(_scale, TargetScale, 0.1f);
            if (NPC.life < NPC.lifeMax / 2f && !_phase2 && State != AIState.Stunned)
            {
                SwitchState(AIState.Stunned);
            }
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget && State != AIState.Despawn)
                {
                    SwitchState(AIState.Despawn);
                }
            }
            switch (State)
            {
                case AIState.Despawn:
                    AI_Despawn();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    if (_phase2)
                    {
                        AI_Idle();
                    }
                    break;
                case AIState.SpinDashWindup:
                    AI_SpinDashWindup();
                    break;
                case AIState.SpinDash:
                    AI_SpinDash();
                    break;
                case AIState.SpinDashEnd:
                    AI_SpinDashEnd();
                    break;
                case AIState.LeafGlideJump:
                    AI_LeafGlideJump();
                    break;
                case AIState.LeafGlideSpin:
                    AI_LeafGlideSpin();
                    break;
                case AIState.LeafGlideLand:
                    AI_LeafGlideLand();
                    break;
                case AIState.KnifeThrowJump:
                    AI_KnifeThrowJump();
                    break;
                case AIState.KnifeThrowSpin:
                    AI_KnifeThrowSpin();
                    break;
                case AIState.KnifeThrowDaggers:
                    AI_KnifeThrowDaggers();
                    break;
                case AIState.LongBow:
                    AI_LongBow();
                    break;
                case AIState.BowWindup:
                    AI_BowWindup();
                    break;
                case AIState.BowDaggerThrow:
                    AI_BowDaggerThrow();
                    break;
                case AIState.Stunned:
                    AI_Stunned();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;
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
        private void AI_Despawn()
        {
            Timer++;
            NPC.noTileCollide = true;
            NPC.velocity.X *= 0.9f;
            if (NPC.velocity.Y < 0)
                NPC.velocity.Y *= 0.8f;
            NPC.noGravity = false;
            if(Timer >= 100)
            {
                NPC.active = false;
            }
        }

        private void AI_Death()
        {
            Timer++;
            _animation = AnimationState.Stunned;
            if(Timer == 1)
            {
                NPC.velocity.Y = -5;
                SoundStyle death = AssetRegistry.Sounds.Minerva.MinervaDeath;
                SoundEngine.PlaySound(death, NPC.position);
            }

            if(Timer < 60)
                RetargetCameraModifier.ReTargetPosition = NPC.Center;
            NPC.rotation += NPC.direction * 0.05f;
            NPC.noTileCollide = true;
            NPC.velocity.X *= 0.91f;
            if (NPC.velocity.Y < 10)
            {
                NPC.velocity.Y += 0.5f;
            }
            if (Timer % 5 == 0)
            {
                int gore1 = GoreHelper.TypeFallingLeafWhite;
                int gore2 = GoreHelper.TypeFallingLeafRed;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Gore.NewGore(SourceFromThis, NPC.Center + Main.rand.NextVector2Circular(4, 4), velocity, gore1);
                }

            }


            TargetOutlineColor = Color.Transparent;
            if (Timer >= 130)
            {
                FXUtil.ShakeCamera(NPC.position, 1024, 12);
                var p =FXUtil.GlowCircleBoom(NPC.Center, Color.Green, Color.GreenYellow, Color.DarkGreen);
                p.Scale *= 2f;
                var p2 = FXUtil.GlowCircleBoom(NPC.Center, Color.Green, Color.GreenYellow, Color.DarkGreen);
                p2.Scale *= 1.2f;


                FXUtil.GlowCircleBoom(NPC.Center,
                   innerColor: Color.White,
                   glowColor: Color.Yellow,
                   outerGlowColor: Color.Red, duration: 15, baseSize: 0.12f);

                FXUtil.ShakeCamera(NPC.Center, 1024, 32);
                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, NPC.position);

                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Green, 1f).noGravity = true;
                }
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkOliveGreen, 1f).noGravity = true;
                }

                FXUtil.GlowCircleBoom(NPC.Center,
                    innerColor: Color.Yellow,
                    glowColor: Color.Green,
                    outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
                CreateJumpParticle();
                CreateIvythornSplash(NPC.Center, Vector2.Zero);
                NPC.Kill();
            }
        }

        private void AI_Stunned()
        {
            Timer++;
            if (Timer == 1)
            {
                _phase2 = true;
                NPC.velocity.Y = -8;
                SoundStyle stunned = AssetRegistry.Sounds.Minerva.Stunned;
                SoundEngine.PlaySound(stunned, NPC.position);
                FXUtil.ShakeCamera(NPC.position, 1024, 4);
            }
        
            _animation = AnimationState.Stunned;
            _haloColor = Color.Lerp(_haloColor, Color.Goldenrod, 0.1f);
            TargetOutlineColor = Color.Transparent;
            NPC.noGravity = false;
            NPC.velocity.X *= 0.9f;
            if (Timer >= 240)
            {
            
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Idle()
        {

            if (!_namePlate)
            {
                ShowNamePlate();
                _namePlate = true;
            }
            _haloColor *= 0.5f;
            Timer++;
            _afterImageTime *= 0.9f;
            _animation = AnimationState.IdleDance;
            TargetOutlineColor = Color.Transparent;
            if(Timer == 1)
            {
                NPC.TargetClosest();
            }
        
            if (NPC.HasValidTarget)
            {
                NPC.spriteDirection = -NPC.direction;
            }
            NPC.noGravity = false;
            //During idle she just dances around in place moving side to side for a while;
            if (Timer % 60 == 0)
            {
                Cycle++;
                if (Cycle >= 4)
                {
                    Cycle = 0;
                }
                switch (Cycle)
                {
                    case 0:
                        TargetScale = Vector2.One;
                        break;
                    case 1:
                        TargetScale = new Vector2(1.05f, 0.95f);
                        break;
                    case 2:
                        TargetScale = Vector2.One;
                        break;
                    case 3:
                        TargetScale = new Vector2(0.95f, 1.05f);
                        break;
                }
                //Cute little squish
            }

            //Dance around
            float direction = Cycle % 2 == 0 ? 1 : -1;
            float xVelocity = direction * 2;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, xVelocity, 0.1f);
            NPC.rotation = NPC.velocity.X * 0.025f;

            if (Timer >= 240)
            {
                TargetScale = Vector2.One;
                ChooseAttack();

            }
        }



        #region Spin Dash
        private void AI_SpinDashWindup()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                //Set dash velocity
                _dashXSpeed = NPC.direction * 21;

                //Tiny hop should look nice
                NPC.velocity.Y = -8;


                //We're finally adding sound effects!
                SoundStyle leafSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Nature.LeafRustle1 : AssetRegistry.Sounds.Nature.LeafRustle2;
                leafSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(leafSound, NPC.position);

                SoundStyle voice1 = AssetRegistry.Sounds.Minerva.MinervaVoice1;
                SoundEngine.PlaySound(voice1, NPC.position);

                CreateJumpParticle();
                CreateIvythornSplash(NPC.Bottom, -Vector2.UnitY * 4);
                _afterImageTime = 0.5f;
                TargetScale = new Vector2(0.8f, 1.4f);
            }
            TargetScale = Vector2.Lerp(TargetScale, Vector2.One, 0.1f);
            _afterImageTime *= 0.9f;


            TargetOutlineColor = Color.Yellow;


            //Create some gores
            if (Timer % 5 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Grass);
            }


            //Slow down to get ready for the dash
            float readyTime = 35;
            float interpolant = Timer / (readyTime - 5f);
            float ease = EasingFunction.InOutSine(interpolant);
            float targetSpeed = MathHelper.Lerp(-NPC.direction * 3, 0, ease);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetSpeed, 0.1f);

            if (Timer < readyTime / 2f)
            {
                NPC.rotation += NPC.direction * 0.12f;
                _animation = AnimationState.SpinDanceStartup;
            }
            else
            {
                NPC.rotation += NPC.direction * 0.2f;
                _animation = AnimationState.KnifeSpin;
            }

            if (MultiplayerHelper.IsHost && Timer % 5 == 0 && Timer < 12)
            {
                Vector2 velocity = MyTarget.Center - NPC.Center;
                velocity = velocity.SafeNormalize(Vector2.Zero);
                velocity *= 4;
                Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                    ModContent.ProjectileType<LeafBlade>(), LeafBladeDamage, 1, Main.myPlayer);
            }
            NPC.rotation = MathHelper.WrapAngle(NPC.rotation);
            if (Timer >= readyTime && NPC.collideY)
            {
                SwitchState(AIState.SpinDash);
            }
        }

        private void AI_SpinDash()
        {
            //After image effect to make it look cooler
            _afterImageTime = MathHelper.Lerp(_afterImageTime, 1f, 0.1f);
            Timer++;
            if (Timer == 1)
            {

                SoundStyle voice1 = AssetRegistry.Sounds.Minerva.MinervaVoice2;
                SoundEngine.PlaySound(voice1, NPC.position);
                SoundStyle voice21 = AssetRegistry.Sounds.Minerva.MinervaSpin;
                SoundEngine.PlaySound(voice21, NPC.position);
                int gore1 = GoreHelper.TypeFallingLeafWhite;
                int gore2 = GoreHelper.TypeFallingLeafRed;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Gore.NewGore(SourceFromThis, NPC.Center, velocity, gore1);

                    velocity = velocity.RotatedByRandom(MathHelper.TwoPi);
                    Gore.NewGore(SourceFromThis, NPC.Center, velocity, gore2);
                }

            }

            if (Timer % 4 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Grass);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WoodFurniture);
                LegacyParticle.NewParticle<EmberParticle>(NPC.Bottom, -Vector2.UnitY, newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            if (Timer % 3 == 0)
            {
                int gore1 = GoreHelper.TypeFallingLeafWhite;
                int gore2 = GoreHelper.TypeFallingLeafRed;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Gore.NewGore(SourceFromThis, NPC.Center + Main.rand.NextVector2Circular(4, 4), velocity, gore1);
                }

            }
            TargetOutlineColor = Color.Red;
            _animation = AnimationState.SpinDance;

            float dashTicks = 55;
            float interpolant = Timer / dashTicks;

            //Anticipation ease
            //https://easingwizard.com/
            Vector2 control1 = new Vector2(0.8f, -0.4f);
            Vector2 control2 = new Vector2(0.5f, 1f);
            float easing = EasingFunction.BezierEase(interpolant, control1, control2);
            NPC.velocity.X = MathHelper.Lerp(-_dashXSpeed / 3f, _dashXSpeed, easing);


            //Create a little bit of a lean towards the direction she's moving
            float targetRotation = NPC.velocity.X * 0.025f;
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRotation, 0.1f);
            NPC.rotation = MathHelper.WrapAngle(NPC.rotation);
            if (Timer >= dashTicks + 10)
            {
                SwitchState(AIState.SpinDashEnd);
            }
        }

        private void AI_SpinDashEnd()
        {
            //After image effect to make it look cooler
            _afterImageTime *= 0.92f;
            Timer++;
            if (Timer == 1)
            {
                SoundStyle voice1 = AssetRegistry.Sounds.Minerva.MinervaVoice3;
                SoundEngine.PlaySound(voice1, NPC.position);

                NPC.velocity.Y = -8;

                //We're finally adding sound effects!
                CreateJumpParticle();
                CreateIvythornSplash(NPC.Bottom, -Vector2.UnitY * 4);
                _afterImageTime = 0.5f;
                TargetScale = new Vector2(0.8f, 1.4f);
            }

            TargetScale = Vector2.Lerp(TargetScale, Vector2.One, 0.1f);
            TargetOutlineColor = Color.Transparent;



            NPC.velocity.X *= 0.97f;


            float endTicks = 60;

            if (Timer < endTicks / 2f)
            {
                if (MultiplayerHelper.IsHost && Timer == 18)
                {
                    Vector2 velocity = MyTarget.Center - NPC.Center;
                    velocity = velocity.SafeNormalize(Vector2.Zero);
                    velocity *= 13;
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                        ModContent.ProjectileType<LeafBlade>(), LeafBladeDamage, 1, Main.myPlayer, ai1: 1);
                }
                _animation = AnimationState.KnifeSpin;
                NPC.rotation += NPC.direction * 0.2f;
            }
            else if (NPC.velocity.Y > 0)
            {
                _animation = AnimationState.LeafGlide;
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.noGravity = true;
                if (NPC.velocity.Y < 5)
                    NPC.velocity.Y += 0.5f;
                if (Timer % 5 == 0)
                {
                    CreateJumpParticle();
                }

            }


            //Create a little bit of a lean towards the direction she's moving
            if (Timer >= 5 && NPC.collideY)
            {
                SwitchState(AIState.Idle);
            }
        }

        #endregion

        #region Leaf Glide
        private void AI_LeafGlideJump()
        {

            Timer++;
            TargetOutlineColor = Color.Yellow;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle voice1 = AssetRegistry.Sounds.Minerva.MinervaVoice2;
                SoundEngine.PlaySound(voice1, NPC.position);
                SoundStyle voice21 = AssetRegistry.Sounds.Minerva.MinervaSpin;
                SoundEngine.PlaySound(voice21, NPC.position);
                int gore1 = GoreHelper.TypeFallingLeafWhite;
                int gore2 = GoreHelper.TypeFallingLeafRed;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Gore.NewGore(SourceFromThis, NPC.Center, velocity, gore1);

                    velocity = velocity.RotatedByRandom(MathHelper.TwoPi);
                    Gore.NewGore(SourceFromThis, NPC.Center, velocity, gore2);
                }

                CreateJumpParticle();
                NPC.velocity.X = NPC.direction;
                NPC.velocity.Y = -21;
                TargetScale = new Vector2(0.9f, 1.31f);
            }
            else if (Timer > 4 && Timer < 15)
            {
                _animation = AnimationState.KnifeJump;

                NPC.velocity.Y *= 0.98f;
            }
            else if (Timer < 25)
            {

                if (Timer % 3 == 0)
                {
                    int gore1 = GoreHelper.TypeFallingLeafWhite;
                    int gore2 = GoreHelper.TypeFallingLeafRed;
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                        Gore.NewGore(SourceFromThis, NPC.Center + Main.rand.NextVector2Circular(4, 4), velocity, gore1);
                    }

                }
                TargetScale = Vector2.One;
                if (NPC.velocity.Y < 0.5f)
                {
                    NPC.velocity.Y += 1;
                }

            }
            else
            {
                _animation = AnimationState.LeafJump;

                NPC.velocity.Y *= 0.98f;
                if (Timer >= 30)
                {
                    SwitchState(AIState.LeafGlideSpin);
                }
            }
        }

        private void AI_LeafGlideSpin()
        {
            _animation = AnimationState.LeafGlide;
            Timer++;
            if (Timer == 1)
            {
                SoundStyle voice1 = AssetRegistry.Sounds.Minerva.MinervaLaugh;
                SoundEngine.PlaySound(voice1, NPC.position);
            }
            TargetOutlineColor = Color.Lerp(Color.Transparent,
                Color.Lerp(Color.Transparent, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 24)), Timer % 60 / 60f);

            float targetVelocityX = NPC.direction * 0.4f;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocityX, 0.1f);
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0.2f, 0.1f);
            NPC.rotation = NPC.velocity.X * 0.05f;
            NPC.noGravity = true;

            if (Timer % 60 == 0)
            {
                NPC.velocity.X = -NPC.direction * 3;
                SoundStyle voice1 = AssetRegistry.Sounds.Minerva.MinervaVoice1;
                SoundEngine.PlaySound(voice1, NPC.position);
                if (MultiplayerHelper.IsHost)
                {
                    float speedModifier = MathHelper.Lerp(0.5f, 2f, Timer / 240f);
                    Vector2 leftVelocity = -Vector2.UnitX * 10 * speedModifier;
                    Vector2 rightVelocity = Vector2.UnitX * 10 * speedModifier;
                    var source = NPC.GetSource_FromThis();
                    int projType = ModContent.ProjectileType<LeafBoomerang>();
                    Projectile.NewProjectile(source, NPC.Center, leftVelocity, projType, LeafBoomerangDamage, 1, Main.myPlayer);
                    Projectile.NewProjectile(source, NPC.Center, rightVelocity, projType, LeafBoomerangDamage, 1, Main.myPlayer);
                }
            }
            if (Timer >= 240)
            {
                SwitchState(AIState.LeafGlideLand);
            }
        }

        private void AI_LeafGlideLand()
        {
            _animation = AnimationState.LeafGlideLand;
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.Y = 1;
            }
            if (Timer % 3 == 0)
            {
                int gore1 = GoreHelper.TypeFallingLeafWhite;
                int gore2 = GoreHelper.TypeFallingLeafRed;
                for (int i = 0; i < 1; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Gore.NewGore(SourceFromThis, NPC.Center + Main.rand.NextVector2Circular(4, 4), velocity, gore1);
                }
            }

            if (NPC.collideY || Timer >= 30)
            {
                SwitchState(AIState.Idle);
            }
        }
        #endregion

        #region Aerial Knife Throw
        private void AI_KnifeThrowJump()
        {
            _animation = AnimationState.KnifeJump;
            Timer++;
            TargetOutlineColor = Color.Yellow;
            NPC.noGravity = false;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.velocity.X = NPC.direction;
                NPC.velocity.Y = -14;
                TargetScale = new Vector2(0.9f, 1.5f);
                CreateJumpParticle();
                CreateIvythornSplash(NPC.Bottom, -Vector2.UnitY * 2);

            }
            else if (Timer > 4 && Timer < 15)
            {
                NPC.velocity.Y *= 0.98f;
            }
            else if (Timer < 25)
            {
                TargetScale = Vector2.One;
            }
            else
            {
                NPC.velocity.Y *= 0.98f;
                if (Timer >= 30)
                {
                    SwitchState(AIState.KnifeThrowSpin);
                }
            }
        }

        private void AI_KnifeThrowSpin()
        {
            _animation = AnimationState.KnifeSpin;
            _afterImageTime = MathHelper.Lerp(_afterImageTime, 1f, 0.1f);

            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.velocity.X = NPC.direction;
                SoundStyle voice1 = AssetRegistry.Sounds.Minerva.MinervaVoice2;
                SoundEngine.PlaySound(voice1, NPC.position);

                SoundStyle spins2 = AssetRegistry.Sounds.Minerva.MinervaSpin;
                SoundEngine.PlaySound(spins2, NPC.position);
            }
            if (Timer % 3 == 0)
            {
                int gore1 = GoreHelper.TypeFallingLeafWhite;
                int gore2 = GoreHelper.TypeFallingLeafRed;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Gore.NewGore(SourceFromThis, NPC.Center + Main.rand.NextVector2Circular(4, 4), velocity, gore1);
                }

            }

            if (Timer % 5 == 0)
            {
                var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity);
                p.fadeToColor = Color.DarkGreen;
                p.shrink = true;
                p.color *= 0.8f;
                p.Scale *= 0.6f;
            }
            TargetOutlineColor = Color.Yellow;
            NPC.velocity.Y *= 0.9f;
            NPC.velocity.X *= 1.1f;
            NPC.rotation += NPC.direction * 0.15f;
            if (Timer >= 30)
            {
                SwitchState(AIState.KnifeThrowDaggers);
            }
        }

        private void AI_KnifeThrowDaggers()
        {
            _animation = AnimationState.KnifeTrowAerial;
            Timer++;
            TargetOutlineColor = Color.Red;

            if (Timer == 1)
            {
                NPC.rotation = NPC.direction * 0.05f;
            }

            if (Timer == 7)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 rightVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15;
                    var source = NPC.GetSource_FromThis();
                    int projType = ModContent.ProjectileType<LeafBlade>();
                    Projectile.NewProjectile(source, NPC.Center, rightVelocity, projType, LeafBoomerangDamage, 1, Main.myPlayer);
                }
                SoundStyle voice1 = AssetRegistry.Sounds.Minerva.MinervaVoice1;
                SoundEngine.PlaySound(voice1, NPC.position);
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 leftVelocity = Vector2.UnitX * 7 * -NPC.direction;
                    var source = NPC.GetSource_FromThis();
                    int projType = ModContent.ProjectileType<LeafBoomerang>();
                    Projectile.NewProjectile(source, NPC.Center, leftVelocity, projType, LeafBoomerangDamage, 1, Main.myPlayer);

                }
            }

            if (Timer == 15)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 rightVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15;
                    var source = NPC.GetSource_FromThis();
                    int projType = ModContent.ProjectileType<LeafBlade>();
                    Projectile.NewProjectile(source, NPC.Center, rightVelocity, projType, LeafBoomerangDamage, 1, Main.myPlayer);
                }
            }
            if (Timer >= 15)
            {
                NPC.rotation -= NPC.direction * NPC.velocity.Length() * 0.0025f;
                NPC.noGravity = true;
                if (NPC.velocity.Y < 5)
                    NPC.velocity.Y += 0.5f;
                if (NPC.collideY)
                {
                    SoundStyle voice = AssetRegistry.Sounds.Minerva.MinervaVoice3;
                    voice.PitchVariance = 0.3f;
                    SoundEngine.PlaySound(voice, NPC.position);

                    CreateIvythornSplash(NPC.position, NPC.direction * Vector2.UnitX);
                    _scale = new Vector2(1.5f, 0.85f);
                    Cycle++;
                    if (Cycle >= 3)
                    {
                        SwitchState(AIState.LeafGlideLand);
                    }
                    else
                    {
                        SwitchState(AIState.KnifeThrowJump);
                    }
                }
            }
            else
            {
                NPC.velocity.Y = 1;
                NPC.velocity.Y *= 0.8f;
            }
        }
        #endregion

        #region Grounded Knives
        private void AI_LongBow()
        {
            _animation = AnimationState.Bow;
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle spins2 = AssetRegistry.Sounds.Minerva.MinervaLaugh;
                SoundEngine.PlaySound(spins2, NPC.position);
            }
            TargetOutlineColor = Color.Yellow;
            NPC.velocity.X = 0;
            if (Timer >= 60)
            {
                SwitchState(AIState.BowWindup);
            }
        }

        private void AI_BowWindup()
        {
            _animation = AnimationState.GroundedKnivesWindup;
            Timer++;
            if (Timer == 1)
            {


                if (Cycle == 2)
                {
                    SoundStyle voice3 = AssetRegistry.Sounds.Minerva.MinervaVoice3;
                    SoundEngine.PlaySound(voice3, NPC.position);
                }
                else
                {
                    SoundStyle spins2 = AssetRegistry.Sounds.Minerva.MinervaVoice1;
                    SoundEngine.PlaySound(spins2, NPC.position);

                }
            }
            TargetOutlineColor = Color.Yellow;
            NPC.velocity.X *= 0.93f;
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (Timer >= 30)
            {
                SwitchState(AIState.BowDaggerThrow);
            }
        }

        private void AI_BowDaggerThrow()
        {
            _animation = AnimationState.KnifeThrowGrounded;
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.X = NPC.direction * 2;
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 rightVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15;

                    var source = NPC.GetSource_FromThis();
                    int projType = ModContent.ProjectileType<LeafBlade>();
                    Projectile.NewProjectile(source, NPC.Center,
                        rightVelocity, projType, LeafBladeDamage, 1, Main.myPlayer, ai1: 1);
                    Projectile.NewProjectile(source, NPC.Center,
                        rightVelocity.RotatedBy(MathHelper.ToRadians(-22)), projType, LeafBladeDamage, 1, Main.myPlayer, ai1: 1);
                    Projectile.NewProjectile(source, NPC.Center,
                          rightVelocity.RotatedBy(MathHelper.ToRadians(22)), projType, LeafBladeDamage, 1, Main.myPlayer, ai1: 1);
                }
            }
            TargetOutlineColor = Color.Red;
            if (Timer >= 12)
            {
                _resetAnimation = true;
                Cycle++;
                if (Cycle >= 3)
                {
                    SwitchState(AIState.LeafGlideLand);
                }
                else
                {
                    SwitchState(AIState.BowWindup);
                }
            }
        }
        #endregion

        public override void OnKill()
        {
            base.OnKill();
            DownedBossTracker.ClearFlag(DownedBossFlag.Minerva);
        }

        private void DoRandomAttack()
        {
            int rand = Main.rand.Next(0, 4);
            switch (rand)
            {
                case 0:
                    SwitchState(AIState.SpinDashWindup);
                    break;
                case 1:
                    SwitchState(AIState.LeafGlideJump);
                    break;
                case 2:
                    SwitchState(AIState.LongBow);
                    break;
                case 3:
                    SwitchState(AIState.KnifeThrowJump);
                    break;
            }
        }
        private void ChooseAttack()
        {
            if (MultiplayerHelper.IsHost)
            {

                switch (AttackCycle)
                {
                    case 0:
                        SwitchState(AIState.SpinDashWindup);
                        break;
                    case 1:
                        SwitchState(AIState.KnifeThrowJump);
                        break;
                    case 2:
                        SwitchState(AIState.SpinDashWindup);
                        break;
                    case 3:
                        SwitchState(AIState.LeafGlideJump);
                        break;
                    case 4:
                        SwitchState(AIState.LongBow);
                        break;
                    case 5:
                        SwitchState(AIState.SpinDashWindup);
                        break;
                    case 6:
                        DoRandomAttack();
                        break;
                    case 7:
                        DoRandomAttack();
                        break;
                }
                AttackCycle++;
                if (AttackCycle >= 8)
                {
                    AttackCycle = 0;
                }
            }
        }
        #region Draw Code
        private GlowDonutParticle CreateJumpParticle()
        {
            var jumpParticle = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
            jumpParticle.Scale *= 1;
            jumpParticle.fadeToColor = Color.DarkGreen;
            jumpParticle.shrink = true;
            return jumpParticle;

        }
        private void CreateIvythornSplash(Vector2 position, Vector2 velocity)
        {
            int[] gores = AutoGoreLoader.FindGores("IvynWood");
            foreach (int g in gores)
            {
                Gore.NewGore(NPC.GetSource_FromThis(),
                    position,
                    velocity.RotatedByRandom(MathHelper.ToRadians(20)), g, 1f);
            }
            SoundStyle soundStyle = AssetRegistry.Sounds.Magic.VineWrap;
            soundStyle.PitchVariance = 0.3f;
            SoundEngine.PlaySound(soundStyle, position);

            for (float f = 0; f < 16; f++)
            {
                Vector2 vel = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                vel *= Main.rand.NextFloat(0f, 1f);
                Dust.NewDustPerfect(position, DustID.t_LivingWood, vel, newColor: Color.White);
            }
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(48, 32, completionRatio);
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White * 0.5f, Color.White * 0f, completionRatio) * _afterImageTime;
        }

        private void DrawWindTrailing()
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.Dashtrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, ColorFunction, WidthFunction, shader, offset: NPC.Size / 2);
        }
        private void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawOrigin = NPC.frame.Size() / 2;
            spriteBatch.Draw(texture, drawPos, NPC.frame, Color.White.MultiplyRGBA(drawColor), NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawWindTrailing();
            DrawAfterImage(spriteBatch, screenPos);
            Draw(spriteBatch, screenPos, drawColor);
            return false;
        }

        private void DrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 oldDrawPos = oldPos - screenPos;

                float f = i;
                float interpolant = f / (float)NPC.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant);
                fadeColor *= _afterImageTime;
                oldDrawPos += NPC.Size / 2f;
                fadeColor *= 0.1f;
                spriteBatch.Draw(texture, oldDrawPos, NPC.frame, fadeColor, NPC.oldRot[i], drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 h = Vector2.UnitX * outlineOffset;
            Vector2 v = Vector2.UnitY * outlineOffset;
            Draw(spriteBatch, screenPos + h, _outlineColor);
            Draw(spriteBatch, screenPos - h, _outlineColor);
            Draw(spriteBatch, screenPos + v, _outlineColor);
            Draw(spriteBatch, screenPos - v, _outlineColor);

        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);
            if(State == AIState.Stunned)
            {
                DrawHelper.DrawHalo(NPC.Center - new Vector2(0, 54), _haloColor, 3);
            }
        }
        #endregion
    }
}
