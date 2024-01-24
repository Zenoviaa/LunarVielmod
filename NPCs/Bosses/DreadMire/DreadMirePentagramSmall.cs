using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.DreadMire
{
    internal class DreadMirePentagramSmall : ModNPC
    {
        public bool Down;
        public float Rot;
        public bool Lightning;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Stalker Lighting");
        }

        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
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

        public override void SetDefaults()
        {
            NPC.aiStyle = 0;
            NPC.alpha = 255;
            NPC.width = 0;
            NPC.height = 0;
            NPC.damage = 8;
            NPC.defense = 8;
            NPC.lifeMax = 156;
            NPC.value = 30f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }

        float alphaCounter = 0;
        float counter = 4;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/Pentagram").Value;
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color((int)(55f * alphaCounter), (int)(15f * alphaCounter), (int)(25f * alphaCounter), 0), NPC.rotation, new Vector2(157, 157), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Rot);
            writer.Write(Lightning);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Rot = reader.ReadSingle();
            Lightning = reader.ReadBoolean();
        }

        public override void AI()
        {
            counter -= 0.09f;
            if (!Down)
            {
                alphaCounter += 0.09f;
                if (alphaCounter >= 5)
                {
                    Down = true;
                }
            }
            else
            {
                if (alphaCounter <= 0)
                {
                    NPC.active = false;
                }
                alphaCounter -= 0.09f;
            }
            if (counter <= 0)
            {
                NPC.active = false;

            }
            if (!Lightning)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Rot = Main.rand.NextFloat(-0.05f, 0.05f);
                    Lightning = true;
                    NPC.rotation = Main.rand.NextFloat(360);
                    NPC.netUpdate = true;
                }
            }
            NPC.rotation -= Rot;
        }
    }
}

