using Stellamod.Core.DashSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Accessories
{
    internal class HeartPiece : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 30;
            Item.height = 40;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.MaxDashCount += 2;
        }
    }
}
