using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class GintzlBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 2, 50);
            Item.rare = ItemRarityID.Green;
            Item.buffType = ModContent.BuffType<GintBroo>();
            Item.accessory = true;
        }
    }
}