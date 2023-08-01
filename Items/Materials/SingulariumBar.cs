
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Stellamod.Tiles;

namespace Stellamod.Items.Materials
{
    internal class SingulariumBar : ModItem
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
            Item.maxStack = 999; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 1);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 4);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }

    }
}