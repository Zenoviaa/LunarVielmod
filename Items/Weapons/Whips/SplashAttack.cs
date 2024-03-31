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
    internal class SplashAttack : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SplashAttackDebuff.TagDamage);
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 54;
            Item.DefaultToWhip(ModContent.ProjectileType<SplashAttackProj>(), 10, 3, 24);
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<KaleidoscopicInk>(), 20);
            recipe.AddIngredient(ModContent.ItemType<ArtisanBar>(), 5);
            recipe.AddIngredient(ItemID.ThornWhip, 1);
            recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 10);
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
