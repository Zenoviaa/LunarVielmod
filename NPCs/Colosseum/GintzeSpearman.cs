using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Foods;
using Stellamod.Items.Ores;
using Stellamod.NPCs.Colosseum.Common;
using Stellamod.NPCs.Colosseum.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum
{
    public class GintzeSpearman : BaseColosseumNPC
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

        private Player Target => Main.player[NPC.target];
        private float SightLineProgress;
        private Vector2 FireVelocity;
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
            // DisplayName.SetDefault("Storm Knight");
            Main.npcFrameCount[NPC.type] = 11;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GintzlMetal>(), 1, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10, 1, 3));
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 150;
            NPC.damage = 24;
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

        public override void AI()
        {
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
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

        private void SwitchState(AIState state)
        {
            if (StellaMultiplayer.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void AI_Pace()
        {
            Timer++;
            float moveSpeed = 1;
            Vector2 targetVelocity = new Vector2(-DirectionToTarget * moveSpeed, 0);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity.X, 0.3f);
            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            float targetSineLineProgress = 0f;
            SightLineProgress = MathHelper.Lerp(SightLineProgress, targetSineLineProgress, 0.1f);
            if (distanceToTarget > FleeDistance)
            {
                SwitchState(AIState.Spear_Throw);
            }
        }

        private void AI_SpearThrow()
        {
            Timer++;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0, 0.1f);
            float targetSineLineProgress = Timer / 120f;
            SightLineProgress = MathHelper.Lerp(SightLineProgress, targetSineLineProgress, 0.1f);
            FireVelocity = Vector2.Lerp(FireVelocity, (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero), 0.1f);
            if(Timer >= 120)
            {
                if (StellaMultiplayer.IsHost)
                {
                    int damage = 12;
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

        private void DrawSightLine(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_SightLine").Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = new Vector2(0, texture.Height / 2);
            float drawRotation = FireVelocity.ToRotation();
            float drawScale = 1f;
            Color lineDrawColor = Color.Lerp(Color.Transparent, Color.White, SightLineProgress);
            spriteBatch.Restart(blendState: BlendState.Additive);
            spriteBatch.Draw(texture, drawPos, null, lineDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            spriteBatch.RestartDefaults();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawSightLine(spriteBatch, screenPos, drawColor);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.LightBlue * num107 * .8f;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(
                GlowTexture,
                NPC.Center - Main.screenPosition + Drawoffset,
                NPC.frame,
                color1,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                effects,
                0
            );
            SpriteEffects spriteEffects3 = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.White);
            for (int num103 = 0; num103 < 4; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 4; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 180, default, .6f);
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Copper, 0f, -2f, 180, default, .6f);
                    Main.dust[num].noGravity = true;
                    Dust expr_62_cp_0 = Main.dust[num];
                    expr_62_cp_0.position.X = expr_62_cp_0.position.X + (Main.rand.Next(-50, 51) / 20 - 1.5f);
                    Dust expr_92_cp_0 = Main.dust[num];
                    expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + (Main.rand.Next(-50, 51) / 20 - 1.5f);
                    if (Main.dust[num].position != NPC.Center)
                    {
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                    }
                }
            }
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
                    if(_frame >= 4)
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
    }
}
