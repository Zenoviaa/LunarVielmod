using Stellamod.Common.ArmorRework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.AccShop
{
    public class TravelersBackpack : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToAccessory();
            Item.rare = ModContent.RarityType<ShopRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            ArmorStatsPlayer statsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            statsPlayer.inventorySlots += 30;
        }
    }
}
