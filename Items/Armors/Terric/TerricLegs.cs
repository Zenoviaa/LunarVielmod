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
    [AutoloadEquip(EquipType.Legs)]
    public class TerricLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Terric Boots");
            // Tooltip.SetDefault("Increases movement Speed 8%");
        }
        public override void SetDefaults()
        {
            Item.Size = new Vector2(18);
            Item.value = Item.sellPrice(silver: 22);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 5;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.4f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<TerrorFragments>(), 5);
            recipe.AddIngredient(ItemType<DreadFoil>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
