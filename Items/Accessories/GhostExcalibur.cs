using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Stellamod.Items.Materials;
using Stellamod.Tiles;

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
            Item.rare = 1;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MyPlayer>().GHE = true;

        }
    }
}