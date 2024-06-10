using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class FlameburstBalls : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Throwing;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 10;
            Item.mana = 0;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Molted Crust Ball");
			// Tooltip.SetDefault("Shoots fast homing sparks of fire!");
		}
		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.mana = 2;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 14;
			Item.useAnimation = 14;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.DD2_BetsyFireballShot;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Flamelash;
			Item.shootSpeed = 7f;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.noUseGraphic = true;
		}
		

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.HellstoneBar, 10);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
	
			Recipe recipe2 = CreateRecipe();
			recipe2.AddIngredient(ItemID.Flamelash, 1);
			recipe2.AddTile(TileID.Anvils);
			recipe2.Register();
			recipe2.AddIngredient(ModContent.ItemType<EvasiveBalls>(), 1);
			
		}
	}
}









