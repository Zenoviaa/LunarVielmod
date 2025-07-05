using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Test
{
    internal class ExtraBlockRange : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.tileBoost += 10000;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.blockRange += 10000;

        }
    }
}
