using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.ArmorShop
{
    public class ArmorShopSet
    {
        public ArmorShopSet()
        {
            heads = new List<Item>();
            bodies = new List<Item>();
            legs = new List<Item>();
        }

        public Item material;
        public List<Item> heads;
        public List<Item> bodies;
        public List<Item> legs;

        public void SetMaterial(int itemType)
        {
            Item item = new Item(itemType);
            material = item;
            material.stack = 30;
        }

        public void AddHead(int itemType)
        {
            Item item = new Item(itemType);
            heads.Add(item);
        }

        public void AddBody(int itemType)
        {
            Item item = new Item(itemType);
            bodies.Add(item);
        }

        public void AddLegs(int itemType)
        {
            Item item = new Item(itemType);
            legs.Add(item);
        }

        public bool IsInSet(Item item)
        {
            for (int i = 0; i < heads.Count; i++)
            {
                Item head = heads[i];
                if (head.type == item.type)
                    return true;
            }

            for (int i = 0; i < bodies.Count; i++)
            {
                Item body = bodies[i];
                if (body.type == item.type)
                    return true;
            }
            for (int i = 0; i < legs.Count; i++)
            {
                Item leg = legs[i];
                if (leg.type == item.type)
                    return true;
            }
            return false;
        }

        public bool HasPurchased()
        {
            ArmorShopPlayer armorShopPlayer = Main.LocalPlayer.GetModPlayer<ArmorShopPlayer>();
            for (int i = 0; i < heads.Count; i++)
            {
                Item head = heads[i];
                if (!armorShopPlayer.HasPurchased(head.type))
                    return false;
            }

            for (int i = 0; i < bodies.Count; i++)
            {
                Item body = bodies[i];
                if (!armorShopPlayer.HasPurchased(body.type))
                    return false;
            }
            for (int i = 0; i < legs.Count; i++)
            {
                Item leg = legs[i];
                if (!armorShopPlayer.HasPurchased(leg.type))
                    return false;
            }
            return true;
        }

        public void QuickSpawn(Player player)
        {
            var source = player.GetSource_FromThis();
            ArmorShopPlayer armorShopPlayer = player.GetModPlayer<ArmorShopPlayer>();
            for (int i = 0; i < heads.Count; i++)
            {
                Item head = heads[i];
                player.QuickSpawnItem(source, head.type);
                armorShopPlayer.PurchasedArmors.Add(head);
            }

            for (int i = 0; i < bodies.Count; i++)
            {
                Item body = bodies[i];
                player.QuickSpawnItem(source, body.type);
                armorShopPlayer.PurchasedArmors.Add(body);
            }
            for (int i = 0; i < legs.Count; i++)
            {
                Item leg = legs[i];
                player.QuickSpawnItem(source, legs[i].type);
                armorShopPlayer.PurchasedArmors.Add(leg);
            }
        }

        public void Register()
        {
            ModContent.GetInstance<ArmorShopGroups>().AddSet(this);
        }
    }
}
