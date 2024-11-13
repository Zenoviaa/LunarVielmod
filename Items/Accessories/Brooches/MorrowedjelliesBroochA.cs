using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class MorrowedJelliesBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 0, 90);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
            Item.buffType = ModContent.BuffType<JellyB>();
            Item.defense = 20;
        }
    }
}