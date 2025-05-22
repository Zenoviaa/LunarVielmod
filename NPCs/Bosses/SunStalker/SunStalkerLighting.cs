using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.SunStalker
{
    internal class SunStalkerLighting : ModNPC
    {
        public bool Lightning;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Stalker Lighting");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
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

        public override void SetDefaults()
        {
            NPC.alpha = 2;
            NPC.width = 24;
            NPC.height = 24;
            NPC.damage = 8;
            NPC.defense = 8;
            NPC.lifeMax = 156;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
  
        }
        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            if (!Lightning)
            {
                NPC.scale = Main.rand.NextFloat(0.9f, 1.21f);
                Lightning = true;
                NPC.rotation = Main.rand.NextFloat(360);
            }

            bool expertMode = Main.expertMode;
            Player player = Main.player[NPC.target];
            NPC.frameCounter++;
            if (frame == 3)
            {
                NPC.active = false;

            }
            if (NPC.frameCounter >= 5)
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
            if (Main.rand.NextBool(2))
            {
                int dustnumber = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
            Lighting.AddLight(NPC.Center, Color.Yellow.ToVector3() * 2.25f * Main.essScale);
            int spOff = NPC.alpha / 6;
            SpriteEffects Effects = ((base.NPC.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            for (float j = -(float)Math.PI; j <= (float)Math.PI / 2f; j += (float)Math.PI / 2f)
            {
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[base.NPC.type], base.NPC.Center + new Vector2(0f, -2f) + new Vector2(4f + NPC.alpha * 0.25f + spOff, 0f).RotatedBy(base.NPC.rotation + j) - Main.screenPosition, base.NPC.frame, Color.FromNonPremultiplied(255 + spOff * 2, 255 + spOff * 2, 255 + spOff * 2, 100 - base.NPC.alpha), base.NPC.rotation, base.NPC.frame.Size() / 2f, base.NPC.scale, Effects, 0f);
            }
            spriteBatch.Draw((Texture2D)TextureAssets.Npc[base.NPC.type], base.NPC.Center - Main.screenPosition, base.NPC.frame, base.NPC.GetAlpha(lightColor * 0.5f), base.NPC.rotation, base.NPC.frame.Size() / 2f, base.NPC.scale, Effects, 0f);

            return true;
        }
        public override void AI()
        {


        }
    }
}

