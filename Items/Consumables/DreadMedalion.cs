using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.NPCs.Bosses.DreadMire;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    internal class DreadMedalion : ModItem
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
            if (!Main.dayTime)
            {
                if (NPC.AnyNPCs(ModContent.NPCType<DreadMireR>()))
                {
                    return false;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<DreadMireR>()))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.NewText(LangText.Misc("DreadMedalion"), Color.Red);
                        int npcID = NPC.NewNPC(player.GetSource_FromThis(), (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DreadMireR>());
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    else
                    {
                        Main.NewText(LangText.Misc("DreadMedalion"), Color.Red);
                        StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<DreadMireR>(), (int)player.position.X, (int)player.position.Y);
                    }
                }
            }

            return true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreadFragment>(), 3);
            recipe.Register();
        }
    }

}
