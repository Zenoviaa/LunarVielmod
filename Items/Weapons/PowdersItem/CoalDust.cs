using Stellamod.Items.Harvesting;
using Stellamod.Projectiles.Powders;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class CoalDust : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Coal Powder");
			/* Tooltip.SetDefault("Throw magical dust on them!" +
				"\nForged from the gems you collect!" +
				"\nTia is a bum"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 4;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<CoalPowder>();
			Item.autoReuse = true;
			Item.shootSpeed = 12f;
			Item.crit = 43;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Lenabee");
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.StoneBlock, 50);
			recipe.AddIngredient(ItemID.DirtBlock, 15);
			recipe.AddIngredient(ItemID.Bomb, 3);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<Bagitem>(), 3);
			recipe.AddIngredient(ItemID.Leather, 5);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			int dir = player.direction;

			Projectile.NewProjectile(source, position, velocity *= player.GetModPlayer<MyPlayer>().IgniterVelocity, type, damage, knockback, player.whoAmI);
			return false;
		}
	}
}