using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.Projectiles.Nails;
using Stellamod.Projectiles.Paint;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
	public class InkingSpire : ModItem
	{
		public int AttackCounter = 1;
		public int combowombo = 1;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Doorlauncher"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Electrical nail, thats weird");
		}
		public override void SetDefaults()
		{
			Item.damage = 34;
			Item.DamageType = DamageClass.Melee;
			Item.width = 0;
			Item.height = 0;
			Item.useTime = 200;
			Item.useAnimation = 90;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.value = 10000;
			Item.noMelee = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<InkingSProj>();
			Item.shootSpeed = 20f;
			Item.noUseGraphic = true;
			Item.crit = 12;
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, 
			ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 Offset = Vector2.Normalize(velocity) * 1f;

			if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
			{
				position += Offset;
			}


			if(player.altFunctionUse == 2)
			{
				type = ModContent.ProjectileType<InkingSThrow>();
            }
			else
            {		
				combowombo++;
                if (combowombo == 4)
                {
                    type = ModContent.ProjectileType<InkingSProj>();
                    combowombo = 0;
                }
                else
                {
                    type = ModContent.ProjectileType<InkingSProj2>();
                }
            }
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.noUseGraphic = true;
				Item.useTime = 100; // The Item's use time in ticks (60 ticks == 1 second.)
				Item.useAnimation = 30; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.noMelee = true; //so the Item's animation doesn't do damage
				Item.knockBack = 11;

				Item.shootSpeed = 0f; // the speed of the projectile (measured in pixels per frame)
				Item.channel = true;			
			}
			else
            {
                Item.useTime = 200;
                Item.useAnimation = 90;
                Item.shoot = ModContent.ProjectileType<InkingSProj>();
                Item.shootSpeed = 20f;
            }

			return base.CanUseItem(player);
		}


		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse == 2)
			{
				return true;
			}

			if (player.altFunctionUse != 2)
			{
				int dir = AttackCounter;
				AttackCounter = -AttackCounter;
				Projectile.NewProjectile(source, position, velocity, type, (damage + player.GetModPlayer<MyPlayer>().PPPaintDMG2), 
					knockback, player.whoAmI, 1, dir);
			}
			
			return false;
		}


		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Antaciz>(), 1);
			recipe.AddIngredient(ModContent.ItemType<KaleidoscopicInk>(), 20);
			recipe.AddIngredient(ModContent.ItemType<ArtisanBar>(), 5);
			recipe.AddIngredient(ModContent.ItemType<DreadFoil>(), 5);
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 20);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}

	}
}