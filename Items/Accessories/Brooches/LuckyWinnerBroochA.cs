using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class LuckyWinnerBroochA : BaseBrooch
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
            Item.buffType = ModContent.BuffType<LuckyB>();
            Item.accessory = true;
            BroochType = BroochType.Advanced;
        }
    }
}