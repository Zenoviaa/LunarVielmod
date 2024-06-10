using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class PrismaticBalls : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Throwing;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 20;
            Item.mana = 0;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Prismatic Cryadia Ball");
			// Tooltip.SetDefault("Shoots fast homing sparks of everlasting crystal light!");
		}
		public override void SetDefaults()
		{
			Item.damage = 23;
			Item.mana = 2;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 10000;
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.DD2_BookStaffCast;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.RainbowRodBullet;
			Item.shootSpeed = 7f;
			Item.autoReuse = true;
			Item.crit = 12;
			Item.noUseGraphic = true;
		}

	
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Ectoplasm, 10);
			recipe.AddIngredient(ItemID.CrystalSerpent, 1);
			recipe.AddIngredient(ItemID.CrystalBall, 1);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 10);



			Recipe recipe2 = CreateRecipe();
			recipe2.AddIngredient(ItemID.Ectoplasm, 10);
			recipe2.AddIngredient(ItemID.RainbowRod, 1);
			recipe2.AddTile(TileID.MythrilAnvil);
			recipe2.Register();
			recipe2.AddIngredient(ModContent.ItemType<FlameburstBalls>(), 1);
			
		}
	}
}









