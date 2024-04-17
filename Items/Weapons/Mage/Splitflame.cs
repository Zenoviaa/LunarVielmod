using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class Splitflame : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Split-flame candle");
			// Tooltip.SetDefault("Totally not the candle from split, totally.");
		}
		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.mana = 5;
			Item.width = 18;
			Item.height = 21;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.noMelee = true;
			Item.knockBack = 4f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 10000;
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.DD2_BookStaffCast;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<splitshot>();
			Item.shootSpeed = 4f;
			Item.autoReuse = true;
			Item.crit = 7;
		}

        public override void AddRecipes()
        {
            base.AddRecipes();
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Torch, 12);
			recipe.AddTile(TileID.Anvils);
			recipe.AddIngredient(ItemID.HellstoneBar, 12);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 7);
			recipe.Register();
		}
    }
}