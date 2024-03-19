
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Utilis;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Acidic
{
    public class AcidSpirit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Wraith");
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 25;
            NPC.height = 20;
            NPC.damage = 20;
            NPC.defense = 2;
            NPC.lifeMax = 120;
            NPC.HitSound = SoundID.NPCHit30;
            NPC.DeathSound = SoundID.NPCDeath38;
            NPC.value = 30f;
            NPC.alpha = 60;
            NPC.knockBackResist = .75f;
            NPC.aiStyle = 14;
            NPC.alpha = 120;
            NPC.noGravity = true;
        }

        private int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 4)
            {
                frame++;
                NPC.frameCounter = 0;
            }
            if (frame >= 3)
            {
                frame = 0;
            }

            NPC.frame.Y = frameHeight * frame;
        }

        private float Size;
        private bool CheckSize;
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust && !Main.pumpkinMoon && !Main.snowMoon))
            {
                return spawnInfo.Player.ZoneAcid() ? 0.8f : 0f;
            }
            return 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X + 5, NPC.height - NPC.frame.Height + 3);
            Vector2 DrawPos = NPC.position - screenPos + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, 3).RotatedBy(radians) * time, NPC.frame, new Color(53, 107, 112, 0), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, 5).RotatedBy(radians) * time, NPC.frame, new Color(152, 208, 113, 0), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }


            Lighting.AddLight(NPC.Center, Color.GreenYellow.ToVector3() * 2.25f * Main.essScale);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(152, 208, 113), new Color(53, 107, 112), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            if (Main.hardMode)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GraftedSoul>(), 2, 1, 5));
            }
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VirulentPlating>(), minimumDropped: 1, maximumDropped: 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AcidStaketers>(), chanceDenominator: 30));
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
            if (!CheckSize)
            {
                Size = Main.rand.NextFloat(0.75f, 1f);
                CheckSize = true;
            }

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
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 
                    ModContent.DustType<Dusts.GlowDust>(), newColor: new Color(24, 142, 61));
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 
                    ModContent.DustType<Dusts.GunFlash>(), newColor: new Color(24, 142, 61));
                Main.dust[d].rotation = (Main.dust[d].position - NPC.position).ToRotation() - MathHelper.PiOver4;
            }

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != NPC.Center)
                    {
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                    }
                }
            }
        }
    }
}