using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    internal class Stonen : ClassSwapItem
	{
		public override DamageClass AlternateClass => DamageClass.Melee;

        public override void SetDefaults()
		{
			Item.damage = 98;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Throwing;
			Item.value = 1200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<RockerP>();
			Item.shootSpeed = 10f;
			Item.autoReuse = true;
			Item.crit = 15;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BeetleHusk, 5);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 25);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 25);
			recipe.AddIngredient(ItemID.ThrowingKnife, 3);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}