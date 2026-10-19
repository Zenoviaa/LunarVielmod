using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;

namespace Stellamod.Helpers
{
    public static class PlayerHelper
    {
        public static long GetCoinValue(Item item)
        {
            long num = 0;
            if (item.type == ItemID.CopperCoin)
            {
                num += item.stack;
            }
            if (item.type == ItemID.SilverCoin)
            {
                num += (long)item.stack * 100L;
            }
            if (item.type == ItemID.GoldCoin)
            {
                num += (long)item.stack * 10000L;
            }
            if (item.type == ItemID.PlatinumCoin)
            {
                num += (long)item.stack * 1000000L;
            }
            return num;
        }
        public static long DropCoins(Player player, int drama)
        {
            //Edited from vanilla's source code
            var inventory = player.inventory;
            var itemSource_Death = player.GetItemSource_Death();
            long num = 0L;
            var copperCoins = 0;
            var silverCoins = 0;
            var goldCoins = 0;
            var platinumCoins = 0;

            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].IsACoin)
                {
                    int amountToDrop = inventory[i].stack;
                    inventory[i].stack -= amountToDrop;
                    if (inventory[i].type == ItemID.CopperCoin)
                    {
                        copperCoins += amountToDrop;
                 
                    }
                    if (inventory[i].type == ItemID.SilverCoin)
                    {
                        silverCoins+=amountToDrop;
                    }
                    if (inventory[i].type == ItemID.GoldCoin)
                    {
                        goldCoins+=amountToDrop;
                    }
                    if (inventory[i].type == ItemID.PlatinumCoin)
                    {
                        platinumCoins+=amountToDrop;
                    }
                }
            }

            void DropCoins( int wallet, int coinType)
            {
                var maxNumToDropPerStep = wallet / drama;
                if (wallet <= 0)
                    return;

                for(var i = 0; i < drama; i++)
                {
                    var numtoDrop = maxNumToDropPerStep;
                    if (numtoDrop > wallet)
                        numtoDrop = wallet;
               
                    int itemToSpawn = Item.NewItem(itemSource_Death, (int)player.position.X, (int)player.position.Y, player.width, player.height, coinType);
                    Main.item[itemToSpawn].stack = numtoDrop;
                    Main.item[itemToSpawn].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
                    Main.item[itemToSpawn].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
                    Main.item[itemToSpawn].noGrabDelay = 100;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemToSpawn);
                    }
                    wallet -= numtoDrop;
                }
            }
            DropCoins(copperCoins, ItemID.CopperCoin);
            DropCoins(silverCoins, ItemID.SilverCoin);
            DropCoins(goldCoins, ItemID.GoldCoin);
            DropCoins(platinumCoins, ItemID.PlatinumCoin);

            return num;
        }
        public static long DropCoins(Player player)
        {
            //Edited from vanilla's source code
            var inventory = player.inventory;
            var itemSource_Death = player.GetItemSource_Death();
            long num = 0L;
            for (int i = 0; i < 59; i++)
            {
                if (inventory[i].IsACoin)
                {
                    int itemToSpawn = Item.NewItem(itemSource_Death, (int)player.position.X, (int)player.position.Y, player.width, player.height, player.inventory[i].type);
                    int amountToDrop = inventory[i].stack / 2;
                    amountToDrop = inventory[i].stack - amountToDrop;
                    inventory[i].stack -= amountToDrop;
                    if (inventory[i].type == ItemID.CopperCoin)
                    {
                        num += amountToDrop;
                    }
                    if (inventory[i].type == ItemID.SilverCoin)
                    {
                        num += (long)amountToDrop * 100L;
                    }
                    if (inventory[i].type == ItemID.GoldCoin)
                    {
                        num += (long)amountToDrop * 10000L;
                    }
                    if (inventory[i].type == ItemID.PlatinumCoin)
                    {
                        num += (long)amountToDrop * 1000000L;
                    }
                    if (inventory[i].stack <= 0)
                    {
                        inventory[i] = new Item();
                    }
                    Main.item[itemToSpawn].stack = amountToDrop;
                    Main.item[itemToSpawn].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
                    Main.item[itemToSpawn].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
                    Main.item[itemToSpawn].noGrabDelay = 100;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemToSpawn);
                    }
                    if (i == 58)
                    {
                        Main.mouseItem = inventory[i].Clone();
                    }
                }
            }
            return num;
        }

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
