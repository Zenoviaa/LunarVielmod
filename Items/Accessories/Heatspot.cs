using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Stellamod;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;

namespace Stellamod.Items.Accessories
{
	public class Heatspot : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Hiker's Backpack");
			/* Tooltip.SetDefault("Increased Regeneration!" +
				"\n +3% damage" +
				"\n Drops stumps on the ground as you walk! "); */

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
			player.GetDamage(DamageClass.Magic) *= 1.04f; // Increase ALL player damage by 100%


		}




	}
}