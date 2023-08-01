using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;


namespace Stellamod.Items.Materials.HardMode
{
    public class DuskBars : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dread Foil");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = 1;
        }
        public override void AddRecipes()
        {

            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient(ModContent.ItemType<BlazefuryShard>(), 1);
            recipe.AddIngredient(ModContent.ItemType<StarlightEssence>(), 1);
            recipe.AddIngredient(ItemID.AdamantiteBar, 1);
            recipe.AddIngredient(ItemID.OrichalcumBar, 1);
            recipe.AddIngredient(ItemID.PalladiumBar, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            Recipe recipe2 = CreateRecipe(1);
            recipe2.AddIngredient(ModContent.ItemType<BlazefuryShard>(), 1);
            recipe2.AddIngredient(ModContent.ItemType<StarlightEssence>(), 1);
            recipe2.AddIngredient(ItemID.TitaniumBar, 1);
            recipe2.AddIngredient(ItemID.OrichalcumBar, 1);
            recipe2.AddIngredient(ItemID.PalladiumBar, 1);
            recipe2.AddTile(TileID.MythrilAnvil);
            recipe2.Register();

            Recipe recipe3 = CreateRecipe(1);
            recipe3.AddIngredient(ModContent.ItemType<BlazefuryShard>(), 1);
            recipe3.AddIngredient(ModContent.ItemType<StarlightEssence>(), 1);
            recipe3.AddIngredient(ItemID.AdamantiteBar, 1);
            recipe3.AddIngredient(ItemID.MythrilBar, 1);
            recipe3.AddIngredient(ItemID.PalladiumBar, 1);
            recipe3.AddTile(TileID.MythrilAnvil);
            recipe3.Register();

            Recipe recipe4 = CreateRecipe(1);
            recipe4.AddIngredient(ModContent.ItemType<BlazefuryShard>(), 1);
            recipe4.AddIngredient(ModContent.ItemType<StarlightEssence>(), 1);
            recipe4.AddIngredient(ItemID.AdamantiteBar, 1);
            recipe4.AddIngredient(ItemID.OrichalcumBar, 1);
            recipe4.AddIngredient(ItemID.CobaltBar, 1);
            recipe4.AddTile(TileID.MythrilAnvil);
            recipe4.Register();

            Recipe recipe5 = CreateRecipe(1);
            recipe5.AddIngredient(ModContent.ItemType<BlazefuryShard>(), 1);
            recipe5.AddIngredient(ModContent.ItemType<StarlightEssence>(), 1);
            recipe5.AddIngredient(ItemID.AdamantiteBar, 1);
            recipe5.AddIngredient(ItemID.MythrilBar, 1);
            recipe5.AddIngredient(ItemID.CobaltBar, 1);
            recipe5.AddTile(TileID.MythrilAnvil);
            recipe5.Register();

            Recipe recipe6 = CreateRecipe(1);
            recipe6.AddIngredient(ModContent.ItemType<BlazefuryShard>(), 1);
            recipe6.AddIngredient(ModContent.ItemType<StarlightEssence>(), 1);
            recipe6.AddIngredient(ItemID.TitaniumBar, 1);
            recipe6.AddIngredient(ItemID.OrichalcumBar, 1);
            recipe6.AddIngredient(ItemID.PalladiumBar, 1);
            recipe6.AddTile(TileID.MythrilAnvil);
            recipe6.Register();

            Recipe recipe7 = CreateRecipe(1);
            recipe7.AddIngredient(ModContent.ItemType<BlazefuryShard>(), 1);
            recipe7.AddIngredient(ModContent.ItemType<StarlightEssence>(), 1);
            recipe7.AddIngredient(ItemID.TitaniumBar, 1);
            recipe7.AddIngredient(ItemID.OrichalcumBar, 1);
            recipe7.AddIngredient(ItemID.CobaltBar, 1);
            recipe7.AddTile(TileID.MythrilAnvil);
            recipe7.Register();
        }
    }
}