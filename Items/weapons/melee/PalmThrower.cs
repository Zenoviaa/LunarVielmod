using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
	internal class PalmThrower : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Palm Tomahawks");
			// Tooltip.SetDefault("Throw around tomahawks forged from palm, sounds boring :(");
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
			recipe.AddIngredient(ModContent.ItemType<VerianOre>(), 25);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 25);
			recipe.AddIngredient(ModContent.ItemType<Stick>(), 25);
			recipe.AddIngredient(ItemID.ThrowingKnife, 3);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}