﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Materials;
using Stellamod.Items.Accessories;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.UI.Panels;
using Terraria.Audio;
using Stellamod.Projectiles;

namespace Stellamod.Items.Consumables
{
	public class Gambit : ModItem
	{

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gambit Dice");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}" +
				"\n Gamble away your soul after battle for great rewards!"); // References a language key that says "Right Click To Open" in the language of the game

			ItemID.Sets.PreHardmodeLikeBossBag[Type] = true; // ..But this set ensures that dev armor will only be dropped on special world seeds, since that's the behavior of pre-hardmode boss bags.

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
		}

		public override void SetDefaults()
		{
			Item.maxStack = 10;
			Item.consumable = true;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Purple;
			Item.value = 200;
			
		}
		public float Timer;
		public override bool CanRightClick()
		{
			return true;
		}

		public override void RightClick(Player player)
		{
			var entitySource = player.GetSource_OpenItem(Type);
			switch (Main.rand.Next(5))
			{

				case 0:


					CombatText.NewText(player.getRect(), Color.YellowGreen, "Wohooo", true, false);


					player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(1, 1));

					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));



					break;

				case 1:


					CombatText.NewText(player.getRect(), Color.YellowGreen, "Omg, its something!", true, false);

					player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(1, 2));

					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));



					break;

				case 2:


					CombatText.NewText(player.getRect(), Color.YellowGreen, "Are you disappointed? You should be.", true, false);
					player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(0, 1));
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));




					break;

				case 3:


					CombatText.NewText(player.getRect(), Color.YellowGreen, "Wow, you have no maidens and no luck..", true, false);

					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


					break;


					break;


				case 4:

					CombatText.NewText(player.getRect(), Color.YellowGreen, "Sooo lucky!", true, false);




					player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(2, 2));

					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));





					break;



					



			}
			SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Rolldice"));
			float speedX = Main.rand.NextFloat(-4f, 4f);
			float speedY = Main.rand.Next(-10, 11) * 0.2f;
			Vector2 here = player.position;
			Projectile.NewProjectile(entitySource, player.position.X, player.position.Y + 60, speedX * 0, speedY * 0, ModContent.ProjectileType<Dicein>(), 0, 0f, player.whoAmI, 0f, 0f);
			player.GetModPlayer<MyPlayer>().Dice = true;
		}
		// Below is code for the visuals

		public override Color? GetAlpha(Color lightColor)
		{
			// Makes sure the dropped bag is always visible
			return Color.Lerp(lightColor, Color.White, 0.4f);
		}

		public override void PostUpdate()
		{
			// Spawn some light and dust when dropped in the world
			Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

			if (Item.timeSinceItemSpawned % 12 == 0)
			{
				Vector2 center = Item.Center + new Vector2(0f, Item.height * -0.1f);

				// This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
				Vector2 direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
				float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
				Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

				Dust dust = Dust.NewDustPerfect(center + direction * distance, DustID.SilverFlame, velocity);
				dust.scale = 0.5f;
				dust.fadeIn = 1.1f;
				dust.noGravity = true;
				dust.noLight = true;
				dust.alpha = 0;
			}
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

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			return true;
		}
	}
}