using Stellamod.Core.BackpackSystem;
using Stellamod.Items.Accessories.Players;
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
            BackpackPlayer backpackPlayer = player.GetModPlayer<BackpackPlayer>();
            backpackPlayer.hasBackpack = true;
        }
        public override void UpdateVanity(Player player)
        {
            base.UpdateVanity(player);
            BackpackPlayer backpackPlayer = player.GetModPlayer<BackpackPlayer>();
            backpackPlayer.hasBackpack = true;
        }
    }
}
