using Stellamod.Items.Armors;
using Stellamod.Items.Armors.ForestCore;
using Stellamod.Items.Armors.HeavyMetal;
using Stellamod.Items.Armors.Jianxin;
using Stellamod.Items.Armors.Winterborn;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.ArmorShop
{
    internal class ArmorShopGroups : ModSystem
    {
        public List<ArmorShopSet> Armors;

        public override void PostSetupContent()
        {
            base.PostSetupContent();
            Armors = new List<ArmorShopSet>();


            //Ivythorn Set
            ArmorShopSet ivythornSet = new ArmorShopSet();
            ivythornSet.AddHead(ModContent.ItemType<ForestCoreHead>());
            ivythornSet.AddBody(ModContent.ItemType<ForestCoreBody>());
            ivythornSet.AddLegs(ModContent.ItemType<ForestCoreLegs>());
            ivythornSet.SetMaterial(ModContent.ItemType<Ivythorn>());
            ivythornSet.Register();

            //Winterborn Set
            ArmorShopSet winterbornSet = new ArmorShopSet();
            winterbornSet.AddHead(ModContent.ItemType<WinterbornHead>());
            winterbornSet.AddBody(ModContent.ItemType<WinterbornBody>());
            winterbornSet.AddLegs(ModContent.ItemType<WinterbornLegs>());
            winterbornSet.SetMaterial(ModContent.ItemType<WinterbornShard>());
            winterbornSet.Register();

            //Celestial Moon Set
            ArmorShopSet celestiaMoonSet = new ArmorShopSet();
            celestiaMoonSet.AddHead(ModContent.ItemType<CelestiaMoonHelmet>());
            celestiaMoonSet.AddHead(ModContent.ItemType<CelestiaMoonMask>());
            celestiaMoonSet.AddBody(ModContent.ItemType<CelestiaMoonBreastplate>());
            celestiaMoonSet.AddLegs(ModContent.ItemType<CelestiaMoonLegs>());
            celestiaMoonSet.SetMaterial(ModContent.ItemType<GlisteningBar>());
            celestiaMoonSet.Register();

            //Heavy metal/gitnzl
            ArmorShopSet GintzeSet = new ArmorShopSet();
            GintzeSet.AddHead(ModContent.ItemType<HeavyMetalHead>());
            GintzeSet.AddHead(ModContent.ItemType<HeavyMetalBody>());
            GintzeSet.AddBody(ModContent.ItemType<HeavyMetalLegs>());
            GintzeSet.SetMaterial(ModContent.ItemType<GintzlMetal>());
            GintzeSet.Register();

            //Jianxin
            ArmorShopSet JianxinSet = new ArmorShopSet();
            JianxinSet.AddHead(ModContent.ItemType<JianxinMask>());
            JianxinSet.AddHead(ModContent.ItemType<JianxinCoat>());
            JianxinSet.AddBody(ModContent.ItemType<JianxinPants>());
            JianxinSet.SetMaterial(ItemID.LunarBar);
            JianxinSet.Register();
        }

        public ArmorShopSet FindSet(Item item)
        {
            foreach(var armor in Armors)
            {
                if (armor.IsInSet(item))
                    return armor;
            }
            return null;
        }

        public void AddSet(ArmorShopSet armorShopSet)
        {
            Armors.Add(armorShopSet);
        }
    }
}
