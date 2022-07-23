using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Stellamod;

namespace Stellamod.Items.Accessories
{
	public class PlantH : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plant Heritage");
			Tooltip.SetDefault("Gain the power of plants!" +
				"\n+40 life and +15 increased defense" +
				"\n -15% Movement ");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(10);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;


		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{


			player.maxRunSpeed -= 0.15f;
			player.statDefense += 15;
			player.statLifeMax += 40;


		}




	}
}