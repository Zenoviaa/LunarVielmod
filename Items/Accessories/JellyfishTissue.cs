using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class JellyfishTissue : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Jellyfish Tissue");
			/* Tooltip.SetDefault("Woa really cool!" +
				"\n+40 Defense" +
				"\n But at the cost of 40% damage "); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 10);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;


		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{

			player.GetDamage(DamageClass.Generic) *= 0.5f; // Increase ALL player damage by 100%
			player.statDefense += 40;


		}




	}
}