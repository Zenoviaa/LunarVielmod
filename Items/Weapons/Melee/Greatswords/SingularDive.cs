using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Gun;
using Stellamod.Projectiles.Slashers.SingularDive;
using Stellamod.Projectiles.Slashers.Voyager;
using Stellamod.Projectiles.Swords.Altride;
using Stellamod.Projectiles.Swords.Fenix;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee.Greatswords
{
	public class SingularDive : ModItem
	{

		public int AttackCounter = 1;
		public int combowombo = 0;

		public override void SetStaticDefaults()
		{


		
			ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
			ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			// DisplayName.SetDefault("Frost Swing");
			/* Tooltip.SetDefault("Shoots one bone bolt to swirl and kill your enemies after attacking!" +
			"\nHitting foes with the melee swing builds damage towards the swing of the weapon"); */
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{

			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");

			line = new TooltipLine(Mod, "Alcarishasd",  Helpers.LangText.Common("Greatsword"))
			{
				OverrideColor = ColorFunctions.GreatswordWeaponType
			};
			tooltips.Add(line);




		}


		public override void SetDefaults()
		{
			Item.damage = 23;
			Item.DamageType = DamageClass.Melee;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 15;
			Item.rare = ItemRarityID.Lime;
			Item.autoReuse = true;
			Item.value = 100000;
			Item.shoot = ModContent.ProjectileType<SingularDiveProj>();
			Item.shootSpeed = 20f;
			Item.noUseGraphic = true;
			Item.noMelee = true;


		}

		

		

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.shoot = ModContent.ProjectileType<SingularDiveDive>();
				Item.noUseGraphic = true;
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.noMelee = true; //so the Item's animation doesn't do damage
				Item.knockBack = 17;
				Item.autoReuse = true;
				Item.useTurn = true;
				Item.DamageType = DamageClass.Melee;
				Item.shootSpeed = 20f;
				Item.useAnimation = 20;
				Item.useTime = 45;
				Item.consumeAmmoOnLastShotOnly = true;

			}
			else
			{

			}

			return base.CanUseItem(player);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse == 2)
			{
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SingularDiveDive>(), damage * 3, knockback, player.whoAmI);
                return false;
			}



			if (player.altFunctionUse != 2)
			{


				int dir = AttackCounter;
				if (player.direction == 1)
				{
					player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter;
				}
				else
				{
					player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter * -1;

				}
				AttackCounter = -AttackCounter;
				Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SingularDiveProj>(), damage * 3, knockback, player.whoAmI, 1, dir);


				float numberProjectiles = 2;
				float rotation = MathHelper.ToRadians(20);
				position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
				for (int i = 0; i < numberProjectiles; i++)
				{
					Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
					Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<GraniteMagmumProj>(), damage, Item.knockBack, player.whoAmI);
				}

			}
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwingyAr") { Pitch = Main.rand.NextFloat(-10f, 10f) }, player.Center);

            return false;
		}


		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.Anvils);
			recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 20);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 10);
			recipe.Register();
		}


	}
}