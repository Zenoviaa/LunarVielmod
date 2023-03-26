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
	public class MOTT : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Medallion of the Tombs");
			Tooltip.SetDefault("Every 10 seconds drop high damage spiky balls on the ground" +
				"\n+20% Ranged damage..." +
				"\n But at the cost of all your defense :( ");

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
			player.GetModPlayer<MyPlayer>().TAuraSpawn = true;
			player.GetDamage(DamageClass.Ranged) *= 1.2f; // Increase ALL player damage by 100%
			player.GetModPlayer<MyPlayer>().TAuraCooldown--;
			player.statDefense -= 400;


		}




	}
}