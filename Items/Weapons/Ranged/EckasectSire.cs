using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Crossbows.Eckasect;
using Stellamod.Projectiles.Crossbows.Ultras;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{

    public class EckasectSire : ModItem
    {
		public int AttackCounter = 1;
		public int combowombo = 0;
		public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wooden Crossbow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Use a small crossbow and shoot three bolts!"
                + "\n'Triple Threat!'"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{

			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");
			line = new TooltipLine(Mod, "Sire", "(A) 3 Weapons in one!")
			{
				OverrideColor = new Color(108, 271, 99)

			};
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Sire", "(A) Use your right click to switch between 3 forms!")
			{
				OverrideColor = new Color(220, 87, 24)

			};
			tooltips.Add(line);








		}
		public override void SetDefaults()
        {
            Item.damage = 21;
            Item.DamageType = DamageClass.Magic;
            Item.width = 32;
            Item.height = 25;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<EckasectGenesisHold>();
            Item.scale = 0.8f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(gold: 50);
            Item.noUseGraphic = true;
            Item.channel = true;


        }
	
		
		//Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Accessories/Brooches/AdvancedBroochesBackpack").Value;
	//	spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);

	//	return false;

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				if (!player.HasBuff<Genesis>() && !player.HasBuff<Liberator>() && !player.HasBuff<Executor>())
				{
					player.AddBuff(ModContent.BuffType<Genesis>(), 100000);
					player.ClearBuff(ModContent.BuffType<Liberator>());
					player.ClearBuff(ModContent.BuffType<Executor>());

					Item.shoot = ModContent.ProjectileType<EckasectGenesisSwitch>();
					Item.noUseGraphic = true;
					Item.useStyle = ItemUseStyleID.Shoot;
					Item.noMelee = true; //so the Item's animation doesn't do damage
					Item.knockBack = 11;
					Item.autoReuse = true;
					Item.mana = 20;
					Item.useTurn = true;
					Item.DamageType = DamageClass.Magic;
					Item.shootSpeed = 10f;
					Item.useAnimation = 20;
					Item.useTime = 20;
					Item.consumeAmmoOnLastShotOnly = true;
					Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");

					return true;
				}


				else if (!player.HasBuff<Genesis>() && !player.HasBuff<Liberator>() || player.HasBuff<Executor>())
				{
					player.AddBuff(ModContent.BuffType<Genesis>(), 100000);
					player.ClearBuff(ModContent.BuffType<Liberator>());
					player.ClearBuff(ModContent.BuffType<Executor>());

					Item.shoot = ModContent.ProjectileType<EckasectGenesisSwitch>();
					Item.noUseGraphic = true;
					Item.useStyle = ItemUseStyleID.Shoot;
					Item.noMelee = true; //so the Item's animation doesn't do damage
					Item.knockBack = 11;
					Item.autoReuse = true;
					Item.mana = 20;
					Item.useTurn = true;
					Item.DamageType = DamageClass.Magic;
					Item.shootSpeed = 10f;
					Item.useAnimation = 20;
					Item.useTime = 20;
					Item.consumeAmmoOnLastShotOnly = true;
					Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");

					return true;
				}

				else if (player.HasBuff<Genesis>())
				{
					player.AddBuff(ModContent.BuffType<Liberator>(), 100000);
					player.ClearBuff(ModContent.BuffType<Genesis>());
					player.ClearBuff(ModContent.BuffType<Executor>());

					Item.shoot = ModContent.ProjectileType<EckasectLiberatorSwitch>();
					Item.noUseGraphic = true;
					Item.useStyle = ItemUseStyleID.Shoot;
					Item.noMelee = true; //so the Item's animation doesn't do damage
					Item.knockBack = 11;
					Item.autoReuse = true;
					Item.mana = 20;
					Item.useTurn = true;
					Item.DamageType = DamageClass.Magic;
					Item.shootSpeed = 10f;
					Item.useAnimation = 20;
					Item.useTime = 20;
					Item.consumeAmmoOnLastShotOnly = true;
					Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");

					return true;
				}

				else if (player.HasBuff<Liberator>())
				{
					player.AddBuff(ModContent.BuffType<Executor>(), 100000);
					player.ClearBuff(ModContent.BuffType<Genesis>());
					player.ClearBuff(ModContent.BuffType<Liberator>());

					Item.shoot = ModContent.ProjectileType<EckasectExecutorSwitch>();
					Item.noUseGraphic = true;
					Item.useStyle = ItemUseStyleID.Shoot;
					Item.noMelee = true; //so the Item's animation doesn't do damage
					Item.knockBack = 11;
					Item.autoReuse = true;
					Item.mana = 20;
					Item.useTurn = true;
					Item.DamageType = DamageClass.Magic;
					Item.shootSpeed = 10f;
					Item.useAnimation = 20;
					Item.useTime = 20;
					Item.consumeAmmoOnLastShotOnly = true;
					Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");
					//GallinLock2
					return true;
				}


			}
			else
			{
				Item.mana = 0;
				Item.UseSound = null;
				Item.DamageType = DamageClass.Magic;
			}

			return true;
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Player player = Main.player[Main.myPlayer];

			if (!player.HasBuff<Genesis>() && !player.HasBuff<Liberator>() && !player.HasBuff<Executor>())
			{
				Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Ranged/EckasectSire").Value;
				spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
				return false;
			}
			if (player.HasBuff<Genesis>())
			{
				Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Ranged/EckasectSire").Value;
				spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
				return false;
			}

			if (player.HasBuff<Liberator>())
			{
				Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Ranged/EckasectLiber").Value;
				spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
				return false;
			}

			if (player.HasBuff<Executor>())
			{
				Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Ranged/EckasectExe").Value;
				spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
				return false;
			}


			return true;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse == 2)
			{
				int count = 48;
				for (int k = 0; k < count; k++)
				{
					Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
					newVelocity *= 1f - Main.rand.NextFloat(0.3f);
					Dust.NewDust(position, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
				}

				return true;
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


				if (!player.HasBuff<Genesis>() && !player.HasBuff<Liberator>() && !player.HasBuff<Executor>())
				{
					Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<EckasectGenesisHold>(), damage, knockback, player.whoAmI, 1, dir);
				}

				if (player.HasBuff<Genesis>())
				{
					Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<EckasectGenesisHold>(), damage, knockback, player.whoAmI, 1, dir);
				}

				if (player.HasBuff<Liberator>())
				{
					Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<EckasectLiberatorHold>(), damage, knockback, player.whoAmI, 1, dir);
				}

				if (player.HasBuff<Executor>())
				{
					Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<EckasectExecutorHold>(), damage, knockback, player.whoAmI, 1, dir);
					Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ExecutionRay>(), damage / 2, knockback, player.whoAmI, 1, dir);
				}
			}


			return false;
		}
	


	}
}