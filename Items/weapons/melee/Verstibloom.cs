using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee
{
    public class Verstibloom : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Thorner");
			/* Tooltip.SetDefault("Shoots a swirling red bloom to swirl and kill your enemies after attacking!" +
				"\nHitting foes with the melee swing builds damage towards the swing of the weapon" +
				"\nDoes trenourmous damage!"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 46;
			Item.DamageType = DamageClass.Melee;
			Item.width = 32;
			Item.mana = 5;
			Item.height = 32;
			Item.useTime = 23;
			Item.crit = 30;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_DarkMageAttack;
			Item.autoReuse = false;
			Item.value = Item.sellPrice(0, 0, 0, 20);
			Item.shoot = ModContent.ProjectileType<FrostSwProj2>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback);
			
			if (Item.shoot == ModContent.ProjectileType<FrostSwProj2>())
				Item.shoot = ModContent.ProjectileType<VerstibloomProjectile>();
			else
				Item.shoot = ModContent.ProjectileType<FrostSwProj2>();

			return base.Shoot(player, source, position, velocity, type, damage, knockback);

		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.WoodenSword, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 5);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 5);
			recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 15);
			recipe.AddIngredient(ItemID.Vine, 3);
			recipe.AddIngredient(ItemID.Stinger, 3);
		}
	}
}