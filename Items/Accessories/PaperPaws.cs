using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class PaperPaws : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Paper Paws");
			/* Tooltip.SetDefault("Why don't you give your summons more credit?" +
				"\n+1 summon"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = 200;
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{


			player.maxMinions += 1;


		}




	}
}