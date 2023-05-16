using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
	internal class Bongos : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star-Gilded Bongo");
			// Tooltip.SetDefault("Bong bong boom :)");
		}
		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.mana = 3;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 14;
			Item.useAnimation = 14;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/bongo");
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<BoomCircle>();
			Item.autoReuse = true;
			Item.crit = 22;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BorealWood, 10);
			recipe.AddIngredient(ItemID.Stinger, 3);
			recipe.AddIngredient(ItemID.JungleSpores, 3);
			recipe.AddIngredient(ItemID.FallenStar, 3);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 20);
			recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 120);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
			return false;
			
		}
	}
}
