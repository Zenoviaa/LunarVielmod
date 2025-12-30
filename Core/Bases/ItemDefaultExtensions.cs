using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public static class ItemDefaultExtensions
    {
        public static void DefaultToSafunai(this Item item)
        {
            SafunaiGlobalItem globalItem = item.GetGlobalItem<SafunaiGlobalItem>();
            globalItem.isSafunai = true;
            item.DamageType = DamageClass.Melee;
        }
    }
}
