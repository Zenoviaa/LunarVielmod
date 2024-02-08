using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Powders;
using Stellamod.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class Verstidust : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Versti Powder");
			/* Tooltip.SetDefault("Throw magical dust on them!" +
				"\nA musical dust that has calming flowers"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 5;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Green;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<FlowerPowder>();
			Item.autoReuse = true;
			Item.shootSpeed = 28f;
			Item.crit = 26;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Lenabee");
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<DustedSilk>(), 12);
			recipe.AddIngredient(ModContent.ItemType<Bagitem>(), 1);
			recipe.AddIngredient(ModContent.ItemType<MorrowVine>(), 5);
			recipe.AddIngredient(ModContent.ItemType<StarSilk>(), 15);
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