using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Materials.Tech;
using Stellamod.NPCs.Bosses.Zui;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    internal class RadianceStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.consumable = false;
            Item.rare = ItemRarityID.LightRed;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: 10);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 10;
            Item.useTime = 10;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Gold);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            //The below code makes this item hover up and down in the world
            //Don't forget to make the item have no gravity, otherwise there will be weird side effects
            float hoverSpeed = 5;
            float hoverRange = 0.2f;
            float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
            Vector2 position = new Vector2(Item.position.X, Item.position.Y + y);
            Item.position = position;
        }


        public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
        {



            if (!player.GetModPlayer<MyPlayer>().ZoneVillage)
            {
                return false;
            }

            else if (player.GetModPlayer<MyPlayer>().ZoneVillage)
            {

                if (NPC.AnyNPCs(ModContent.NPCType<ZuiTheTraveller>()))
                {
                    return false;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<ZuiTheTraveller>()))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.NewText(LangText.Misc("RadianceStone.1"), Color.Gold);
                        int npcID = NPC.NewNPC(player.GetSource_FromThis(), (int)player.position.X, (int)player.position.Y, ModContent.NPCType<ZuiTheTraveller>());
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    else
                    {
                        Main.NewText(LangText.Misc("RadianceStone.1"), Color.Gold);
                        StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<ZuiTheTraveller>(), (int)player.position.X, (int)player.position.Y);
                    }

                    int TextToSpawn = Main.rand.Next(1, 8 + 1);
                    if (TextToSpawn == 1)
                    {
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(252, 208, 113, 44), LangText.Misc("RadianceStone.2"));
                    }
                    if (TextToSpawn == 2)
                    {
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(252, 208, 113, 44), LangText.Misc("RadianceStone.3"));
                    }
                    if (TextToSpawn == 3)
                    {
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(252, 208, 113, 44), LangText.Misc("RadianceStone.4"));
                    }
                    if (TextToSpawn == 4)
                    {
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(252, 208, 113, 44), LangText.Misc("RadianceStone.5"));
                    }
                    if (TextToSpawn == 5)
                    {
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(252, 208, 113, 44), LangText.Misc("RadianceStone.6"));
                    }
                    if (TextToSpawn == 6)
                    {
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(252, 208, 113, 44), LangText.Misc("RadianceStone.7"));
                    }
                    if (TextToSpawn == 7)
                    {
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(252, 208, 113, 44), LangText.Misc("RadianceStone.8"));
                    }
                    if (TextToSpawn == 8)
                    {
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(252, 208, 113, 44), LangText.Misc("RadianceStone.9"));
                    }

                }
                else
                {

                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(252, 208, 113, 44), LangText.Misc("RadianceStone.10"));
                }

            }



            


            return true;
        }

    }


}

