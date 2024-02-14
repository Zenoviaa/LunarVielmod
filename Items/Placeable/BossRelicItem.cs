using Stellamod.Tiles.Furniture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Stellamod.Items.Placeable
{
    public abstract class BossRelicItem : ModItem
    {
        public abstract int TileType { get; }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType);
            Item.value = 150;
            Item.maxStack = 20;
            Item.width = 30;
            Item.height = 44;

            Item.rare = ItemRarityID.Master;
            Item.master = true; // This makes sure that "Master" displays in the tooltip, as the rarity only changes the item name color
            Item.value = Item.buyPrice(0, 5);
        }
    }
}
