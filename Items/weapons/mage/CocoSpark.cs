using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
	internal class CocoSpark : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Coconut Tome");
			Tooltip.SetDefault("Look master, I can summon, Coconuts!");
		}
		public override void SetDefaults()
		{
			Item.damage = 17;
			Item.mana = 5;
			Item.width = 18;
			Item.height = 21;
			Item.useTime = 22;
			Item.useAnimation = 132;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_BookStaffCast;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<CocoShot>();
			Item.shootSpeed = 4f;
			Item.autoReuse = true;
			Item.crit = 22;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.PalmWood, 25);
			recipe.AddIngredient(ItemID.Seashell, 5);
			recipe.AddIngredient(ItemID.Starfish, 8);
			recipe.AddIngredient(ItemID.DemoniteBar, 8);
			recipe.AddIngredient(ItemID.ShadowScale, 8);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 8);
			recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 8);
		}
	}
}









