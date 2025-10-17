using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.SpringHills.BossesSH.Minerva.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.UI.DialogueTowning.DialogueTowningUISystem;

namespace Stellamod.Content.Areas.SpringHills.BossesSH.Minerva
{
    public class Minerva : ScarletBoss
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

            LeafGlideJump,
            LeafGlideSpin,
            LeafGlideLand,

            KnifeThrowJump,
            KnifeThrowSpin,
            KnifeThrowDaggers,

            LongBow,
            BowWindup,
            BowDaggerThrow,

            Stunned

        }

        private bool _resetAnimation;
        private bool _phase2;
        private float _dashXSpeed;
        private Vector2 _scale;
        private AnimationState _animation;
        private bool _beatHit;
        private float _beatCounter;
        private Color _outlineColor;

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
            NPC.width = 64;
            NPC.height = 48;
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
                    if(_frame >= 10)
                    {
                        _frame = 0;
                    }
                    break;
                case AnimationState.SpinDanceStartup:
                    if(_frame < 10)
                    {
                        _frame = 10;
                    }
                    if(_frame >= 14)
                    {
                        _frame = 13;
                    }
                    break;
                case AnimationState.SpinDance:
                    if(_frame < 14)
                    {
                        _frame = 14;
                    }
                    if(_frame >= 18)
                    {
                        _frame = 14;
                    }
                    break;
                case AnimationState.LeafJump:
                    _frame = 18;
                    break;
                case AnimationState.LeafGlide:
                    if(_frame < 19)
                    {
                        _frame = 19;
                    }
                    if(_frame >= 23)
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
                    if(_frame < 26)
                    {
                        _frame = 26;
                    }
                    if(_frame >= 33)
                    {
                        _frame = 32;
                    }
                    break;
                case AnimationState.Bow:
                    if(_frame < 33)
                    {
                        _frame = 33;
                    }
                    if(_frame >= 39)
                    {
                        _frame = 38;
                    }
                    break;
                case AnimationState.GroundedKnivesWindup:
                    if(_frame < 39)
                    {
                        _frame = 39;
                    }
                    if(_frame >= 42)
                    {
                        _frame = 41;
                    }
                    break;
                case AnimationState.KnifeThrowGrounded:
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
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.position - screenPos + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
            drawPos.Y -= 12;

            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;

            Color outlineColor = _outlineColor;
            Vector2 drawOrigin = NPC.frame.Size() / 2;
            spriteBatch.Restart(effect: whiteShader.Effect);


            spriteBatch.Draw(texture, drawPos + left, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);

            spriteBatch.RestartDefaults();
            spriteBatch.Draw(texture, drawPos, NPC.frame, Color.White.MultiplyRGB(drawColor), NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
            return false;
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
            if (StellaMultiplayer.IsHost)
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
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            _scale = Vector2.Lerp(_scale, TargetScale, 0.1f);
            if (NPC.life < NPC.lifeMax / 2f && !_phase2 && State != AIState.Stunned)
            {
                SwitchState(AIState.Stunned);
            }
            switch (State)
            {
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
            }
        }
        private void AI_Stunned()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.Y = -5;
            }
            _animation = AnimationState.Stunned;
            NPC.velocity.X *= 0.9f;
            if (Timer >= 240)
            {
                _phase2 = true;
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Idle()
        {
            Timer++;
            _animation = AnimationState.IdleDance;
            TargetOutlineColor = Color.Transparent;
            NPC.TargetClosest();
            if (NPC.HasValidTarget)
            {
                NPC.spriteDirection = -NPC.direction;
            }

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
                //  ChooseAttack();
                SwitchState(AIState.SpinDashWindup);
            }
        }

        #region Spin Dash
        private void AI_SpinDashWindup()
        {
            Timer++;
            if (Timer == 1)
            {
                //Set dash velocity
                _dashXSpeed = NPC.direction * 15;
            }

            _animation = AnimationState.SpinDanceStartup;
            TargetOutlineColor = Color.Yellow;


            float readyTime = 35;
            float interpolant = Timer / (readyTime - 5f);
            float ease = EasingFunction.InOutSine(interpolant);
            float targetSpeed = MathHelper.Lerp(-NPC.direction * 3, 0, ease);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetSpeed, 0.1f);
            if (Timer >= readyTime)
            {
                SwitchState(AIState.SpinDash);
            }
        }

        private void AI_SpinDash()
        {
            Timer++;
            if (Timer == 1)
            {

            }

            if(Timer % 4 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Grass);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WoodFurniture);
            }
            TargetOutlineColor = Color.Red;
            _animation = AnimationState.SpinDance;

            float dashTicks = 60;
            float interpolant = Timer / dashTicks;
            float ease = EasingFunction.InExpo(interpolant);
            NPC.velocity.X = MathHelper.Lerp(_dashXSpeed, 0f, ease);
            if (Timer >= dashTicks)
            {
                SwitchState(AIState.Idle);
            }
        }

        #endregion

        #region Leaf Glide
        private void AI_LeafGlideJump()
        {
            _animation = AnimationState.LeafJump;
            Timer++;
            TargetOutlineColor = Color.Yellow;
            if (Timer == 1)
            {
                NPC.velocity.X = NPC.direction;
                NPC.velocity.Y = -25;
                TargetScale = new Vector2(0.9f, 1.1f);
            }
            else if (Timer > 4 && Timer < 15)
            {
                NPC.velocity.Y *= 0.98f;
            }
            else if (Timer < 25)
            {
                TargetScale = Vector2.One;
                if (NPC.velocity.Y < 0.5f)
                {
                    NPC.velocity.Y += 1;
                }
            }
            else
            {
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
            TargetOutlineColor = Color.Red;

            float targetVelocityX = NPC.direction * 0.4f;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocityX, 0.1f);
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0.2f, 0.1f);
            NPC.rotation = NPC.velocity.X * 0.05f;

            if (Timer % 70 == 0)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 leftVelocity = -Vector2.UnitX * 10;
                    Vector2 rightVelocity = Vector2.UnitX * 10;
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
            if (Timer == 1)
            {
                NPC.velocity.X = NPC.direction;
                NPC.velocity.Y = -15;
                TargetScale = new Vector2(0.9f, 1.1f);
            }
            else if (Timer > 4 && Timer < 15)
            {
                NPC.velocity.Y *= 0.98f;
            }
            else if (Timer < 25)
            {
                TargetScale = Vector2.One;
                if (NPC.velocity.Y < 1)
                {
                    NPC.velocity.Y += 1;
                }
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
            Timer++;
            TargetOutlineColor = Color.Yellow;
            NPC.velocity.Y *= 0.9f;
            NPC.velocity.X *= 0.9f;
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
            NPC.velocity.Y = 1;
            NPC.velocity.Y *= 0.8f;
            if (Timer >= 15)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 rightVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15;
                    var source = NPC.GetSource_FromThis();
                    int projType = ModContent.ProjectileType<LeafBlade>();
                    Projectile.NewProjectile(source, NPC.Center, rightVelocity, projType, LeafBoomerangDamage, 1, Main.myPlayer);
                }
                Cycle++;
                if (Cycle >= 3)
                {
                    SwitchState(AIState.LeafGlideLand);
                }
                else
                {
                    SwitchState(AIState.KnifeThrowSpin);
                }
            }
        }
        #endregion

        #region Grounded Knives
        private void AI_LongBow()
        {
            _animation = AnimationState.Bow;
            Timer++;
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
            TargetOutlineColor = Color.Yellow;
            NPC.velocity.X = 0;
            if (Timer >= 30)
            {
                SwitchState(AIState.BowDaggerThrow);
            }
        }

        private void AI_BowDaggerThrow()
        {
            _animation = AnimationState.KnifeThrowGrounded;
            Timer++;
            TargetOutlineColor = Color.Red;
            if (Timer >= 15)
            {
                NPC.velocity.X = -NPC.direction;
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 rightVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15;
                    var source = NPC.GetSource_FromThis();
                    int projType = ModContent.ProjectileType<LeafBlade>();
                    Projectile.NewProjectile(source, NPC.Center, rightVelocity, projType, LeafBladeDamage, 1, Main.myPlayer);
                }
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
            if (StellaMultiplayer.IsHost)
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
    }

}
