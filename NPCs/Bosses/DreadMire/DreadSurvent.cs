using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DreadMire
{

    public class DreadSurvent : ModNPC
    {
        bool Spawn;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
                new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Powerful skulls tainted by devilish intent (dreadmire must�ve got bored with this one)")),
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 25;
            NPC.height = 20;
            NPC.damage = 8;
            NPC.defense = 0;
            NPC.lifeMax = 20;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.ShadowFlame] = true;
            NPC.alpha = 60;
            NPC.knockBackResist = .75f;
            NPC.aiStyle = 14;
            NPC.alpha = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
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
            if (frame >= 3)
            {
                frame = 0;
            }
            NPC.frame.Y = frameHeight * frame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            int spOff = NPC.alpha / 20;

            for (float j = -(float)Math.PI; j <= (float)Math.PI / 2f; j += (float)Math.PI / 2f)
            {
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[NPC.type], NPC.Center + new Vector2(0f, -2f) + new Vector2(4f + NPC.alpha * 0.25f + spOff, 0f).RotatedBy(NPC.rotation + j) - Main.screenPosition, NPC.frame, Color.FromNonPremultiplied(150 + spOff * 2, 60 + spOff * 2, 150 + spOff * 2, 255 - NPC.alpha), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, Effects, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(255, 8, 55), new Color(99, 39, 51), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
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
            NPC.ai[2]++;
            NPC.spriteDirection = -NPC.direction;
            NPC.ai[0]++;
            NPC.rotation = NPC.velocity.X * 0.03f;
            if (NPC.ai[2] == 1 && !Spawn)
            {
                Spawn = true;
  
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagramSmall>());
                }    
            }
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 2;
            }
            if (Main.rand.NextBool(25))
            {
                int dustnumber = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RuneDust>(), 0f, 0f, 150, Color.Red, 1);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                var EntitySource = NPC.GetSource_Death();

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
                }
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RuneDust>(), 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RuneDust>(), 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);

            }
            else
            {
                for (int k = 0; k < 7; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RuneDust>(), 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RuneDust>(), 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                }
            }
        }
    }
}