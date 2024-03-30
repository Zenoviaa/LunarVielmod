using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Powders;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class CrystalPowder : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sepsis Powder");
			/* Tooltip.SetDefault("Throw magical dust on them!" +
				"\nA sparkly star dust that does double damage as the igniter!"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.LightRed;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<CrystalPowderProj>();
			Item.autoReuse = true;
			Item.shootSpeed = 12f;
			Item.crit = 2;
			Item.UseSound = SoundID.Grass;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.CrystalShard, 15);
			recipe.AddIngredient(ItemID.FallenStar, 10);
			recipe.AddIngredient(ModContent.ItemType<Bagitem>(), 3);
			recipe.AddIngredient(ModContent.ItemType<ConvulgingMater>(), 50);
			recipe.AddIngredient(ModContent.ItemType<MorrowVine>(), 200);
			recipe.AddTile(ModContent.TileType<AlcaologyTable>());

			recipe.Register();
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			int dir = player.direction;

			Projectile.NewProjectile(source, position, velocity *= player.GetModPlayer<MyPlayer>().IgniterVelocity, type, damage, knockback, player.whoAmI);
			return false;
		}

	}
}