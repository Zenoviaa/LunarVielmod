using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

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
            Item.rare = ItemRarityID.Green;
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
