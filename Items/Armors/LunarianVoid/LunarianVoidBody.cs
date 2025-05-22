using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Armors.LunarianVoid
{
    [AutoloadEquip(EquipType.Body)]
    public class LunarianVoidBody : ModItem
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
            Item.rare = ItemRarityID.Green;
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Throwing) += 10f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 14);
            recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 30);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
