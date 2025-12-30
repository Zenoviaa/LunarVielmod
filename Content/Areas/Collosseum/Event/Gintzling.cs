using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.Event
{
    public class Gintzling : BaseColosseumNPC,
        IDrawOutlines
    {
        private bool _warn;
        private bool _contactDamage;
        private bool _pauseAnimation;
        private Color _outlineColor;

        private Color TargetOutlineColor;
        private Player Target => Main.player[NPC.target];
        private ref float Timer => ref NPC.ai[0];
        private enum AIState
        {
            Idle,
            Jump,
            JumpWarn
        }
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 58; // The width of the npc's hitbox (in pixels)
            NPC.height = 58; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 30; // The amount of damage that this npc deals
            NPC.defense = 10; // The amount of defense that this npc has
            NPC.lifeMax = 70; // The amount of health that this npc has
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.value = 50f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0.4f;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }

        public override void Colosseum_AI()
        {
            base.Colosseum_AI();
            if (!NPC.HasValidTarget)
                NPC.TargetClosest();

            if (_contactDamage)
                TargetOutlineColor = Color.Red;
            else if (_warn)
                TargetOutlineColor = Color.Yellow;
            else
                TargetOutlineColor = Color.Transparent;
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            _warn = false;
            _contactDamage = false;
            _pauseAnimation = false;
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Jump:
                    AI_Jump();
                    break;
                case AIState.JumpWarn:
                    AI_JumpWarn();
                    break;
            }
            NPC.spriteDirection = -NPC.direction;
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

        private void AI_Idle()
        {

            Timer++;
            NPC.velocity.X *= 0.9f;
            NPC.direction = (Target.Center.X > NPC.Center.X) ? 1 : -1;
            if(Timer >= 100)
            {
                SwitchState(AIState.JumpWarn);
            }
        }

        private void AI_Jump()
        {
            Timer++; _pauseAnimation = true;
            _contactDamage = true;
            if (Timer == 1)
            {
                float jumpX = (MathF.Abs(Target.Center.X - NPC.Center.X) / 32f);
                float maxX = 9;
                float x = MathF.Min(jumpX, maxX);
                NPC.velocity.X = NPC.direction * x;

                int jumpHeight = (int)(MathF.Abs(Target.Center.Y - NPC.Center.Y) / 16f) + 4;
                if (Target.Center.Y > NPC.Center.Y)
                    jumpHeight = 3;
                NPC.velocity.Y -= jumpHeight;
            }

            if (Timer > 10 && NPC.collideY)
            {
                SwitchState(AIState.Idle);
            }

            //Failsafe
            if (Timer > 120)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_JumpWarn()
        {
            _warn = true;
            Timer++;
            if(Timer >= 60)
            {
                SwitchState(AIState.Jump);
            }
        }


        public override bool? CanFallThroughPlatforms()
        {
            return Target.Bottom.Y - 16 > NPC.Bottom.Y;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Lowest of the Gintze but can wipe you out fast! They need food too yknow :("))
            });
        }

        private int _frame = 0;
        public override void FindFrame(int frameHeight)
        {
            if (_pauseAnimation)
           {
                _frame = 0;
            }

            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 4)
            {
                _frame++;
                NPC.frameCounter = 0;
            }
            if (_frame >= 8)
            {
                _frame = 0;
            }
            NPC.frame.Y = frameHeight * _frame;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            GintzeHitEffect(hit);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Morrowpes"), NPC.position);
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
                spriteBatch.Draw(texture, drawCenter, NPC.frame, afterImageColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
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
    }
}