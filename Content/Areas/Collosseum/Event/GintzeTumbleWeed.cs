using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Items.Accessories.Foods;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.Event
{
    public class GintzeTumbleWeed : BaseColosseumNPC,
        IDrawOutlines
    {
        private int _frame;
        private enum AIState
        {
            Chase,
            Jump
        }
        private Color _outlineColor;
        private ref float Timer => ref NPC.ai[0];

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float BuiltUpSpeed => ref NPC.ai[2];
        private Player Target => Main.player[NPC.target];
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
            base.SetStaticDefaults();
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 250;
            NPC.damage = 37;
            NPC.defense = 8;
            NPC.value = 65f;
            NPC.width = 32;
            NPC.height = 32;
            NPC.knockBackResist = 0.55f;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return Target.Bottom.Y - 16 > NPC.Bottom.Y;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && State == AIState.Chase;
        }

        public override void Colosseum_AI()
        {
            base.Colosseum_AI();
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
            switch (State)
            {
                case AIState.Chase:
                    AI_Chase();
                    break;
                case AIState.Jump:
                    AI_Jump();
                    break;
            }

            NPC.rotation += 0.05f;
            NPC.rotation += NPC.velocity.X * 0.1f;
        }

        private void AI_Chase()
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Red, 0.1f);

            Timer++;
            float moveSpeed = BuiltUpSpeed;
            Vector2 targetVelocity = new Vector2(DirectionToTarget * moveSpeed, 0);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity.X, 0.02f);
            if (NPC.collideY)
            {
                if(BuiltUpSpeed < 5)
                    BuiltUpSpeed += 0.1f;
            }
            bool jumpWhenBelowPlayer = Target.Bottom.Y < NPC.Top.Y && NPC.collideY;
            bool jumpWhenBouncing = NPC.collideY;
            if (jumpWhenBelowPlayer)
            {
                SwitchState(AIState.Jump);
            }
            if (jumpWhenBouncing)
            {
                NPC.velocity.Y = -NPC.velocity.Y;
            }
        }

        private void AI_Jump()
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Yellow, 0.1f);
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.Y -= 12;
            }
            if(BuiltUpSpeed > 0)
                BuiltUpSpeed -= 0.1f;
            if (Timer > 10 && NPC.collideY)
            {
                SwitchState(AIState.Chase);
            }

            //Failsafe
            if (Timer > 120)
            {
                SwitchState(AIState.Chase);
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

        public override void HitEffect(NPC.HitInfo hit)
        {
            GintzeHitEffect(hit);
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
            spriteBatch.Draw(texture, drawCenter, NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            DrawExtensions.DrawOutline(DrawSprite, spriteBatch, screenPos, _outlineColor);
        }
    }
}
