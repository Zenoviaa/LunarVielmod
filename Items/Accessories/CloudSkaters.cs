using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            Item.rare = ItemRarityID.Green;
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

