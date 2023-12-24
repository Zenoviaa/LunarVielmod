using Terraria;

namespace Stellamod.Helpers
{
    internal static class PlayerHelper
    {
        public static bool RemoveItem(this Player player, int reqItem, int count = 1)
        {
            foreach (Item item in player.inventory)
            {
                if (item.type == reqItem)
                {
                    item.stack-= count;
                    return true;
                }
            }

            return false;
        }
        public static bool HasItemEquipped(this Player player, Item reqItem)
        {
            foreach (Item item in player.armor)
            {
                if (item == reqItem)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasItemEquipped(this Player player, int reqItem)
        {
            foreach (Item item in player.armor)
            {
                if (item.type == reqItem)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
