﻿using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)] // Load the spritesheet you create as a shield for the player when it is equipped.
	public class ShadeScarf : BaseDashItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 10);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Steali>(), 1);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 15);
			recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 5);
			recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 10);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			base.UpdateAccessory(player, hideVisual);
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.DashVelocity += 7;
			player.moveSpeed *= 1.3f;
			player.maxRunSpeed *= 1.3f;
			player.statLifeMax2 += 10;
		}
	}
}