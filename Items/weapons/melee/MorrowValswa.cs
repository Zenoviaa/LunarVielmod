using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
	public class MorrowValswa : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrow Valswa");
		}
		public override void SetDefaults()
		{
			Item.channel = true;
			Item.damage = 16;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.crit = 4;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 8;
			Item.useTurn = false;
			Item.value = Terraria.Item.sellPrice(0, 0, 1, 0);
			Item.rare = ItemRarityID.White;
			Item.autoReuse = true;
			Item.shootSpeed = 6f;
			Item.noUseGraphic = false;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
		}
		public override bool CanUseItem(Player player)
		{
			return base.CanUseItem(player);
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.WoodenSword, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 3);
			recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 15);
			recipe.AddIngredient(ItemID.Torch, 20);
		}
	}
}