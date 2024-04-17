using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class CocoSpark : ClassSwapItem
	{
		//Alternate class you want it to change to
		public override DamageClass AlternateClass => DamageClass.Summon;

		//Defaults for the other class
		public override void SetClassSwappedDefaults()
		{
			//Do if(IsSwapped) if you want to check for the alternate class
			//Stats to have when in the other class
			Item.mana = 0;
			Item.damage = 48;
		}
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Coconut Tome");
			// Tooltip.SetDefault("Look master, I can summon, Coconuts!");
		}
		public override void SetDefaults()
		{
			Item.damage = 15;
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
			recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 8);
			recipe.AddIngredient(ModContent.ItemType<VerianOre>(), 8);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}









