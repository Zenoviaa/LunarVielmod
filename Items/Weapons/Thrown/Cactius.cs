
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Items.Weapons.Thrown
{
    public class Cactius : ModItem
	{
        public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Cactius"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

        public override void SetDefaults()
        {
            Item.damage = 6;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 40;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.crit = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Cactius2>();
            Item.shootSpeed = 15f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.PalmWood, 13);
            recipe.AddIngredient(ModContent.ItemType<Plantius>(), 1);
            recipe.AddIngredient(ItemID.Cactus, 5);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}