using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class PlantH : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Plant Heritage");
			/* Tooltip.SetDefault("Gain the power of plants!" +
				"\n+40 life and +15 increased defense" +
				"\nBut you have decreased speed "); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(silver: 12);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{

			player.GetModPlayer<MyPlayer>().PlantH = true;	
			player.statDefense += 15;
			player.statLifeMax2 += 40;
			player.maxRunSpeed *= 0.40f;
		}
	}
}