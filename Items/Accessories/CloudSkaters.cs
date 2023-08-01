using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Accessories
{
    public class CloudSkaters : ModItem
    {    
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cloud Skaters");
            // Tooltip.SetDefault("Decreases falling Speed by 9%");
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 12);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!player.ZoneSkyHeight)
            {
                player.gravDir -= 0.2f;
                player.gravity -= 0.2f;
            }

        }
    }
    }

