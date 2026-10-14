using Stellamod.Common.StaminaSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.AccShop
{
    public class TrainingScissors : ModItem
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
            StaminaNDashPlayer dashPlayer = player.GetModPlayer<StaminaNDashPlayer>();
            dashPlayer.ExtraImmunityFramesBonus += 3;
        }
    }
}
