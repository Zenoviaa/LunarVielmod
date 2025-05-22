
using Terraria;
using Terraria.ID;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class IronWand : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 9;
            Item.shootSpeed = 10;
            Item.useTime = 18;
            Item.useAnimation = 36;
            Size = 8;
            TrailLength = 16;
        }


        public override int GetNormalSlotCount()
        {
            return 2;
        }

        public override int GetTimedSlotCount()
        {
            return 0;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.IronBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
