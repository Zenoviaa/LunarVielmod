
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    public class CleanestCleaver : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Melee;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 20;
        }

        public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Plantius"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}


        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 40;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.crit = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CleanestCleaverProg>();
            Item.shootSpeed = 15f;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.IronBar, 9);
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}