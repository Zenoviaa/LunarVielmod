using Stellamod.Common.XixianFlaskSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.LostItems
{
    public class InsourceCoolerPotion : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<FlaskPlayer>().insourceSecondsBonusPerInsource += 2;
        }
    }
}
