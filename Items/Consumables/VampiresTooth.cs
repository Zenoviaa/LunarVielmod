using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.NPCs.Bosses.Veiizal;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    internal class VampiresTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.consumable = false;
            Item.rare = ItemRarityID.Green;
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
            DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Red);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
            return true;
        }



        public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
        {
            if (!Main.dayTime)
            {
                if (NPC.AnyNPCs(ModContent.NPCType<Veiizal>()))
                {
                    return false;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<Veiizal>()))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.NewText(LangText.Misc("Veiizal"), Color.Red);
                        int npcID = NPC.NewNPC(player.GetSource_FromThis(), (int)player.position.X, (int)player.position.Y - 500, ModContent.NPCType<Veiizal>());
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    else
                    {
                        Main.NewText(LangText.Misc("Veiizal"), Color.Red);
                        StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<Veiizal>(), (int)player.position.X, (int)player.position.Y - 500);
                    }
                }
            }

            return true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreadFoil>(), 15);
            recipe.Register();
        }
    }


}
