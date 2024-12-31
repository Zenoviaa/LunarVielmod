
using Stellamod.Items.Harvesting;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    internal class ArnchaliteBar : ModItem
    {

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's heighta
            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(2);
            recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 2);
            recipe.AddIngredient(ModContent.ItemType<ArncharChunk>(), 1);
            recipe.AddIngredient(ModContent.ItemType<WanderingFlame>(), 1);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
}