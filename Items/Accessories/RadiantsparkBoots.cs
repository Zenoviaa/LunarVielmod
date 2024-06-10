using Microsoft.Xna.Framework;
using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
	[AutoloadEquip(EquipType.Waist)] // Load the spritesheet you create as a shield for the player when it is equipped.
	public class RadiantsparkBoots : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Steali");
			/* Tooltip.SetDefault("A small fast dash that provides invincibility as you dash" +
				"\nIncreased regeneration" +
				"\nYou may not attack while this is in use" +
				"\nHollow Knight inspiried!"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(platinum: 3);
			Item.rare = ModContent.RarityType<Helpers.GoldenSpecialRarity>();
			Item.accessory = true;


		
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<ShadeScarf>(), 1);
			recipe.AddIngredient(ModContent.ItemType<SoulStrideres>(), 1);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 15);
			recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 25);
			recipe.AddIngredient(ItemID.FrostsparkBoots, 1);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();


            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ModContent.ItemType<ShadeScarf>(), 1);
            recipe2.AddIngredient(ModContent.ItemType<SoulStrideres>(), 1);
            recipe2.AddIngredient(ModContent.ItemType<RippedFabric>(), 15);
            recipe2.AddIngredient(ModContent.ItemType<RadianuiBar>(), 25);
            recipe2.AddIngredient(ItemID.TerrasparkBoots, 1);
            recipe2.AddTile(TileID.TinkerersWorkbench);
            recipe2.Register();
        }

		public override bool CanEquipAccessory(Player player, int slot, bool modded)
		{
			return !player.GetModPlayer<DashPlayer>().OneDashAccessoryEquipped;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{

			player.GetModPlayer<DashPlayer>().OneDashAccessoryEquipped = true;
			player.GetModPlayer<DashPlayer3>().DashAccessoryEquipped = true;
			//	player.GetDamage(DamageClass.Generic) *= 0.95f;
			player.lifeRegen += 1;
			player.GetDamage(DamageClass.Generic) *= 1.05f;
			player.maxRunSpeed *= 1.4f;
			player.GetCritChance(DamageClass.Generic) *= 1.15f;
			player.statLifeMax2 += 30;
			player.moveSpeed += 0.8f;
			player.fairyBoots = true;
			player.lavaImmune = true;
		
			
			player.GetModPlayer<MyPlayer>().GIBomb = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<GIBomb>()] == 0)
			{
				Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
					ModContent.ProjectileType<GIBomb>(), 70, 4, player.whoAmI);
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}

}