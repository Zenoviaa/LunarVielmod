using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class MOTT : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Medallion of the Tombs");
			/* Tooltip.SetDefault("Every 10 seconds drop high damage spiky balls on the ground" +
				"\n+5% Ranged damage..."); */

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
			player.GetModPlayer<MyPlayer>().TAuraSpawn = true;
			player.GetDamage(DamageClass.Ranged) += 0.05f; // Increase ALL player damage by 100%
			player.GetModPlayer<MyPlayer>().TAuraCooldown--;
		}
	}
}