
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.DropRules;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Bosses.SupernovaFragment;
using Stellamod.NPCs.Overworld.ShadowWraith;
using Stellamod.Utilis;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Abyssal
{

    public class AbysmLunatic : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Wraith");
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            Main.npcFrameCount[NPC.type] = 10;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Powerful skulls sent by skeletron to roam the darkness for the next pray"))
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 90;
            NPC.damage = 8;
            NPC.defense = 2;
            NPC.lifeMax = 400;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.ShadowFlame] = true;
            NPC.alpha = 60;
            NPC.knockBackResist = .75f;
            NPC.aiStyle = 14;
            NPC.alpha = 0;
            NPC.noGravity = true;
        }

        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 3)
            {
                frame++;
                NPC.frameCounter = 0;
            }

            if (frame >= 10)
            {
                frame = 0;
            }

            NPC.frame.Y = frameHeight * frame;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust && !Main.pumpkinMoon && !Main.snowMoon) && Main.hardMode)
            {
                return spawnInfo.Player.ZoneAbyss() ? 1.5f : 0f;
            }
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EldritchSoul>(), minimumDropped: 0, maximumDropped: 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ConvulgingMater>(), minimumDropped: 1, maximumDropped: 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LunarBand>(), 30));

            LeadingConditionRule hardmodeDropRule = new LeadingConditionRule(new HardmodeDropRule());
            hardmodeDropRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<EldritchSoul>(), minimumDropped: 1, maximumDropped: 5));
            npcLoot.Add(hardmodeDropRule);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int spOff = NPC.alpha / 20;
            for (float j = -(float)Math.PI; j <= (float)Math.PI / 2f; j += (float)Math.PI / 2f)
            {
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[NPC.type], NPC.Center + new Vector2(0f, -2f) + new Vector2(4f + NPC.alpha * 0.25f + spOff, 0f).RotatedBy(NPC.rotation + j) - Main.screenPosition, NPC.frame, Color.FromNonPremultiplied(50 + spOff * 2, 75 + spOff * 2, 255 + spOff * 2, 255 - NPC.alpha), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, Effects, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(50, 74, 255), new Color(49, 39, 124), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[NPC.type], drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Lighting.AddLight(NPC.Center, Color.LightBlue.ToVector3() * 1.75f * Main.essScale);
            if (GlowTexture is not null)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                Vector2 halfSize = new Vector2(GlowTexture.Width / 2, GlowTexture.Height / Main.npcFrameCount[NPC.type] / 2);
                spriteBatch.Draw(
                    GlowTexture,
                    new Vector2(NPC.position.X - screenPos.X + NPC.width / 2 - GlowTexture.Width * NPC.scale / 2f + halfSize.X * NPC.scale, NPC.position.Y - screenPos.Y + NPC.height - GlowTexture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + halfSize.Y * NPC.scale + Main.NPCAddHeight(NPC) + NPC.gfxOffY),
                    NPC.frame,
                    Color.White,
                    NPC.rotation,
                    halfSize,
                    NPC.scale,
                    spriteEffects,
                0);
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];


            NPC.spriteDirection = -NPC.direction;
            NPC.ai[0]++;
            NPC.rotation = NPC.velocity.X * 0.03f;
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 2;
            }

            if (NPC.ai[0] == 400)
            {
                NPC.alpha = 40;
                NPC.ai[0] = 0;
                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

                SoundEngine.PlaySound(SoundID.Item104, NPC.position);
                SoundEngine.PlaySound(SoundID.Zombie83, NPC.position);
                float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                int damage = Main.expertMode ? 40 : 43;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, ModContent.ProjectileType<VoidFlame>(), damage, 1, Main.myPlayer, 0, 0);
            }
            if (NPC.ai[0] == 300)
            {
                SoundEngine.PlaySound(SoundID.Zombie82, NPC.position);
                SoundEngine.PlaySound(SoundID.Zombie99, NPC.position);

                NPC.velocity *= 0.92f;
            }
            if (NPC.ai[0] >= 300)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.UnusedWhiteBluePurple);
                    dust.velocity *= -1f;
                    dust.scale *= .8f;
                    dust.noGravity = true;
                    Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                    vector2_1.Normalize();
                    Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                    dust.velocity = vector2_2;
                    vector2_2.Normalize();
                    Vector2 vector2_3 = vector2_2 * 34f;
                    dust.position = NPC.Center - vector2_3;
                    NPC.netUpdate = true;
                }

                NPC.velocity *= 0.92f;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                //var EntitySource = NPC.GetSource_Death();
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UnusedWhiteBluePurple, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UnusedWhiteBluePurple, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
            }
            else
            {
                for (int k = 0; k < 7; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UnusedWhiteBluePurple, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UnusedWhiteBluePurple, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                }
            }
        }
    }
}