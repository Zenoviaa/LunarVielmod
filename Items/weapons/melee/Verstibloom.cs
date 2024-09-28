using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee
{
	public class Verstibloom : ClassSwapItem
	{
		//Alternate class you want it to change to
		public override DamageClass AlternateClass => DamageClass.Magic;

		//Defaults for the other class
		public override void SetClassSwappedDefaults()
		{
			//Do if(IsSwapped) if you want to check for the alternate class
			//Stats to have when in the other class
			Item.damage = 14;
			Item.mana = 20;
			Item.knockBack = 12;
		}
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Swing");
			/* Tooltip.SetDefault("Shoots one bone bolt to swirl and kill your enemies after attacking!" +
			"\nHitting foes with the melee swing builds damage towards the swing of the weapon"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.DamageType = DamageClass.Melee;
			Item.width = 32;
			Item.mana = 3;
			Item.height = 32;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 7;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.autoReuse = false;
			Item.value = Item.sellPrice(0, 0, 0, 20);
			Item.shoot = ModContent.ProjectileType<VerstiSwing>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

			if (Item.shoot == ModContent.ProjectileType<VerstiSwing>())
				Item.shoot = ModContent.ProjectileType<VerstiSwing2>();
			else
				Item.shoot = ModContent.ProjectileType<VerstiSwing>();

			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.Anvils);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
			recipe.AddIngredient(ModContent.ItemType<FrostSwing>(), 1);
			recipe.AddIngredient(ItemID.JungleSpores, 12);
			recipe.Register();
		}
	}
}