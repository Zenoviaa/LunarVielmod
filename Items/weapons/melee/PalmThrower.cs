using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Projectiles;
using Stellamod.Items.Materials;
using Terraria.DataStructures;
using Stellamod.UI.systems;
using Terraria;

namespace Stellamod.Items.weapons.melee
{
	internal class PalmThrower : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Palm Tomahawks");
			Tooltip.SetDefault("Throw around tomahawks forged from palm, sounds boring :(");
		}


		public override void SetDefaults()
		{
			Item.damage = 28;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Melee;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Beachthrow>();
			Item.shootSpeed = 7f;
			Item.autoReuse = true;
			Item.crit = 15;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}





		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.PalmWood, 5);
			recipe.AddIngredient(ItemID.Seashell, 5);
			recipe.AddIngredient(ItemID.DemoniteBar, 7);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 25);
			recipe.AddIngredient(ItemID.ThrowingKnife, 3);
		}
	}
}