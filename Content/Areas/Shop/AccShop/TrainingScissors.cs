using Stellamod.Items.Accessories.Players;
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
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.ExtraImmunityFramesBonus += 3;
        }
    }
}
