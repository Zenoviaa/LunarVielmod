using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;

using Stellamod.Items.Materials;
using Stellamod.Projectiles.Weapons.Bow;

namespace Stellamod.Items.Weapons.Ranged

{
	public class CloudBow : ModItem
	{

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Overcast's String");
		}
		public override void SetDefaults() 
		{
			Item.damage = 31;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 37;
			Item.useAnimation = 37;
			Item.useStyle = 5;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Arrow;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (type == ProjectileID.WoodenArrowFriendly)
			{
				type = ModContent.ProjectileType<CloudBolt>();
			}
			float numberProjectiles = 3;
			float rotation = MathHelper.ToRadians(10);
			position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
			for (int i = 0; i < numberProjectiles; i++)
            {
                var EntitySource = player.GetSource_FromThis();
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
				Projectile.NewProjectile(EntitySource, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
			}

			return false; // return false because we don't want tmodloader to shoot projectile
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<AzureaBar>(), 20);
			recipe.AddTile(TileID.SkyMill);
			recipe.Register();
		}
	}
}