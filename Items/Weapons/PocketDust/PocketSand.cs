using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.PocketProj;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PocketDust
{
    internal class PocketSand : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Pocket Sand");
			/* Tooltip.SetDefault("Throw magical dust on them!" +
				"\nDust that can be used with and for combos in igniters" +
				"\n Can penetrate armored enemies like nothing!"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 8;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<PocketSandProj>();
			Item.autoReuse = true;
			Item.shootSpeed = 20f;
			Item.ArmorPenetration = 300;
			Item.noUseGraphic = true;

			Item.crit = 12;
			Item.UseSound = SoundID.Grass;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.SandBlock, 100);
			recipe.AddIngredient(ItemID.FallenStar, 1);
			recipe.AddTile(ModContent.TileType<AlcaologyTable>());
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<Bagitem>(), 1);
			recipe.AddIngredient(ModContent.ItemType<MorrowVine>(), 5);
			recipe.AddIngredient(ItemID.Silk, 5);
		}

	}
}