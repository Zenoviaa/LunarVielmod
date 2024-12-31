using Stellamod.Common.Bases;
using Stellamod.Projectiles.Crossbows;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged.Crossbows
{
    internal class WoodenCrossbow : BaseCrossbowItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 12;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            CrossbowProjectileType = ModContent.ProjectileType<WoodenCrossbowHold>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 15);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}