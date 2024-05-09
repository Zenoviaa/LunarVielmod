using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Brooches;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.IgniterEx;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Igniters
{
	internal class MushyCard : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bone Pickler Card Igniter");
			/* Tooltip.SetDefault("Use with a combination of dusts to make spells :)" +
				"\n Use a powder or dust and then use this type of weapon!"); */
		}

		public override void SetDefaults()
		{
			Item.damage = 13;
			Item.mana = 3;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 70;
			Item.useAnimation = 70;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/clickk");
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<IgniterStart>();
			Item.autoReuse = true;
			Item.crit = 50;
			Item.shootSpeed = 20;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			for (int i = 0; i < Main.npc.Length; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.active && npc.HasBuff<Dusted>())
				{
					Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity, type, damage, knockback, player.whoAmI);

				}


			}
			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Mushroom>(), 150);
			recipe.AddIngredient(ItemID.Mushroom, 5);
			recipe.AddIngredient(ModContent.ItemType<GraftedSoul>(), 10);
			recipe.AddIngredient(ModContent.ItemType<BlankCard>(), 1);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}



		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Player player = Main.player[Main.myPlayer];
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();

			//Check that this item is equipped

			//Check that you have advanced brooches since these don't work without
			if (broochPlayer.hasIgniteron)
			{
				//Give backglow to show that the effect is active
				DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, new Color(198, 124, 225));
			}
			else
			{
				float sizeLimit = 28;
				//Draw the item icon but gray and transparent to show that the effect is not active
				Main.DrawItemIcon(spriteBatch, Item, position, Color.Gray * 0.8f, sizeLimit);
				return false;
			}


			return true;
		}

	}
}