using Stellamod.Buffs.Whipfx;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Whips;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Whips
{
    internal class ColdheartAnklebiter : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ColdheartAnklebiterDebuff.TagDamage);
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 54;
            Item.DefaultToWhip(ModContent.ProjectileType<ColdheartAnklebiterProj>(), 82, 3, 24);
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<IllurineScale>(), 18);
            recipe.AddIngredient(ItemID.Ectoplasm, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        // Makes the whip receive melee prefixes
        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
