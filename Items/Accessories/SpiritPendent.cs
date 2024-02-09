
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class SpiritPendent : ModItem
    {
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(gold: 2);
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MyPlayer>().SpiritPendent = true;
            Lighting.AddLight(player.position, 2.0f, 2.0f, 4.75f);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
