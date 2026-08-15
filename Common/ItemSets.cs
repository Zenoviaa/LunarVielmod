using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common
{
    public class ItemSets : ModSystem
    {
        public override void ResizeArrays()
        {
            base.ResizeArrays();
            IsSoldBySirestias = ItemID.Sets.Factory.CreateBoolSet();
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
