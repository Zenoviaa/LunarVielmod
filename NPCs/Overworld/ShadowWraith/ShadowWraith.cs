
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Overworld.ShadowWraith
{

    public class ShadowWraith : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Wraith");
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            Main.npcFrameCount[NPC.type] = 4;
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
            NPC.width = 24;
            NPC.height = 30;
            NPC.damage = 20;
			NPC.defense = 2;
			NPC.lifeMax = 50;
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

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldNightMonster.Chance * 0.5f;
        }

        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 4)
            {
                frame++;
                NPC.frameCounter = 0;
            }

            if (frame >= 4)
            {
                frame = 0;
            }

            NPC.frame.Y = frameHeight * frame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            SpriteEffects Effects = ((base.NPC.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int spOff = NPC.alpha / 20;
            for (float j = -(float)Math.PI; j <= (float)Math.PI / 2f; j += (float)Math.PI / 2f)
            {
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[base.NPC.type], base.NPC.Center + new Vector2(0f, -2f) + new Vector2(4f + NPC.alpha * 0.25f + spOff, 0f).RotatedBy(base.NPC.rotation + j) - Main.screenPosition, base.NPC.frame, Color.FromNonPremultiplied(150 + spOff * 2, 60 + spOff * 2, 150 + spOff * 2, 255 - base.NPC.alpha), base.NPC.rotation, base.NPC.frame.Size() / 2f, base.NPC.scale, Effects, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Pink) * (float)(((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[NPC.type], drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadeHandTome>(), 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadeCharm>(), 50));
         
            if (NPC.downedBoss1)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarkEssence>(), 1, 1, 3));
            }
        }

        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Lighting.AddLight(NPC.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);
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
                    new Vector2(NPC.position.X - screenPos.X + (NPC.width / 2) - GlowTexture.Width * NPC.scale / 2f + halfSize.X * NPC.scale, NPC.position.Y - screenPos.Y + NPC.height - GlowTexture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + halfSize.Y * NPC.scale + Main.NPCAddHeight(NPC) + NPC.gfxOffY),
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
            if (Main.dayTime || player.dead)
            {
                for (int j = 0; j < 10; j++)
                {
                    var EntitySource = NPC.GetSource_Death();

                    int a = Gore.NewGore(EntitySource, NPC.position, NPC.velocity, 99);
                    Main.gore[a].timeLeft = 20;
                    Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
                }
                NPC.active = false;
            }

            NPC.spriteDirection = -NPC.direction;
            NPC.ai[0]++;
            NPC.rotation = NPC.velocity.X * 0.03f;
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 2;
            }

            if (NPC.ai[0] == 500)
            {
                NPC.alpha = 40;
                NPC.ai[0] = 0;
                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
 
                SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                SoundEngine.PlaySound(SoundID.Zombie53, NPC.position);
                float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                int damage = Main.expertMode ? 10 : 14;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, ModContent.ProjectileType<ShadowFlare>(), damage, 1, 
                        Main.myPlayer, 0, 0);
            }

            if (NPC.ai[0] >= 400)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.VenomStaff);
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
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.VenomStaff, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.VenomStaff, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
            }
            else
            {
                for (int k = 0; k < 7; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.VenomStaff, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.VenomStaff, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
                }
            }
        }
    }
}