using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
	public class Yumiko : ClassSwapItem
	{
		//Alternate class you want it to change to
		public override DamageClass AlternateClass => DamageClass.Magic;

		//Defaults for the other class
		public override void SetClassSwappedDefaults()
		{
			//Do if(IsSwapped) if you want to check for the alternate class
			//Stats to have when in the other class
			Item.damage = 200;
			Item.mana = 22;
			Item.useTime = 50;
			Item.useAnimation = 50;
		}
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
			line = new TooltipLine(Mod, "Alcarishxx", "The sorrowing souls within are crying")
			{
				OverrideColor = new Color(244, 119, 255)

			};
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Alcarishxx", "This weapon is bound by Fenix")
			{
				OverrideColor = new Color(244, 200, 255)

			};
			tooltips.Add(line);
			base.ModifyTooltips(tooltips);


		}
		public override void SetDefaults()
		{
			Item.damage = 225;
			Item.DamageType = DamageClass.Melee;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 80;
			Item.useAnimation = 80;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 15;
			Item.rare = ItemRarityID.Lime;
			Item.autoReuse = true;
			Item.value = 100000;
			Item.shoot = ModContent.ProjectileType<YumikoShot>();
			Item.shootSpeed = 80f;
			Item.noMelee = true;
			Item.noUseGraphic = true;


		}


		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			float numberProjectiles = 4;
			float numberProjectiles2 = 3;
			float rotation = MathHelper.ToRadians(20);
			float rotation2 = MathHelper.ToRadians(30);
			position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
				Vector2 perturbedSpeed2 = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation2, rotation2, i / (numberProjectiles2 - 1))) * 1f;// This defines the projectile roatation and speed. .4f == projectile speed
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, ModContent.ProjectileType<YumikoShot2>(), damage, Item.knockBack, player.whoAmI);
			}

			int Sound = Main.rand.Next(1, 4);
			if (Sound == 1)
			{
				 SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Yumiko"), position);
			}
			if (Sound == 2)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Yumiko2"), position);
			}
			if (Sound == 3)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Yumiko3"), position);

			}
			if (Sound == 4)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Yumiko4"), position);

			}
			return false;
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
	}
}