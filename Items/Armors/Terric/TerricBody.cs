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

namespace Stellamod.Items.Armors.Terric
{
    [AutoloadEquip(EquipType.Body)]
    public class TerricBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Terric Chestplate");

        }
        public override void SetDefaults()
        {
            Item.Size = new Vector2(18);
            Item.value = Item.sellPrice(silver: 26);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 6;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<TerrorFragments>(), 8);
            recipe.AddIngredient(ItemType<DreadFoil>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}


