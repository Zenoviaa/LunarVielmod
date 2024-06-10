using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
	internal class SpiritCapsule : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 100;

        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Pearlescent Ice Ball");
			// Tooltip.SetDefault("Shoots fast homing sparks of light!");
		}
		public override void SetDefaults()
		{
			Item.damage = 120;
			Item.width = 40;
			Item.height = 40;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.DamageType = DamageClass.Throwing;
			Item.value = 12000;
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.DD2_DarkMageAttack;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SpiritCapsuleP>();
			Item.shootSpeed = 20f;
			Item.autoReuse = true;
			Item.crit = 12;
			Item.noUseGraphic = true;
			Item.useAnimation = 25;
			Item.useTime = 25;
			Item.consumable = false;

		}


		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Ectoplasm, 35);
			recipe.AddIngredient(ItemID.SoulofFlight, 35);
			recipe.AddIngredient(ItemID.SoulofFright, 35);
			recipe.AddIngredient(ItemID.SoulofSight, 35);
			recipe.AddIngredient(ItemID.SoulofMight, 35);
			recipe.AddIngredient(ItemID.SoulofLight, 35);
			recipe.AddIngredient(ItemID.SoulofNight, 35);
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 35);
			recipe.AddIngredient(ModContent.ItemType<GraftedSoul>(), 35);
			recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 35);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}









