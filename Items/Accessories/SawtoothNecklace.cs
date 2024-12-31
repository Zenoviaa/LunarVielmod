using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class SawtoothNecklace : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 48;
            Item.value = 2500;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            //Increased armor pen
            player.GetArmorPenetration(DamageClass.Generic) += 12;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SharkToothNecklace, 1);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 20);
            recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
