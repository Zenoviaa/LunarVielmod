using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Stellamod.Items.Accessories
{
    internal class GhostExcalibur : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rune of Detonation");
            // Tooltip.SetDefault("Has a chance to explode enemies doing double damage");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 2500;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MyPlayer>().GHE = true;

        }
    }
}