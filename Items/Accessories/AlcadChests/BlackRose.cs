using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.AlcadChests
{
    internal class BlackRose : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 36;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.HasBuff(BuffID.ManaSickness))
            {
                int combatText = CombatText.NewText(player.getRect(), Color.Red, "10", true);
                CombatText numText = Main.combatText[combatText];
                numText.lifeTime = 60;
             
                player.ClearBuff(BuffID.ManaSickness);
                player.statLife -= 10;
            }
        }
    }

    internal class BlackManaRose : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 36;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaFlower = true;
            player.manaCost -= 0.08f;
            if (player.HasBuff(BuffID.ManaSickness))
            {
                int combatText = CombatText.NewText(player.getRect(), Color.Red, "10", true);
                CombatText numText = Main.combatText[combatText];
                numText.lifeTime = 60;

                player.ClearBuff(BuffID.ManaSickness);
                player.statLife -= 10;
            }
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.ManaFlower);
            recipe.AddIngredient(ModContent.ItemType<BlackRose>());
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}
