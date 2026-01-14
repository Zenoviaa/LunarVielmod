using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Foods;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.Event
{
    public class GintzeSoldier : BaseColosseumNPC,
        IDrawOutlines
    {
        private bool _warn;
        private bool _contactDamage;
        private Color _outlineColor;

        private Color TargetOutlineColor;

        private int _frame;
        private enum AIState
        {
            Chase,
            Jump
        }
        private ref float Timer => ref NPC.ai[0];

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float RandFactor => ref NPC.ai[2];
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
            Main.npcFrameCount[NPC.type] = 11;
        }

        public override void SetDefaults()
        {
            NPC.width = 50; // The width of the npc's hitbox (in pixels)
            NPC.height = 52; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 37; // The amount of damage that this npc deals
            NPC.defense = 0; // The amount of defense that this npc has
            NPC.lifeMax = 180; // The amount of health that this npc has
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.value = 5f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0.2f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.25f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }
            if (_frame >= Main.npcFrameCount[Type])
            {
                _frame = 0;
            }
            switch (State)
            {
                case AIState.Jump:
                    _frame = 6;
                    break;
            }
            NPC.frame.Y = frameHeight * _frame;
        }

        public override bool CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target) && _contactDamage;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return Target.Bottom.Y - 16 > NPC.Bottom.Y;
        }

        public override void Colosseum_AI()
        {
            base.Colosseum_AI();
            if(!NPC.HasValidTarget)
                NPC.TargetClosest();
            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = -NPC.direction;
            if (_contactDamage)
                TargetOutlineColor = Color.Red;
            else if (_warn)
                TargetOutlineColor = Color.Yellow;
            else
                TargetOutlineColor = Color.Transparent;
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            _warn = false;
            _contactDamage = false;
            switch (State)
            {
                case AIState.Chase:
                    AI_Chase();
                    break;
                case AIState.Jump:
                    AI_Jump();
                    break;
            }
        }

        private void AI_Chase()
        {
            _contactDamage = true;
            Timer++;
            float moveSpeed = 2;
            Vector2 targetVelocity = new Vector2(DirectionToTarget * moveSpeed, 0);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity.X, 0.3f);
            if (Target.Top.Y < NPC.Top.Y && NPC.collideY)
            {
                SwitchState(AIState.Jump);
            }
        }

        private void AI_Jump()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.Y -= 10;
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
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = TextureAssets.Npc[Type].Value;
            //Draw after images
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                float progressOnTrail = (float)i / (float)NPC.oldPos.Length;
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 drawCenter = oldPos + NPC.Size / 2f - screenPos;
                Vector2 drawOrigin = NPC.frame.Size() / 2f;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, progressOnTrail);
                afterImageColor *= 0.5f;
                spriteBatch.Draw(texture, drawCenter, NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            }
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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A Captain of Gofria's ranks, be careful"))
            });
        }
    }
}