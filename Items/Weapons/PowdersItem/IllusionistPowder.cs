using Stellamod.Projectiles.Powders;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Stellamod.Items.Harvesting;

namespace Stellamod.Items.Weapons.PowdersItem
{
	internal class IllusionistPowder : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Kaev Powder");
			/* Tooltip.SetDefault("Throw magical dust on them and ignite" +
				"\nA blood dust that does high damage with igniters."); */
		}
		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<IllusionistPowderProj>();
			Item.autoReuse = true;
			Item.shootSpeed = 12f;
			Item.crit = 0;
			Item.UseSound = SoundID.Grass;
		}


		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Bagitem>(), 1);
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 50);
			recipe.AddTile(ModContent.TileType<AlcaologyTable>());

			recipe.Register();
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, velocity *= player.GetModPlayer<MyPlayer>().IgniterVelocity, type, damage, knockback, player.whoAmI);
			return false;
		}
	}
}