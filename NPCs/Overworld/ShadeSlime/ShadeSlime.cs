
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Overworld.ShadeSlime
{

    public class ShadeSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shrowded Slime");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.TrailCacheLength[NPC.type] = 20;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A slime formed of pure darkness conjured by powerful beasts"))
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!NPC.downedBoss1)
                return 0f;
            return SpawnCondition.OverworldNightMonster.Chance * 0.3f;
        }

        public override void SetDefaults()
        {
            NPC.width = 47;
            NPC.height = 35;
            NPC.damage = 10;
            NPC.defense = 5;
            NPC.lifeMax = 45;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath24;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 1;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
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
            Color color1 = Color.Pink * num107 * .8f;
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

            SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Pink);
            for (int num103 = 0; num103 < 4; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Blackout, 180);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, minimumDropped: 0, maximumDropped: 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarkEssence>(), minimumDropped: 1, maximumDropped: 3));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            Lighting.AddLight(NPC.Center, Color.Purple.ToVector3() * 0.25f * Main.essScale);
            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;



            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X + 0, NPC.height - NPC.frame.Height + 5);
            Vector2 drawPos = NPC.position - screenPos + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, NPC.frame, new Color(75, 70, 128, 0), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, NPC.frame, new Color(255, 87, 230, 0), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 vector2_3 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width * 0.3f, (NPC.height / Main.npcFrameCount[NPC.type]) * 2f);
            if (NPC.velocity != Vector2.Zero)
            {
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 DrawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY);
                    Color color = NPC.GetAlpha(Color.Lerp(new Color(110, 50, 110), new Color(80, 40, 80), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, DrawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
                }
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            int d = DustID.Shadowflame;
            for (int k = 0; k < 6; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
            }
            if (NPC.life <= 0)
            {
                for (int j = 0; j < 10; j++)
                {
                    var EntitySource = NPC.GetSource_Death();
                    int a = Gore.NewGore(EntitySource, new Vector2(NPC.Center.X + Main.rand.Next(-10, 10), NPC.Center.Y + Main.rand.Next(-10, 10)), NPC.velocity, 99);
                    Main.gore[a].timeLeft = 20;
                    Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
                }
            }
        }
        public override void AI()
        {
            if (Main.dayTime)
            {
                for (int j = 0; j < 10; j++)
                {
                    var EntitySource = NPC.GetSource_Death();
                    int a = Gore.NewGore(EntitySource, new Vector2(NPC.Center.X + Main.rand.Next(-10, 10), NPC.Center.Y + Main.rand.Next(-10, 10)), NPC.velocity, 99);
                    Main.gore[a].timeLeft = 20;
                    Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
                }
                NPC.active = false;
            }
        }

       
    }
}