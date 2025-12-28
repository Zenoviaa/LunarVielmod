using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Items.Accessories.Foods;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.Event
{
    public class GintzeWarrior : BaseColosseumNPC,
        IDrawOutlines
    {
        private bool _pauseAnimation;
        private Color _outlineColor;
        private Color TargetOutlineColor;
        private enum AIState
        {
            Chase,
            Jump,
            Dash,
            Dash_Start
        }
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private float DirectionToTarget
        {
            get
            {
                if (Target.Center.X < NPC.Center.X)
                {
                    return -1;
                }
                return 1;
            }
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10, 1, 3));
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.lifeMax = 200;
            NPC.defense = 7;
            NPC.value = 65f;
            NPC.knockBackResist = 0.55f;
            NPC.width = 30;
            NPC.height = 50;
            NPC.damage = 34;
            NPC.scale = 1.0f;
            NPC.aiStyle = -1;
            NPC.lavaImmune = false;
            NPC.alpha = 0;
            NPC.dontTakeDamage = false;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            GintzeHitEffect(hit);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && State == AIState.Dash;
        }
        private int _frame = 0;
        public override void FindFrame(int frameHeight)
        {
            if (_pauseAnimation)
                return;

            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 5)
            {
                _frame++;
                NPC.frameCounter = 0;
            }
            if (_frame >= 11)
            {
                _frame = 0;
            }
            NPC.frame.Y = frameHeight * _frame;
        }

        public override void Colosseum_AI()
        {
            base.Colosseum_AI();
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            if (!NPC.HasValidTarget)
                NPC.TargetClosest();

            _pauseAnimation = false;
            switch (State)
            {
                case AIState.Chase:
                    AI_Chase();
                    break;
                case AIState.Jump:
                    AI_Jump();
                    break;
                case AIState.Dash:
                    AI_Dash();
                    break;
                case AIState.Dash_Start:
                    AI_DashStart();
                    break;
            }
            NPC.spriteDirection = -NPC.direction;
        }

        private Player Target => Main.player[NPC.target];
        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void AI_Chase()
        {
            Timer++;
            float moveSpeed = 2.5f;
            Vector2 targetVelocity = new Vector2(DirectionToTarget * moveSpeed, 0);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity.X, 0.24f);
            NPC.direction = (int)DirectionToTarget;
            bool jumpWhenBelowPlayer = Target.Bottom.Y < NPC.Top.Y && NPC.collideY;
            bool jumpWhenBouncing = NPC.collideY;
            float xDiff = MathF.Abs(Target.Center.X - NPC.Center.X);
            if (jumpWhenBelowPlayer)
            {
                SwitchState(AIState.Jump);
            }
            else if (jumpWhenBouncing)
            {
                NPC.velocity.Y = -NPC.velocity.Y;
            } 
            else if (xDiff <= 100)
            {
                SwitchState(AIState.Dash_Start);
            }
            TargetOutlineColor = Color.Transparent;

        }
        private void AI_Jump()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.Y -= 12;
            }

            if (Timer > 10 && NPC.collideY)
            {
                SwitchState(AIState.Chase);
            }

            //Failsafe
            if (Timer > 120)
            {
                SwitchState(AIState.Chase);
            }
            TargetOutlineColor = Color.Transparent;
        }
        private void AI_DashStart()
        {
            Timer++;
            NPC.velocity.X *= 0.8f;
            TargetOutlineColor = Color.Yellow;
            _pauseAnimation = true;
            if(Timer >= 100)
            {
                SwitchState(AIState.Dash);
            }
        }

        private void AI_Dash()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.velocity.X = NPC.direction * 2;
            }
            float dashTime = 60f;
            if(Timer < 30)
            {
                NPC.velocity.X *= 1.1f;
            }
            else
            {
                NPC.velocity.X *= 0.8f;
            }

            if(Timer >= dashTime)
            {
                SwitchState(AIState.Chase);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawSprite(spriteBatch, screenPos, drawColor);
            return false;
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            Vector2 drawCenter = NPC.Center - screenPos;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawCenter, NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            DrawExtensions.DrawOutline(DrawSprite, spriteBatch, screenPos, _outlineColor);
        }
    }
}
