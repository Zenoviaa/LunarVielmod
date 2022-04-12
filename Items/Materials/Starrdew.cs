using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    internal class Starrdew : ModItem
    {


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starr Dew");
            Tooltip.SetDefault("Ew! Its sticky! I wonder what else is sticky..." +
            "\nA sticky substance that resonates with the stars or the morrow!");





        }

        public override void SetDefaults()
        {

            Item.width = 20;
            Item.height = 20;

            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 50);
        }
    }


}
