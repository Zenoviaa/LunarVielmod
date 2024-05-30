using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged.Crossbows;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Magic;
using Stellamod.Projectiles.Slashers.Voyager;
using Stellamod.Projectiles.Swords.Altride;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Mage
{
	public class Rhamenthal : ClassSwapItem
	{
		//Alternate class you want it to change to
		public override DamageClass AlternateClass => DamageClass.Ranged;

		//Defaults for the other class
		public override void SetClassSwappedDefaults()
		{
			//Do if(IsSwapped) if you want to check for the alternate class
			//Stats to have when in the other class
			Item.mana = 0;
			Item.damage = 60;
		}
		public override void SetStaticDefaults()
		{


			
			 // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
			ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			// DisplayName.SetDefault("Frost Swing");
			/* Tooltip.SetDefault("Shoots one bone bolt to swirl and kill your enemies after attacking!" +
			"\nHitting foes with the melee swing builds damage towards the swing of the weapon"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 75;
			Item.DamageType = DamageClass.Magic;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 75;
			Item.useAnimation = 75;
			Item.knockBack = 15;
			Item.value = 100000;
			Item.shoot = ModContent.ProjectileType<RhamenthalProjHold>();
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.rare = ItemRarityID.Lime;
			Item.autoReuse = false;
			Item.shootSpeed = 30f;
			Item.scale = 0.8f;
			Item.noMelee = true; // The projectile will do the damage and not the item
			Item.value = Item.buyPrice(silver: 3);
			Item.noUseGraphic = true;
			Item.channel = true;


		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);

			recipe.AddIngredient(ModContent.ItemType<KaleidoscopicInk>(), 20);
			recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 100);
			recipe.AddIngredient(ModContent.ItemType<Nekomara>(), 1);
			recipe.AddIngredient(ModContent.ItemType<CoralCrossbow>(), 1);
			recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 15);
			recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 90);
			recipe.AddIngredient(ModContent.ItemType<TerrorFragments>(), 12);
			recipe.AddIngredient(ModContent.ItemType<WickofSorcery>(), 1);
			recipe.AddIngredient(ItemID.SniperRifle, 1);
			recipe.Register();
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