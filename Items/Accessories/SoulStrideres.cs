using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories

{
    public class SoulStrideres : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 12);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!player.ZoneSkyHeight || player.wet)
            {
                player.gravDir -= 0.05f;
                player.gravity -= 0.05f;
            }
            player.desertBoots = true;
            player.rocketBoots = 5;
            player.moveSpeed += 0.5f;
        }
    }
}

