using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.weapons.melee
{
	public class Testsword : ModItem
	{
		public override void SetStaticDefaults()
		{
		    DisplayName.SetDefault("Test Sword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("This is a test sword");
		}

		public override void SetDefaults()
		{
			Item.damage = 5;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = 12;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ProjectileID.HallowStar;
			Item.autoReuse = true;
			Item.shootSpeed = 10;

		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
		
	}
}