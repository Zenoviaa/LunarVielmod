using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class GovhenShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wooden Shield");
        }
        public override void SetDefaults()
        {
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 12);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 8;
        }
    }
}