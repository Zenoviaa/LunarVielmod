using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Armors.Astrasilk
{
    [AutoloadEquip(EquipType.Body)]
    public class AstrasilkBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Astrasilk Jacket");
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
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Magic) += 0.25f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<StarSilk>(), 14);
            recipe.AddIngredient(ModContent.ItemType<WinterbornShard>(), 11);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }
}
