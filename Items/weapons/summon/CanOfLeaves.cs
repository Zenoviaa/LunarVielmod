using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Projectiles;
using Stellamod.Items.Materials;
using Terraria.DataStructures;
using Stellamod.UI.systems;

namespace Stellamod.Items.weapons.summon
{
	internal class CanOfLeaves : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Box of Leaves");
			Tooltip.SetDefault("Summon the leaves, dont bother picking them up, they are a bit aggressive, and magical...");
		}


		public override void SetDefaults()
		{
			Item.damage = 9;
			Item.mana = 1;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.staff[Item.type] = true;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Summon;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = new Terraria.Audio.LegacySoundStyle(2, 8);
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Logger>();
			Item.shootSpeed = 10f;
			Item.autoReuse = true;
			Item.crit = 15;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    
		{
			// NewProjectile returns the index of the projectile it creates in the NewProjectile array.
			// Here we are using it to gain access to the projectile object.
			int projectileID = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			Projectile projectile = Main.projectile[projectileID];

			ProjectileModifications globalProjectile = projectile.GetGlobalProjectile<ProjectileModifications>();
			// For more context, see ExampleProjectileModifications.cs
			globalProjectile.SetTrail(Color.DarkKhaki);
			globalProjectile.sayTimesHitOnThirdHit = false;
			globalProjectile.applyBuffOnHit = true;

			// We do not want vanilla to spawn a duplicate projectile.
			return false;
		}



		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BorealWood, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 10);
			recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 5);
			recipe.AddIngredient(ItemID.JungleSpores, 3);
		}
	}
}







