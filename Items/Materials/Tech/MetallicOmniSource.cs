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

namespace Stellamod.Items.Materials.Tech
{
    public class MetallicOmniSource : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Metallic Omni Source");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = 1;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = 1;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(2);
            recipe.AddIngredient(ItemType<ArncharChunk>(), 4);
            recipe.AddIngredient(ItemType<LostScrap>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }

}
