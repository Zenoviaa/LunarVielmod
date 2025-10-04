using Stellamod.Items.Armors;
using Stellamod.Items.Armors.AcidArmour;
using Stellamod.Items.Armors.Artisan;
using Stellamod.Items.Armors.Flower;
using Stellamod.Items.Armors.ForestCore;
using Stellamod.Items.Armors.Garbage;
using Stellamod.Items.Armors.HeavyMetal;
using Stellamod.Items.Armors.Illurian;
using Stellamod.Items.Armors.Jianxin;
using Stellamod.Items.Armors.Vextin;
using Stellamod.Items.Armors.Winterborn;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

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
            GintzeSet.AddBody(ModContent.ItemType<HeavyMetalBody>());
            GintzeSet.AddLegs(ModContent.ItemType<HeavyMetalLegs>());
            GintzeSet.SetMaterial(ModContent.ItemType<GintzlMetal>());
            GintzeSet.Register();

            //vextin
            ArmorShopSet Vext = new ArmorShopSet();
            Vext.AddHead(ModContent.ItemType<VextinMask>());
            Vext.AddBody(ModContent.ItemType<VextinRobe>());
            Vext.AddLegs(ModContent.ItemType<VextinBoots>());
            Vext.SetMaterial(ItemID.AntlionMandible);
            Vext.Register();




            //Virulent Armor
            ArmorShopSet Vir = new ArmorShopSet();
            Vir.AddHead(ModContent.ItemType<VirulentHelm>());
            Vir.AddBody(ModContent.ItemType<VirulentArmor>());
            Vir.AddLegs(ModContent.ItemType<VirulentLegs>());
            Vir.SetMaterial(ModContent.ItemType<VirulentPlating>());
            Vir.Register();

            //kaleido
            ArmorShopSet Paint = new ArmorShopSet();
            Paint.AddHead(ModContent.ItemType<ArtisanMask>());
            Paint.AddBody(ModContent.ItemType<ArtisanBreastplate>());
            Paint.AddLegs(ModContent.ItemType<ArtisanThighs>());
            Paint.SetMaterial(ModContent.ItemType<KaleidoscopicInk>());
            Paint.Register();

            //Illurian1
            ArmorShopSet Ill = new ArmorShopSet();
            Ill.AddHead(ModContent.ItemType<IllurianCrestmask>());
            Ill.AddBody(ModContent.ItemType<IllurianCrestplate>());
            Ill.AddLegs(ModContent.ItemType<IllurianCrestpants>());
            Ill.SetMaterial(ModContent.ItemType<IllurineScale>());
            Ill.Register();

            //Illurian2
            ArmorShopSet Ill2 = new ArmorShopSet();
            Ill2.AddHead(ModContent.ItemType<IllurianWarriorHelm>());
            Ill2.AddBody(ModContent.ItemType<IllurianWarriorChestplate>());
            Ill2.AddLegs(ModContent.ItemType<IllurianWarriorGreaves>());
            Ill2.SetMaterial(ModContent.ItemType<IllurineScale>());
            Ill2.Register();

            //radianui1
            ArmorShopSet rad1 = new ArmorShopSet();
            rad1.AddHead(ModContent.ItemType<FlowerHat>());
            rad1.AddBody(ModContent.ItemType<FlowerRobe>());
            rad1.AddLegs(ModContent.ItemType<FlowerPants>());
            rad1.SetMaterial(ModContent.ItemType<RadianuiBar>());
            rad1.Register();

            //radianui2
            ArmorShopSet rad2 = new ArmorShopSet();
            rad2.AddHead(ModContent.ItemType<GarbageMask>());
            rad2.AddBody(ModContent.ItemType<GarbageChestplate>());
            rad2.AddLegs(ModContent.ItemType<GarbagePants>());
            rad2.SetMaterial(ModContent.ItemType<RadianuiBar>());
            rad2.Register();


            //Jianxin
            ArmorShopSet JianxinSet = new ArmorShopSet();
            JianxinSet.AddHead(ModContent.ItemType<JianxinMask>());
            JianxinSet.AddBody(ModContent.ItemType<JianxinCoat>());
            JianxinSet.AddLegs(ModContent.ItemType<JianxinPants>());
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
