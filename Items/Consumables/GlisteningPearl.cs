using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    public class GlisteningPearl : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stalker's Stone");
            // Tooltip.SetDefault("Use this as a sun altar to unleash the solar power…"); // The (English) text shown below your item's name
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of Corely used research amounts depending on item type.
        }

        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<GlisteningOre>(), 5);
            recipe.Register();
        }

    }
}