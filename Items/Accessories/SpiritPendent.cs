
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class SpiritPendent : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spirit Pendent");
        }
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(gold: 2);
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 12);
            Item.rare = ItemRarityID.Blue;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MyPlayer>().SpiritPendent = true;
            Lighting.AddLight(player.position, 1.0f, 1.0f, 2.75f);
        }
    }
}
