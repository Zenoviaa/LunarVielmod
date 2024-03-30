using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)] // Load the spritesheet you create as a shield for the player when it is equipped.
	public class ShadeScarf : ModItem
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
			Item.value = Item.sellPrice(gold: 10);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;


			Item.lifeRegen = 5;
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

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return !player.GetModPlayer<DashPlayer>().OneDashAccessoryEquipped;
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{

            player.GetModPlayer<DashPlayer>().OneDashAccessoryEquipped = true;
            player.GetModPlayer<DashPlayer2>().DashAccessoryEquipped = true;
			//	player.GetDamage(DamageClass.Generic) *= 0.95f;
			player.lifeRegen += 1;
			player.GetDamage(DamageClass.Generic) *= 1.02f;
			player.moveSpeed *= 1.3f;
			player.maxRunSpeed *= 1.3f;
			player.GetCritChance(DamageClass.Generic) *= 1.10f;
			player.statLifeMax2 += 10;

		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}

}