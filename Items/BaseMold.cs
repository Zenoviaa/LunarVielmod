using Stellamod.Common;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items
{
    public abstract class BaseMold : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemSets.IsSoldBySirestias[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.shopSpecialCurrency = Stellamod.MedalCurrencyID;
            Item.shopCustomPrice = 5;
        }
    }
}
