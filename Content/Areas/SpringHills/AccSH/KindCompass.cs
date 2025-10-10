using Stellamod.Core.XixianFlaskSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    public class KindCompass : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            FlaskPlayer flaskPlayer = player.GetModPlayer<FlaskPlayer>();
            flaskPlayer.maxInsourceCount += 1;
        }
    }
}
