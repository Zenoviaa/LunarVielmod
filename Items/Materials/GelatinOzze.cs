using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;

namespace Stellamod.Items.Materials
{
    class GelatinOzze : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gelatin Ozze");
            // Tooltip.SetDefault("Jellyfish are scary... \n Trust me...");
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.value = 10000;
            Item.maxStack = 99;
        }
    }
}