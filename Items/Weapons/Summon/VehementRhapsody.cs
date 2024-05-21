
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Items.Weapons.Summon.VehementRhapsody;
using static Terraria.ModLoader.ModContent;
using Stellamod.Trails;
using Terraria.Graphics.Shaders;
using Stellamod.Projectiles.Summons.Minions;
using Stellamod.Buffs.Minions;


namespace Stellamod.Items.Weapons.Summon
{
	public class VehementRhapsody : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Irradiated Creeper Staff");
			// Tooltip.SetDefault("Summons an Irradiated Creeper to fight with you");
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 11;
			Item.knockBack = 6f;
			Item.mana = 10;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 0, 33, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item44;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = ModContent.BuffType<VehementMinionBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ModContent.ProjectileType<VehementMinionProj>();
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			for (int i = 0; i < 1000; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Item.shoot)
					return false;
			}

            position = Main.MouseWorld;
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			player.AddBuff(Item.buffType, 2);
			SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GSummon"), player.position);
			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.

			return false;
		}


		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse != 2)
			{
				for (int i = 0; i < 1000; ++i)
				{
					if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Item.shoot)
					{
						Main.projectile[i].minionSlots += 1f;
						Main.projectile[i].originalDamage = Item.damage + (int)(4 * Main.projectile[i].minionSlots);
						if (Main.projectile[i].scale < 1.3f)
                        {
							Main.projectile[i].scale += .062f;
						}
							
					}
				}
			}
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 50);
			recipe.AddIngredient(ModContent.ItemType<MoltenScrap>(), 10);
			recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 5);
			recipe.AddTile(TileID.Hellforge);
			recipe.Register();
		}
	}
}