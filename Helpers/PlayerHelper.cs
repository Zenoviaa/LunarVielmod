using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.UI;

namespace Stellamod.Helpers
{
    public static class PlayerHelper
    {
        public static long CountCurrencyInAllBanks(Player player, int currencyID)
        {
            CustomCurrencyManager.TryGetCurrencySystem(currencyID, out CustomCurrencySystem system);
            bool overflowing = false;
            long num = system.CountCurrency(out overflowing, player.inventory);
            long num2 = system.CountCurrency(out overflowing, player.bank.item);
            long num3 = system.CountCurrency(out overflowing, player.bank2.item);
            long num4 = system.CountCurrency(out overflowing, player.bank3.item);
            long num5 = system.CountCurrency(out overflowing, player.bank4.item);
            long num6 = num + num2 + num3 + num4 + num5;
            return num6;
        }
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
