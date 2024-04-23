using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
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


namespace Stellamod.Items.Weapons.Melee
{
	public class Angelenthal : ModItem
	{

		public int AttackCounter = 1;
		public int combowombo = 0;

		public override void SetStaticDefaults()
		{


			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));
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
			line = new TooltipLine(Mod, "Alcarishxxa", "A weapon so powerful, it holds a gravitation pull and can change forms!")
			{
				OverrideColor = new Color(244, 119, 255)

			};
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Alcarishxxa", "Right click for a powerful gravitation slam!")
			{
				OverrideColor = new Color(244, 119, 255)

			};
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Alcarishxxa", "This weapon is bound by Fenix")
			{
				OverrideColor = new Color(244, 200, 255)

			};
			tooltips.Add(line);



		}


		public override void SetDefaults()
		{
			Item.damage = 90;
			Item.DamageType = DamageClass.Generic;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 15;
			Item.rare = ItemRarityID.Lime;
			Item.autoReuse = true;
			Item.value = 100000;
			Item.shoot = ModContent.ProjectileType<AngelenthalProj1>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.noMelee = true;


		}

		

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			Rectangle frame;

			if (Main.itemAnimations[Item.type] != null)
			{
				// In case this item is animated, this picks the correct frame
				frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
			}
			else
			{
				frame = texture.Frame();
			}

			Vector2 frameOrigin = frame.Size() / 2f;
			Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
			Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

			time %= 4f;
			time /= 2f;

			if (time >= 1f)
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;

			for (float i = 0f; i < 1f; i += 0.25f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(200, 70, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(255, 255, 255, 67), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			return true;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.shoot = ModContent.ProjectileType<AngelenthalThrow>();
				Item.noUseGraphic = true;
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.noMelee = true; //so the Item's animation doesn't do damage
				Item.knockBack = 11;
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
				Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<AngelenthalProj1>(), damage, knockback, player.whoAmI, 1, dir);
				

			}


			return false;
		}




	
	}
}