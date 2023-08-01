using Terraria;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Stellamod.Items.Materials;



namespace Stellamod.Items.Armors.Leather
{
    [AutoloadEquip(EquipType.Body)]
    public class LeatherBody : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Leather Vest");
            // Tooltip.SetDefault("Increases throwing damage by 25%");
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
            Item.vanity = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<TerrorFragments>(), 8);
            recipe.AddIngredient(ItemID.Leather, 10);
            recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe.AddIngredient(ItemType<TerrorFragments>(), 8);
            recipe2.AddIngredient(ItemID.Leather, 10);
            recipe2.AddIngredient(ItemID.LeadBar, 5);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }

    }
}
