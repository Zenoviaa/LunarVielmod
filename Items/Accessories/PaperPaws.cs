using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Stellamod;

namespace Stellamod.Items.Accessories
{
	public class PaperPaws : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Paper Paws");
			Tooltip.SetDefault("Why don't you give your summons more credit?" +
				"\n+2 summons");

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


			player.maxMinions += 2;


		}




	}
}