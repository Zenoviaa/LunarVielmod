using Stellamod.Content.Areas.WondrousDarkspace.ArmorWD;
using Stellamod.Content.Armors.ForestCore;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Items.Armors;
using Stellamod.Items.Armors.AcidArmour;
using Stellamod.Items.Armors.Appretience;
using Stellamod.Items.Armors.Artisan;
using Stellamod.Items.Armors.Astrasilk;
using Stellamod.Items.Armors.Daeden;
using Stellamod.Items.Armors.Ducanblitz;
using Stellamod.Items.Armors.Elagent;
using Stellamod.Items.Armors.Eldritchian;
using Stellamod.Items.Armors.Flower;
using Stellamod.Items.Armors.Garbage;
using Stellamod.Items.Armors.Govheil;
using Stellamod.Items.Armors.HeavyMetal;
using Stellamod.Items.Armors.Huntrian;
using Stellamod.Items.Armors.Illurian;
using Stellamod.Items.Armors.Jianxin;
using Stellamod.Items.Armors.Leather;
using Stellamod.Items.Armors.Lovestruck;
using Stellamod.Items.Armors.LunarianVoid;
using Stellamod.Items.Armors.Scissorian;
using Stellamod.Items.Armors.Scrappy;
using Stellamod.Items.Armors.ShadeWraith;
using Stellamod.Items.Armors.Staffigy;
using Stellamod.Items.Armors.Terric;
using Stellamod.Items.Armors.Ulven;
using Stellamod.Items.Armors.Verl;
using Stellamod.Items.Armors.Vextin;
using Stellamod.Items.Armors.Winterborn;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.ArmorShop
{
    public class ArmorShopGroups : ModSystem
    {
        public List<ArmorShopSet> Armors;

        public override void PostSetupContent()
        {
            base.PostSetupContent();
            Armors = new List<ArmorShopSet>();


            //Ivythorn Set done
            ArmorShopSet ivythornSet = new ArmorShopSet();
            ivythornSet.AddHead(ModContent.ItemType<ForestCoreHead>());
            ivythornSet.AddBody(ModContent.ItemType<ForestCoreBody>());
            ivythornSet.AddLegs(ModContent.ItemType<ForestCoreLegs>());
            ivythornSet.SetMaterial(ModContent.ItemType<Ivythorn>());
            ivythornSet.Register();

            //leather done
            ArmorShopSet leth = new ArmorShopSet();
            leth.AddHead(ModContent.ItemType<LeatherHead>());
            leth.AddBody(ModContent.ItemType<LeatherBody>());
            leth.AddLegs(ModContent.ItemType<LeatherLegs>());
            leth.SetMaterial(ItemID.Leather);
            leth.Register();

            //Winterborn Set done
            ArmorShopSet winterbornSet = new ArmorShopSet();
            winterbornSet.AddHead(ModContent.ItemType<WinterbornHead>());
            winterbornSet.AddBody(ModContent.ItemType<WinterbornBody>());
            winterbornSet.AddLegs(ModContent.ItemType<WinterbornLegs>());
            winterbornSet.SetMaterial(ModContent.ItemType<WinterbornShard>());
            winterbornSet.Register();

            //Celestial Moon Set done
            ArmorShopSet celestiaMoonSet = new ArmorShopSet();
            celestiaMoonSet.AddHead(ModContent.ItemType<CelestiaMoonHelmet>());
            celestiaMoonSet.AddHead(ModContent.ItemType<CelestiaMoonMask>());
            celestiaMoonSet.AddBody(ModContent.ItemType<CelestiaMoonBreastplate>());
            celestiaMoonSet.AddLegs(ModContent.ItemType<CelestiaMoonLegs>());
            celestiaMoonSet.SetMaterial(ModContent.ItemType<GlisteningOre>());
            celestiaMoonSet.Register();

            //Shadewrath done
            ArmorShopSet SW = new ArmorShopSet();
            SW.AddHead(ModContent.ItemType<ShadeWraithHead>());
            SW.AddBody(ModContent.ItemType<ShadeWraithBody>());
            SW.AddLegs(ModContent.ItemType<ShadeWraithLegs>());
            SW.SetMaterial(ItemID.GraniteBlock);
            SW.Register();

            //Scissorian done
            ArmorShopSet Ss = new ArmorShopSet();
            Ss.AddHead(ModContent.ItemType<ScissorianMask>());
            Ss.AddBody(ModContent.ItemType<ScrappyBody>());
            Ss.AddLegs(ModContent.ItemType<ScissorianGreaves>());
            Ss.SetMaterial(ModContent.ItemType<AuroreanStarI>());
            Ss.Register();

            //lovestruck done
            ArmorShopSet los = new ArmorShopSet();
            los.AddHead(ModContent.ItemType<LovestruckMask>());
            los.AddBody(ModContent.ItemType<LovestruckBreastplate>());
            los.AddLegs(ModContent.ItemType<LovestruckLegs>());
            los.SetMaterial(ModContent.ItemType<AuroreanStarI>());
            los.Register();

            //Astrasilk done
            ArmorShopSet astr = new ArmorShopSet();
            astr.AddHead(ModContent.ItemType<AstrasilkHead>());
            astr.AddBody(ModContent.ItemType<AstrasilkBody>());
            astr.AddLegs(ModContent.ItemType<AstrasilkLegs>());
            astr.SetMaterial(ModContent.ItemType<AuroreanStarI>());
            astr.Register();


            //Heavy metal/gitnzl done
            ArmorShopSet GintzeSet = new ArmorShopSet();
            GintzeSet.AddHead(ModContent.ItemType<HeavyMetalHead>());
            GintzeSet.AddBody(ModContent.ItemType<HeavyMetalBody>());
            GintzeSet.AddLegs(ModContent.ItemType<HeavyMetalLegs>());
            GintzeSet.SetMaterial(ModContent.ItemType<GintzlMetal>());
            GintzeSet.Register();

            //dread/teric done
            ArmorShopSet terr = new ArmorShopSet();
            terr.AddHead(ModContent.ItemType<TerricHead>());
            terr.AddBody(ModContent.ItemType<TerricBody>());
            terr.AddLegs(ModContent.ItemType<TerricLegs>());
            terr.SetMaterial(ModContent.ItemType<TerrorFragments>());
            terr.Register();

            //Daedia done
            ArmorShopSet Daedia = new ArmorShopSet();
            Daedia.AddHead(ModContent.ItemType<DaediaMask>());
            Daedia.AddBody(ModContent.ItemType<DaediaBreastplate>());
            Daedia.AddLegs(ModContent.ItemType<DaediaThighs>());
            Daedia.SetMaterial(ModContent.ItemType<HypnotizedSoul>());
            Daedia.Register();

            //Staffigy done
            ArmorShopSet staff = new ArmorShopSet();
            staff.AddHead(ModContent.ItemType<StaffigyHat>());
            staff.AddBody(ModContent.ItemType<StaffigyRobe>());
            staff.AddLegs(ModContent.ItemType<StaffigyPants>());
            staff.SetMaterial(ModContent.ItemType<HypnotizedSoul>());
            staff.Register();

            //vextin done
            ArmorShopSet Vext = new ArmorShopSet();
            Vext.AddHead(ModContent.ItemType<VextinMask>());
            Vext.AddBody(ModContent.ItemType<VextinRobe>());
            Vext.AddLegs(ModContent.ItemType<VextinBoots>());
            Vext.SetMaterial(ItemID.AntlionMandible);
            Vext.Register();

            //huntrian done
            ArmorShopSet hunt = new ArmorShopSet();
            hunt.AddHead(ModContent.ItemType<HuntrianHelmet>());
            hunt.AddBody(ModContent.ItemType<HuntrianChestplate>());
            hunt.AddLegs(ModContent.ItemType<HuntrianBoots>());
            hunt.SetMaterial(ItemID.Stinger);
            hunt.Register();

            //---------- Late Prehm

            //LVoid armor done
            ArmorShopSet Luvo = new ArmorShopSet();
            Luvo.AddHead(ModContent.ItemType<LunarianVoidHead>());
            Luvo.AddBody(ModContent.ItemType<LunarianVoidBody>());
            Luvo.AddLegs(ModContent.ItemType<LunarianVoidLegs>());
            Luvo.SetMaterial(ModContent.ItemType<ConvulgingMater>());
            Luvo.Register();


            //Verl done
            ArmorShopSet Verl = new ArmorShopSet();
            Verl.AddHead(ModContent.ItemType<VerlHat>());
            Verl.AddHead(ModContent.ItemType<VerlMask>());
            Verl.AddBody(ModContent.ItemType<VerlBreastplate>());
            Verl.AddLegs(ModContent.ItemType<VerlLeggings>());
            Verl.SetMaterial(ModContent.ItemType<PearlescentScrap>());
            Verl.Register();


            //Elegant done
            ArmorShopSet Ele = new ArmorShopSet();
            Ele.AddHead(ModContent.ItemType<ElagentHead>());
            Ele.AddBody(ModContent.ItemType<ElagentBody>());
            Ele.AddLegs(ModContent.ItemType<ElagentLegs>());
            Ele.SetMaterial(ItemID.Feather);
            Ele.Register();



            //---------- Hardmode

            //Virulent Armor done
            ArmorShopSet Vir = new ArmorShopSet();
            Vir.AddHead(ModContent.ItemType<VirulentHelm>());
            Vir.AddBody(ModContent.ItemType<VirulentArmor>());
            Vir.AddLegs(ModContent.ItemType<VirulentLegs>());
            Vir.SetMaterial(ModContent.ItemType<VirulentPlating>());
            Vir.Register();


            //kaleido done
            ArmorShopSet Paint = new ArmorShopSet();
            Paint.AddHead(ModContent.ItemType<ArtisanMask>());
            Paint.AddBody(ModContent.ItemType<ArtisanBreastplate>());
            Paint.AddLegs(ModContent.ItemType<ArtisanThighs>());
            Paint.SetMaterial(ModContent.ItemType<KaleidoscopicInk>());
            Paint.Register();

            //---------- Post mech


            //Scarppy done
            ArmorShopSet SCP = new ArmorShopSet();
            SCP.AddHead(ModContent.ItemType<ScrappyHead>());
            SCP.AddBody(ModContent.ItemType<ScrappyBody>());
            SCP.AddLegs(ModContent.ItemType<ScrappyLegs>());
            SCP.SetMaterial(ItemID.HallowedBar);
            SCP.Register();

            //Govheil done
            ArmorShopSet Gov1 = new ArmorShopSet();
            Gov1.AddHead(ModContent.ItemType<GovheilHelmet>());
            Gov1.AddBody(ModContent.ItemType<GovheilChainplate>());
            Gov1.AddLegs(ModContent.ItemType<GovheilThighs>());
            Gov1.SetMaterial(ItemID.HallowedBar);
            Gov1.Register();


            //Govheil 2 done
            ArmorShopSet Gov2 = new ArmorShopSet();
            Gov2.AddHead(ModContent.ItemType<GovheilMask>());
            Gov2.AddBody(ModContent.ItemType<GovheilBreastplate>());
            Gov2.AddLegs(ModContent.ItemType<GovheilThighs>());
            Gov2.SetMaterial(ItemID.HallowedBar);
            Gov2.Register();


            //---------- Chlorophyte

            //Daeden
            ArmorShopSet dae = new ArmorShopSet();
            dae.AddHead(ModContent.ItemType<DaedenMask>());
            dae.AddBody(ModContent.ItemType<DaedenChestplate>());
            dae.AddLegs(ModContent.ItemType<DaedenLegs>());
            dae.SetMaterial(ItemID.ChlorophyteBar);
            dae.Register();


            //Ulven
            ArmorShopSet Ulv = new ArmorShopSet();
            Ulv.AddHead(ModContent.ItemType<UlvenHelmet>());
            Ulv.AddBody(ModContent.ItemType<UlvenChestplate>());
            Ulv.AddLegs(ModContent.ItemType<UlvenGreaves>());
            Ulv.SetMaterial(ItemID.ChlorophyteBar);
            Ulv.Register();

            //Appretience
            ArmorShopSet Appre = new ArmorShopSet();
            Appre.AddHead(ModContent.ItemType<AppretienceHat>());
            Appre.AddBody(ModContent.ItemType<AppretienceBreastplate>());
            Appre.AddLegs(ModContent.ItemType<AppretiencePants>());
            Appre.SetMaterial(ItemID.ChlorophyteBar);
            Appre.Register();

            //---------- Post plant

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

            //---------- Post ml


            //Eldrit
            ArmorShopSet eldritch = new ArmorShopSet();
            eldritch.AddHead(ModContent.ItemType<EldritchianHood>());
            eldritch.AddBody(ModContent.ItemType<EldritchianCloak>());
            eldritch.AddLegs(ModContent.ItemType<EldritchianLegs>());
            eldritch.SetMaterial(ItemID.LunarBar);
            eldritch.Register();

            //Ducanbltize done 
            ArmorShopSet ducan = new ArmorShopSet();
            ducan.AddHead(ModContent.ItemType<DucanblitzCap>());
            ducan.AddBody(ModContent.ItemType<DucanblitzBreastplate>());
            ducan.AddLegs(ModContent.ItemType<DucanblitzThighs>());
            ducan.SetMaterial(ItemID.LunarBar);
            ducan.Register();

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
            foreach (var armor in Armors)
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
