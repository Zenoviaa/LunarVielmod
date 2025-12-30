using Stellamod.Content.Items.MoonlightMagic;
using System.Collections.Generic;

namespace Stellamod.Common.MagicSystem.UI
{
    public class EnchantmentComparer : IComparer<BaseEnchantment>
    {
        public int Compare(BaseEnchantment x, BaseEnchantment y)
        {
            int compareElement = x.GetElementType().CompareTo(y.GetElementType());
            if (compareElement == 0)
            {
                return x.DisplayName.Value.CompareTo(y.DisplayName.Value);
            }
            return compareElement;

        }
    }
}
