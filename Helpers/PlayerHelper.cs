using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Helpers
{
    public static class PlayerHelper
    {
        public static bool RemoveItem(this Player player, int reqItem, int count = 1)
        {
            int removedAmount = 0;
            foreach (Item item in player.inventory)
            {
                if (item.type == reqItem)
                {
                    while (item.stack > 0 && removedAmount < count)
                    {
                        item.stack--;
                        removedAmount++;
                    }


                }
            }
            if (removedAmount >= count)
                return true;
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

        public static Player FindClosestPlayer(Vector2 position, float maxDetectDistance)
        {
            Player closestPlayer = null;
            float closestDistance = float.MaxValue;
            foreach (var player in Main.ActivePlayers)
            {
                float distance = Vector2.Distance(position, player.Center);
                if(distance < closestDistance)
                {
                    closestPlayer = player;
                    closestDistance = distance;
                }
            }
          

            return closestPlayer;
        }
    }
}
