using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.NPCs.Bosses.DaedusRework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.Caeva
{
    internal class CaevaDeathRow : ModNPC
    {
        public bool Down;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Stalker Lighting");
        }

        public override void SetDefaults()
        {
            NPC.alpha = 255;
            NPC.width = 0;
            NPC.height = 0;
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

        private float alphaCounter = 0;
        private float counter = 8;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/NPCs/Bosses/Caeva/CaevaDeathRow").Value;
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color((int)(55f * alphaCounter), (int)(05f * alphaCounter), (int)(55f * alphaCounter), 45f), -NPC.rotation, new Vector2(1200, 88), 0.25f * (counter + 0.3f), SpriteEffects.None, 0f);
            return true;
        }
        public override void AI()
        {
            NPC.damage = 0;
            NPC.ai[0]++;

            if (!Down)
            {
                alphaCounter += 0.05f;
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
                alphaCounter -= 0.29f;
            }
        }
    }
}

