using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class EvasiveBalls : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Pearlescent Ice Ball");
			// Tooltip.SetDefault("Shoots fast homing sparks of light!");
		}
		public override void SetDefaults()
		{
			Item.damage = 13;
			Item.mana = 2;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_DarkMageAttack;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.MagicMissile;
			Item.shootSpeed = 7f;
			Item.autoReuse = true;
			Item.crit = 12;
			Item.noUseGraphic = true;
		}
		

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BorealWood, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 20);
		}
	}
}









