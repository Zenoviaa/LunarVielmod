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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Stellamod.Items.Materials
{
    public class VirulentPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Virulent Plating");
            // Tooltip.SetDefault("Radiating within the acid of entities of the tocins");
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Orange;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = 1;
        }
    }
}