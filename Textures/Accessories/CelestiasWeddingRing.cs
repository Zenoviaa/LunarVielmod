using Microsoft.Xna.Framework;
using Stellamod.Brooches;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Weapons.Melee;
using Stellamod.NPCs.Town;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
	public class CelestiasWeddingRing : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Advanced Brooch Knapsack");
			/* Tooltip.SetDefault("Increased Regeneration!" +
				"\n +10% damage" +
				"\n Allows you to equip advanced brooches! (Very useful :P)" +
				"\n Allows the effects of the Hiker's Backpack! "); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "ADBPa", "She'll just love you forever I guess.")
			{
				OverrideColor = new Color(220, 87, 24)

			};
			tooltips.Add(line);
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 90);
			Item.rare = ItemRarityID.Pink;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<STARCORE>(), 1);
			recipe.AddIngredient(ModContent.ItemType<StolenMagicTome>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Bridget>(), 1);
			recipe.AddIngredient(ItemID.SoulofNight, 5);
			recipe.AddIngredient(ItemID.ObsidianShield, 1);
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 50);
			recipe.AddIngredient(ModContent.ItemType<GrailBar>(), 15);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 30);
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
			broochPlayer.hasCelestia = true;
			player.GetModPlayer<MyPlayer>().HikersBSpawn = true;
			player.noKnockback = true;
			player.lavaImmune = true;
			player.GetDamage(DamageClass.Generic) *= 1.12f; // Increase ALL player damage by 100%
			player.GetArmorPenetration(DamageClass.Generic) *= 1.12f; // Increase ALL player damage by 100%

			if (player.ownedProjectileCounts[ModContent.ProjectileType<Celestia>()] == 0)
			{

				Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, player.velocity * -1f,
					ModContent.ProjectileType<Celestia>(), 0, 1f, player.whoAmI);
			}
		}
	}
}