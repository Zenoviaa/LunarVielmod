using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Content.Areas.Collosseum.Event.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.Event
{
    public class GintzeSpearman : BaseColosseumNPC,
        IDrawOutlines
    {
        private enum AIState
        {
            Pace,
            Spear_Throw
        }


        private int _frame = 0;
        private ref float Timer => ref NPC.ai[0];

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float RandFactor => ref NPC.ai[2];
        private Player Target => Main.player[NPC.target];
        private float SightLineProgress;
        private Vector2 FireVelocity;
        private Color _outlineColor;
        private float FleeDistance => 128;

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

        public override bool CanHitNPC(NPC target)
        {
            return false;
        }

        //No contact damage, I'm sorry brah
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 150;
            NPC.damage = 44;
            NPC.defense = 9;
            NPC.value = 65f;
            NPC.width = 30;
            NPC.height = 50;
            NPC.knockBackResist = 0.55f;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.noGravity = false;
            NPC.noTileCollide = false;
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

        public override void Colosseum_AI()
        {
            base.Colosseum_AI();
            if (MultiplayerHelper.IsHost)
            {
                if(RandFactor == 0)
                {
                    RandFactor = Main.rand.NextFloat(0f, 60f);
                    NPC.netUpdate = true;
                }
            }
            if (!NPC.HasValidTarget)
                NPC.TargetClosest();

            NPC.direction = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = -NPC.direction;
            switch (State)
            {
                case AIState.Pace:
                    AI_Pace();
                    break;
                case AIState.Spear_Throw:
                    AI_SpearThrow();
                    break;
            }
        }
        private void AI_Pace()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            float moveSpeed = 1.3f;
            Vector2 targetVelocity = new Vector2(-DirectionToTarget * moveSpeed, 0);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity.X, 0.3f);
            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            float targetSineLineProgress = 0f;
            SightLineProgress = MathHelper.Lerp(SightLineProgress, targetSineLineProgress, 0.1f);
            if (distanceToTarget > FleeDistance && Timer > 60 + RandFactor)
            {
                if (MultiplayerHelper.IsHost)
                {
                    RandFactor = Main.rand.NextFloat(0f, 60f);
                    NPC.netUpdate = true;
                }
                SwitchState(AIState.Spear_Throw);
            }
        }

        private void AI_SpearThrow()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0, 0.1f);
            float targetSineLineProgress = Timer / 120f;
            SightLineProgress = MathHelper.Lerp(SightLineProgress, targetSineLineProgress, 0.1f);
            FireVelocity = Vector2.Lerp(FireVelocity, (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero), 0.1f);

            if (Timer > 60)
            {
                _outlineColor = Color.Lerp(Color.Transparent, Color.Yellow, (Timer - 60f) / 60f);
            }
            else
            {
                _outlineColor *= 0.5f;
            }
            if (Timer >= 120)
            {
                if (MultiplayerHelper.IsHost)
                {
                    int damage = 17;
                    int knockback = 2;
                    Vector2 spawnPoint = NPC.Center;
                    spawnPoint.Y -= 48;
                    Vector2 velocity = FireVelocity * 7f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, velocity,
                        ModContent.ProjectileType<CaptainSpear>(), damage, knockback, Main.myPlayer);
                }
                Timer = 0;
            }

            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            if (distanceToTarget < FleeDistance)
            {
                SwitchState(AIState.Pace);
            }
        }

        private Asset<Texture2D> _sightLineTextureAsset;
        private void DrawSightLine(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            _sightLineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_SightLine");
            Texture2D texture = _sightLineTextureAsset.Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = new Vector2(0, texture.Height / 2);
            float drawRotation = FireVelocity.ToRotation();
            float drawScale = MathHelper.Lerp(1f, 3.3f, SightLineProgress);
            Color lineDrawColor = Color.Lerp(Color.Transparent, Color.White, SightLineProgress);
            lineDrawColor.A = 0;
            spriteBatch.Draw(texture, drawPos, null, lineDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawSightLine(spriteBatch, screenPos, drawColor);
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            return false;
        }


        public override void HitEffect(NPC.HitInfo hit)
        {
            GintzeHitEffect(hit);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.1f;
            if (NPC.frameCounter >= 1)
            {
                _frame++;
                NPC.frameCounter = 0;
            }

            switch (State)
            {
                case AIState.Pace:
                    if (_frame >= 4)
                    {
                        _frame = 0;
                    }
                    break;
                case AIState.Spear_Throw:
                    if (_frame >= 11)
                    {
                        _frame = 0;
                    }
                    break;
            }

            NPC.frame.Y = frameHeight * _frame;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float outlineOffset = 2;
            Vector2 left = drawPos + Vector2.UnitX * -outlineOffset;
            Vector2 right = drawPos + Vector2.UnitX * outlineOffset;
            Vector2 up = drawPos + Vector2.UnitY * -outlineOffset;
            Vector2 down = drawPos + Vector2.UnitY * outlineOffset;
            Color outlineColor = _outlineColor;

            spriteBatch.Draw(texture, left, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, right, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, up, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, down, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        }
    }
}
