using Stellamod.Content.Items.MoonlightMagic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.AccShop
{
    public class TheWorld : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToAccessory();

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            AdvancedMagicPlayer magicPlayer = player.GetModPlayer<AdvancedMagicPlayer>();
            magicPlayer.chargeTimeBonus += 0.75f;
            magicPlayer.chargeDamagePenalty += 0.75f;
        }
    }
}
