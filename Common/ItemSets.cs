using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common
{
    public class ItemSets : ModSystem
    {
        public override void SetupContent()
        {
            IsSoldBySirestias = ItemID.Sets.Factory.CreateBoolSet();
            base.SetupContent();

        }
        public static bool[] IsSoldBySirestias;
    }

    public static class ItemSetsExtensions
    {
        public static void AddToSirestiasShop(this Item item)
        {
            ItemSets.IsSoldBySirestias[item.type] = true;
        }
    }
}
