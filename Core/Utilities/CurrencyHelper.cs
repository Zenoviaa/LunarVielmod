using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Stellamod.Core.Utilities
{
    public struct Wallet
    {
        public int copperCoins;
        public int silverCoins;
        public int goldCoins;
        public int platinumCoins;
        public int GetValue()
        {
            return copperCoins + silverCoins * 100 + goldCoins * 100 * 100 + platinumCoins * 100 * 100 * 100;
        }
    }
    public static class CurrencyHelper
    {


        public static void CountCoins(Item[] inv, ref Wallet wallet)
        {
            for (int i = 0; i < inv.Length; i++)
            {
                int itemType = inv[i].type;
                if (itemType == ItemID.CopperCoin)
                    wallet.copperCoins += inv[i].stack;
                else if (itemType == ItemID.SilverCoin)
                    wallet.silverCoins += inv[i].stack;
                else if (itemType == ItemID.GoldCoin)
                    wallet.goldCoins += inv[i].stack;
                else if (itemType == ItemID.PlatinumCoin)
                    wallet.platinumCoins += inv[i].stack;
            }
        }

        public static void CountCoins(Player player, out Wallet wallet)
        {
            wallet = new Wallet();
            CountCoins(player.inventory, ref wallet);
            CountCoins(player.bank.item, ref wallet);
            CountCoins(player.bank2.item, ref wallet);
            CountCoins(player.bank3.item, ref wallet);
            CountCoins(player.bank4.item, ref wallet);
        }
    }
}
