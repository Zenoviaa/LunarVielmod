using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.Terric
{
    internal class TericGramNPC : ModNPC
    {
        public bool Down;
        public float Rot;
        public bool Lightning;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Stalker Lighting");
        }

        public override void SetDefaults()
        {
            NPC.alpha = 255;
            NPC.width = 0;
            NPC.height = 0;
            NPC.damage = 0;
            NPC.defense = 8;
            NPC.lifeMax = 156;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.friendly = true;
            NPC.aiStyle = 0;
        }
        float alphaCounter = 0;
        float counter = 2;


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {

            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/Pentagram").Value;
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color((int)(55f * alphaCounter), (int)(15f * alphaCounter), (int)(25f * alphaCounter), 0), NPC.rotation, new Vector2(157, 157), 0.2f * (counter + 0.05f), SpriteEffects.None, 0f);
            return true;
        }
        public override void AI()
        {

            Player player = Main.player[NPC.target];
            if (player.GetModPlayer<MyPlayer>().TericGramLevel == 0)
            {
                NPC.active = false;
            }
            NPC.Center = player.Center;
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

            }

            if (!Lightning)
            {
                Rot = Main.rand.NextFloat(-0.05f, 0.05f);
                Lightning = true;
                NPC.rotation = Main.rand.NextFloat(360);
            }
            NPC.rotation -= Rot;
        }
    }
}

