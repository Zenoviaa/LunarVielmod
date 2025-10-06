using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.ArmorShop
{
    internal class ArmorShopPlayer : ModPlayer
    {
        private List<Item> _purchasedArmors;
        public List<Item> PurchasedArmors
        {
            get
            {
                _purchasedArmors ??= new List<Item>();
                return _purchasedArmors;
            }
            set
            {
                _purchasedArmors = value;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["purchasedArmors"] = PurchasedArmors;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            PurchasedArmors = tag.Get<List<Item>>("purchasedArmors");
        }

        public bool HasPurchased(int itemType)
        {
            return PurchasedArmors.Find(x => x.type == itemType) != null;
        }
    }
}
