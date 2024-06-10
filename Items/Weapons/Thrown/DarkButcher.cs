using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
	internal class DarkButcher : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Melee;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 50;

        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Palm Tomahawks");
			// Tooltip.SetDefault("Throw around tomahawks forged from palm, sounds boring :(");
		}
		public override void SetDefaults()
		{
			Item.damage = 58;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Throwing;
			Item.value = 1200;
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<DarkButcherP>();
			Item.shootSpeed = 7f;
			Item.autoReuse = true;
			Item.crit = 15;
			Item.noMelee = true;
			Item.noUseGraphic = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.CursedFlame, 5);
			recipe.AddIngredient(ItemID.ShadowScale, 5);
			recipe.AddIngredient(ModContent.ItemType<VerianOre>(), 25);
			recipe.AddIngredient(ModContent.ItemType<GraftedSoul>(), 25);
			recipe.AddIngredient(ItemID.ThrowingKnife, 3);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}