
using Stellamod.Buffs;
using Stellamod.Common.QuestSystem;
using Stellamod.Common.QuestSystem.Quests;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Accessories.Catacombs;
using Stellamod.Items.Accessories.Igniter;
using Stellamod.Items.Accessories.Runes;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Mage.Stein;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Melee.Greatswords;
using Stellamod.Items.Weapons.Melee.Knives;
using Stellamod.Items.Weapons.Melee.Safunais;
using Stellamod.Items.Weapons.Melee.Shields;
using Stellamod.Items.Weapons.Melee.Spears;
using Stellamod.Items.Weapons.Melee.Swords;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.Crossbows;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Summon.Orbs;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.Items.Weapons.Thrown.Jugglers;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Stellamod.Items
{
    internal class CauldronPlayer : ModPlayer
    {
        public float NothingFailChance;
        public float InkFailChance;
        public List<Item> Crafts = new List<Item>();
        public int CrystalStarCount;
        public override void ResetEffects()
        {
            base.ResetEffects();

            //So I'm thinking we just have these variables
            NothingFailChance = 15;
            InkFailChance = 25;
        }

        public override void PostUpdate()
        {
            base.PostUpdate();
            if (CrystalStarCount > 0)
            {
                Player.AddBuff(ModContent.BuffType<CrystalLuck>(), 2);
            }
        }

        public void Make(Item item)
        {
            //Add the template instance
            if (!Crafts.Contains(item))
                Crafts.Add(item);
        }

        public bool HasMadeItem(Item item)
        {
            return Crafts.Find(x => x.type == item.type) != null;
        }

        public bool HasMadeItem(int itemType)
        {
            return Crafts.Find(x => x.type == itemType) != null;
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag.Add("crafts", Crafts);
            tag.Add("crystalStars", CrystalStarCount);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            Crafts = tag.Get<List<Item>>("crafts");
            CrystalStarCount = tag.Get<int>("crystalStars");
        }
    }

    internal class CauldronBrew
    {
        public int result;
        public int mold;
        public int material;
        public int materialAmount;
        public float weight = 1.0f;
        public int yield = 1;
    }

    internal class Cauldron : ModSystem
    {
        private List<CauldronBrew> _brews;
        public CauldronBrew InkBrew
        {
            get
            {
                CauldronBrew brew = new CauldronBrew();
                brew.result = ModContent.ItemType<KaleidoscopicInk>();
                return brew;
            }
        }

        public CauldronBrew NothingBrew
        {
            get
            {
                CauldronBrew brew = new CauldronBrew();
                brew.result = -1;
                return brew;
            }
        }

        public static event Action<CauldronBrew> OnBrew;
        public CauldronBrew JustCrafted { get; set; }
        public override void PostAddRecipes()
        {
            base.PostAddRecipes();
            _brews = new List<CauldronBrew>();


            //Define all the combos here
            //Using Aurorean Starball for testing this system



            //Spring Mushroom x Bow
            AddBrew(
                result: ModContent.ItemType<MushroomGreatbow>(),
                mold: ModContent.ItemType<BlankBow>(),
                material: ModContent.ItemType<Mushroom>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Spring Mushroom x Juggler
            AddBrew(
                result: ModContent.ItemType<DirtGlove>(),
                mold: ModContent.ItemType<BlankJuggler>(),
                material: ModContent.ItemType<Mushroom>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
              result: ModContent.ItemType<BasicBaseball>(),
              mold: ModContent.ItemType<BlankJuggler>(),
              material: ModContent.ItemType<Mushroom>(),
              materialCount: 10,
              weight: 1.0f,
              yield: 1);



            //Spring Mushroom x Staff
            AddBrew(
                result: ModContent.ItemType<PotionOfManaWand>(),
                mold: ModContent.ItemType<BlankStaff>(),
                material: ModContent.ItemType<Mushroom>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
               result: ModContent.ItemType<PotionOfLifeWand>(),
               mold: ModContent.ItemType<BlankStaff>(),
               material: ModContent.ItemType<Mushroom>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            AddBrew(
               result: ModContent.ItemType<MushroomStave>(),
               mold: ModContent.ItemType<BlankStaff>(),
               material: ModContent.ItemType<Mushroom>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            //Spring Mushroom x Accessory
            AddBrew(
                result: ModContent.ItemType<LeatherGlove>(),
                mold: ModContent.ItemType<BlankAccessory>(),
                material: ModContent.ItemType<Mushroom>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<MushyExtenderPowder>(),
                mold: ModContent.ItemType<BlankAccessory>(),
                material: ModContent.ItemType<Mushroom>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);


            //Spring Mushroom x Card
            AddBrew(
                result: ModContent.ItemType<MushyCard>(),
                mold: ModContent.ItemType<BlankCard>(),
                material: ModContent.ItemType<Mushroom>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<StarterCard>(),
                mold: ModContent.ItemType<BlankCard>(),
                material: ModContent.ItemType<Mushroom>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Spring Mushroom x Bag
            AddBrew(
                result: ModContent.ItemType<MushyPowder>(),
                mold: ModContent.ItemType<BlankBag>(),
                material: ModContent.ItemType<Mushroom>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            /*
            AddBrew(
                result: ModContent.ItemType<CoalDust>(),
                mold: ModContent.ItemType<BlankBag>(),
                material: ModContent.ItemType<Mushroom>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);*/

            //Spring Mushroom x Brooch
            AddBrew(
                result: ModContent.ItemType<AmethystBroochA>(),
                mold: ModContent.ItemType<BlankBrooch>(),
                material: ModContent.ItemType<Mushroom>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Ivythorn x Sword
            AddBrew(
                result: ModContent.ItemType<WoodenSaber>(),
                mold: ModContent.ItemType<BlankSword>(),
                material: ModContent.ItemType<Ivythorn>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
               result: ModContent.ItemType<EYIgniter>(),
               mold: ModContent.ItemType<BlankCard>(),
               material: ModContent.ItemType<Ivythorn>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            //Ivythorn x Bow
            AddBrew(
                result: ModContent.ItemType<IvynShot>(),
                mold: ModContent.ItemType<BlankBow>(),
                material: ModContent.ItemType<Ivythorn>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Ivythorn x Juggler
            AddBrew(
                result: ModContent.ItemType<IvythornShuriken>(),
                mold: ModContent.ItemType<BlankJuggler>(),
                material: ModContent.ItemType<Ivythorn>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Ivythorn x Staff
            AddBrew(
                result: ModContent.ItemType<IvyakenStaff>(),
                mold: ModContent.ItemType<BlankStaff>(),
                material: ModContent.ItemType<Ivythorn>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Ivythorn x Accessory
            AddBrew(
                result: ModContent.ItemType<HikersBackpack>(),
                mold: ModContent.ItemType<BlankAccessory>(),
                material: ModContent.ItemType<Ivythorn>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
               result: ModContent.ItemType<IllusionistBook>(),
               mold: ModContent.ItemType<BlankAccessory>(),
               material: ModContent.ItemType<Ivythorn>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            //Ivythorn x Bag
            AddBrew(
              result: ModContent.ItemType<GrassDirtPowder>(),
              mold: ModContent.ItemType<BlankBag>(),
              material: ModContent.ItemType<Ivythorn>(),
              materialCount: 10,
              weight: 1.0f,
              yield: 1);

            //Ivythorn x Rune
            AddBrew(
              result: ModContent.ItemType<RuneOfCorsage>(),
              mold: ModContent.ItemType<BlankRune>(),
              material: ModContent.ItemType<Ivythorn>(),
              materialCount: 10,
              weight: 1.0f,
              yield: 1);

            //Ivythorn x Shield
            AddBrew(
              result: ModContent.ItemType<WoodShield>(),
              mold: ModContent.ItemType<BlankShield>(),
              material: ModContent.ItemType<Ivythorn>(),
              materialCount: 10,
              weight: 1.0f,
              yield: 1);

            //Ivythorn x Brooch
            AddBrew(
              result: ModContent.ItemType<SlimeBroochA>(),
              mold: ModContent.ItemType<BlankBrooch>(),
              material: ModContent.ItemType<Ivythorn>(),
              materialCount: 10,
              weight: 1.0f,
              yield: 1);

            //Ivythorn x Orb
            AddBrew(
              result: ModContent.ItemType<CanOfLeaves>(),
              mold: ModContent.ItemType<BlankOrb>(),
              material: ModContent.ItemType<Ivythorn>(),
              materialCount: 10,
              weight: 1.0f,
              yield: 1);

            AddBrew(
                result: ModContent.ItemType<AuroreanStarball>(),
                mold: ModContent.ItemType<BlankOrb>(),
                material: ModContent.ItemType<AuroreanStarI>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Fable Scrap x Blank Sword
            AddBrew(
                result: ModContent.ItemType<MorrowSword>(),
                mold: ModContent.ItemType<BlankSword>(),
                material: ModContent.ItemType<AlcadizScrap>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<MorrowRapier>(),
                mold: ModContent.ItemType<BlankSword>(),
                material: ModContent.ItemType<AlcadizScrap>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<MorrowValswa>(),
                mold: ModContent.ItemType<BlankSword>(),
                material: ModContent.ItemType<AlcadizScrap>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<LightSpand>(),
                mold: ModContent.ItemType<BlankSword>(),
                material: ModContent.ItemType<AlcadizScrap>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Fable Scrap x Blank Bow
            AddBrew(
                result: ModContent.ItemType<MorrowSalface>(),
                mold: ModContent.ItemType<BlankBow>(),
                material: ModContent.ItemType<AlcadizScrap>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<MorrowedCrossbow>(),
                mold: ModContent.ItemType<BlankBow>(),
                material: ModContent.ItemType<AlcadizScrap>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Fable Scrap x Blank Juggler
            AddBrew(
                result: ModContent.ItemType<BurningAngel>(),
                mold: ModContent.ItemType<BlankJuggler>(),
                material: ModContent.ItemType<AlcadizScrap>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<BurningFlask>(),
                mold: ModContent.ItemType<BlankJuggler>(),
                material: ModContent.ItemType<AlcadizScrap>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Fable Scrap x Blank Gun
            AddBrew(
               result: ModContent.ItemType<GardenWrecker>(),
               mold: ModContent.ItemType<BlankGun>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            AddBrew(
               result: ModContent.ItemType<wowgun>(),
               mold: ModContent.ItemType<BlankGun>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            //Fable Scrap x Blank Staff
            AddBrew(
               result: ModContent.ItemType<GildedStaff>(),
               mold: ModContent.ItemType<BlankStaff>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            AddBrew(
               result: ModContent.ItemType<Bongos>(),
               mold: ModContent.ItemType<BlankStaff>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            AddBrew(
               result: ModContent.ItemType<StarFlowerStaff>(),
               mold: ModContent.ItemType<BlankStaff>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            //Fable Scrap x Blank Accessory
            AddBrew(
               result: ModContent.ItemType<Bonfire>(),
               mold: ModContent.ItemType<BlankAccessory>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            AddBrew(
               result: ModContent.ItemType<FireEmblem>(),
               mold: ModContent.ItemType<BlankStaff>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            //Fable Scrap x Blank Card
            AddBrew(
               result: ModContent.ItemType<GothivCard>(),
               mold: ModContent.ItemType<BlankCard>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            //Fable Scrap x Blank Dust
            AddBrew(
               result: ModContent.ItemType<FlamePowder>(),
               mold: ModContent.ItemType<BlankBag>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            AddBrew(
               result: ModContent.ItemType<AlcadizPowder>(),
               mold: ModContent.ItemType<BlankBag>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            //Fable Scrap x Blank Shield
            AddBrew(
               result: ModContent.ItemType<HornedNail>(),
               mold: ModContent.ItemType<BlankBag>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            //Fable Scrap x Blank Brooch
            AddBrew(
               result: ModContent.ItemType<MorrowedBroochA>(),
               mold: ModContent.ItemType<BlankBrooch>(),
               material: ModContent.ItemType<AlcadizScrap>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            AddBrew(
              result: ModContent.ItemType<AlcadizDagger>(),
              mold: ModContent.ItemType<BlankBrooch>(),
              material: ModContent.ItemType<AlcadizScrap>(),
              materialCount: 10,
              weight: 1.0f,
              yield: 1);

            //Fable Scrap x Blank Brooch
            AddBrew(
              result: ModContent.ItemType<Violar>(),
              mold: ModContent.ItemType<BlankOrb>(),
              material: ModContent.ItemType<AlcadizScrap>(),
              materialCount: 10,
              weight: 1.0f,
              yield: 1);

            //Winterborn Shard x Blank Sword
            AddBrew(
              result: ModContent.ItemType<FrostBringer>(),
              mold: ModContent.ItemType<BlankSword>(),
              material: ModContent.ItemType<WinterbornShard>(),
              materialCount: 10,
              weight: 1.0f,
              yield: 1);

            AddBrew(
              result: ModContent.ItemType<Auroran>(),
              mold: ModContent.ItemType<BlankSword>(),
              material: ModContent.ItemType<WinterbornShard>(),
              materialCount: 10,
              weight: 1.0f,
              yield: 1);

            AddBrew(
              result: ModContent.ItemType<TheFirstAurora>(),
              mold: ModContent.ItemType<BlankSword>(),
              material: ModContent.ItemType<WinterbornShard>(),
              materialCount: 10,
              weight: 1.0f,
              yield: 1);

            AddBrew(
                result: ModContent.ItemType<MooningSlicer>(),
                mold: ModContent.ItemType<BlankSword>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Winterborn Shard x Blank Bow
            AddBrew(
                result: ModContent.ItemType<IceWalker>(),
                mold: ModContent.ItemType<BlankBow>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<Cryal>(),
                mold: ModContent.ItemType<BlankBow>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<Ebistar>(),
                mold: ModContent.ItemType<BlankBow>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<FrostyCrossbow>(),
                mold: ModContent.ItemType<BlankBow>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Winterborn Shard x Blank Gun
            AddBrew(
                result: ModContent.ItemType<IceCubeMaker>(),
                mold: ModContent.ItemType<BlankGun>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
               result: ModContent.ItemType<Pearlinator>(),
               mold: ModContent.ItemType<BlankGun>(),
               material: ModContent.ItemType<WinterbornShard>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            AddBrew(
               result: ModContent.ItemType<MsFreeze>(),
               mold: ModContent.ItemType<BlankGun>(),
               material: ModContent.ItemType<WinterbornShard>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            //Winterborn Shard x Blank Staff
            AddBrew(
                result: ModContent.ItemType<IceboundStaff>(),
                mold: ModContent.ItemType<BlankStaff>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<AuroranSeeker>(),
                mold: ModContent.ItemType<BlankStaff>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Winterborn Shard x Blank Accessory
            AddBrew(
                result: ModContent.ItemType<IceClimbers>(),
                mold: ModContent.ItemType<BlankAccessory>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Winterborn Shard x Blank Card
            AddBrew(
                result: ModContent.ItemType<FrostCard>(),
                mold: ModContent.ItemType<BlankCard>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Winterborn Shard x Blank Bag
            AddBrew(
                result: ModContent.ItemType<FrostedPowder>(),
                mold: ModContent.ItemType<BlankBag>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Winterborn Shard x Blank Safunai
            AddBrew(
                result: ModContent.ItemType<Parendine>(),
                mold: ModContent.ItemType<BlankSafunai>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Winterborn Shard x Blank Brooch
            AddBrew(
                result: ModContent.ItemType<FrileBroochA>(),
                mold: ModContent.ItemType<BlankBrooch>(),
                material: ModContent.ItemType<WinterbornShard>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Winterborn Shard x Blank Orb
            AddBrew(
               result: ModContent.ItemType<EvasiveBalls>(),
               mold: ModContent.ItemType<BlankOrb>(),
               material: ModContent.ItemType<WinterbornShard>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);


            //Gintz Metal x Sword
            AddBrew(
                 result: ModContent.ItemType<Gutinier>(),
                 mold: ModContent.ItemType<BlankSword>(),
                 material: ModContent.ItemType<GintzlMetal>(),
                 materialCount: 10,
                 weight: 1.0f,
                 yield: 1);

            AddBrew(
                 result: ModContent.ItemType<VerstiDance>(),
                 mold: ModContent.ItemType<BlankSword>(),
                 material: ModContent.ItemType<GintzlMetal>(),
                 materialCount: 10,
                 weight: 1.0f,
                 yield: 1);

            AddBrew(
                 result: ModContent.ItemType<StalkersTallon>(),
                 mold: ModContent.ItemType<BlankSword>(),
                 material: ModContent.ItemType<GintzlMetal>(),
                 materialCount: 10,
                 weight: 1.0f,
                 yield: 1);

            AddBrew(
                 result: ModContent.ItemType<GladiatorSpear>(),
                 mold: ModContent.ItemType<BlankSword>(),
                 material: ModContent.ItemType<GintzlMetal>(),
                 materialCount: 10,
                 weight: 1.0f,
                 yield: 1);

            AddBrew(
                 result: ModContent.ItemType<IronHook>(),
                 mold: ModContent.ItemType<BlankSword>(),
                 material: ModContent.ItemType<GintzlMetal>(),
                 materialCount: 10,
                 weight: 1.0f,
                 yield: 1);

            //Gintz Metal x Bow
            AddBrew(
                 result: ModContent.ItemType<GintzlsSteed>(),
                 mold: ModContent.ItemType<BlankBow>(),
                 material: ModContent.ItemType<GintzlMetal>(),
                 materialCount: 10,
                 weight: 1.0f,
                 yield: 1);

            AddBrew(
                 result: ModContent.ItemType<DesertCrossbow>(),
                 mold: ModContent.ItemType<BlankBow>(),
                 material: ModContent.ItemType<GintzlMetal>(),
                 materialCount: 10,
                 weight: 1.0f,
                 yield: 1);


            AddBrew(
                  result: ModContent.ItemType<WingedFury>(),
                  mold: ModContent.ItemType<BlankBow>(),
                  material: ModContent.ItemType<GintzlMetal>(),
                  materialCount: 10,
                  weight: 1.0f,
                  yield: 1);

            //Gintzl Metal x Juggler

            AddBrew(
                 result: ModContent.ItemType<GintzlSpear>(),
                 mold: ModContent.ItemType<BlankOrb>(),
                 material: ModContent.ItemType<GintzlMetal>(),
                 materialCount: 10,
                 weight: 1.0f,
                 yield: 1);

            AddBrew(
                result: ModContent.ItemType<CleanestCleaver>(),
                mold: ModContent.ItemType<BlankOrb>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);


            AddBrew(
                result: ModContent.ItemType<GreyBricks>(),
                mold: ModContent.ItemType<BlankOrb>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<SpikedLobber>(),
                mold: ModContent.ItemType<BlankOrb>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Gintzl Metal x Gun
            AddBrew(
                 result: ModContent.ItemType<Dunderbustion>(),
                 mold: ModContent.ItemType<BlankGun>(),
                 material: ModContent.ItemType<GintzlMetal>(),
                 materialCount: 10,
                 weight: 1.0f,
                 yield: 1);

            AddBrew(
                 result: ModContent.ItemType<Eagle>(),
                 mold: ModContent.ItemType<BlankGun>(),
                 material: ModContent.ItemType<GintzlMetal>(),
                 materialCount: 10,
                 weight: 1.0f,
                 yield: 1);

            //Gintzl Metal x Staff
            AddBrew(
                  result: ModContent.ItemType<Valtotude>(),
                  mold: ModContent.ItemType<BlankStaff>(),
                  material: ModContent.ItemType<GintzlMetal>(),
                  materialCount: 10,
                  weight: 1.0f,
                  yield: 1);

            AddBrew(
                 result: ModContent.ItemType<SwarmerStaff>(),
                 mold: ModContent.ItemType<BlankStaff>(),
                 material: ModContent.ItemType<GintzlMetal>(),
                 materialCount: 10,
                 weight: 1.0f,
                 yield: 1);



            AddBrew(
                result: ModContent.ItemType<SunBlastStaff>(),
                mold: ModContent.ItemType<BlankStaff>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Gintzl Metal x Accessory
            AddBrew(
                result: ModContent.ItemType<Barry>(),
                mold: ModContent.ItemType<BlankAccessory>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
               result: ModContent.ItemType<SunGlyph>(),
               mold: ModContent.ItemType<BlankAccessory>(),
               material: ModContent.ItemType<GintzlMetal>(),
               materialCount: 10,
               weight: 1.0f,
               yield: 1);

            //Gintzl Metal x Card
            AddBrew(
                result: ModContent.ItemType<GintzeCard>(),
                mold: ModContent.ItemType<BlankCard>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<MetallianCard>(),
                mold: ModContent.ItemType<BlankCard>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Gintzl Metal x Powder
            AddBrew(
                result: ModContent.ItemType<AivanPowder>(),
                mold: ModContent.ItemType<BlankBag>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Gintzl Metal x Rune
            AddBrew(
                result: ModContent.ItemType<RuneOfWind>(),
                mold: ModContent.ItemType<BlankRune>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Gintzl Metal x Saunfai
            AddBrew(
                result: ModContent.ItemType<Alcarish>(),
                mold: ModContent.ItemType<BlankSafunai>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            /*
            AddBrew(
                result: ModContent.ItemType<DesertWhip>(),
                mold: ModContent.ItemType<BlankSafunai>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);
            */
            //Gintzl Metal x Shield
            AddBrew(
                result: ModContent.ItemType<PointedEdge>(),
                mold: ModContent.ItemType<BlankShield>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Gintzl Metal x Brooch
            AddBrew(
                result: ModContent.ItemType<SandyBroochA>(),
                mold: ModContent.ItemType<BlankBrooch>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            AddBrew(
                result: ModContent.ItemType<GintzlBroochA>(),
                mold: ModContent.ItemType<BlankBrooch>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Gintzl Metal x Orb
            AddBrew(
                result: ModContent.ItemType<RazorBragett>(),
                mold: ModContent.ItemType<BlankOrb>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);

            //Gintzl Metal x Stein
            AddBrew(
                result: ModContent.ItemType<Hultinstein>(),
                mold: ModContent.ItemType<BlankStein>(),
                material: ModContent.ItemType<GintzlMetal>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);
            _brews.Sort((x, y) => ModContent.GetModItem(x.mold).Name.CompareTo(ModContent.GetModItem(y.mold).Name));


        }

        private void AddBrew(int result, int mold, int material, int materialCount, float weight = 1.0f, int yield = 1)
        {
            CauldronBrew brew = new CauldronBrew
            {
                result = result,
                mold = mold,
                material = material,
                materialAmount = materialCount,
                weight = weight,
                yield = yield
            };

            _brews.Add(brew);
        }

        private List<CauldronBrew> GetPossibleBrews(int mold, int material, int materialCount)
        {
            List<CauldronBrew> possibleBrews = _brews.Where
                (x => x.mold == mold && x.material == material && materialCount >= x.materialAmount).ToList();
            return possibleBrews;
        }
        private List<CauldronBrew> GetPossibleBrews(int material, int materialCount)
        {
            List<CauldronBrew> possibleBrews = _brews.Where
                (x => x.material == material && materialCount >= x.materialAmount).ToList();
            return possibleBrews;
        }

        public Item FindMold(Item item)
        {
            foreach (var brew in _brews)
            {
                if (brew.result == item.type)
                    return ModContent.GetModItem(brew.mold).Item;
            }
            Item r = new Item();
            r.SetDefaults(0);
            return r;
        }

        public bool IsResult(Item item)
        {
            foreach (var brew in _brews)
            {
                if (brew.result == item.type)
                    return true;
            }
            return false;
        }

        public CauldronBrew FindBrew(Item item)
        {
            foreach (var brew in _brews)
            {
                if (brew.result == item.type)
                    return brew;
            }

            return NothingBrew;
        }



        public Item[] GetMaterials()
        {
            List<Item> materials = new List<Item>();
            foreach (var brew in _brews)
            {
                Item item = ModContent.GetModItem(brew.material).Item;
                if (!materials.Contains(item))
                    materials.Add(item);
            }
            return materials.ToArray();
        }

        public Item[] GetCraftsFromMaterial(int materialType)
        {
            List<Item> crafts = new List<Item>();
            List<CauldronBrew> brewsFromMaterial = _brews.Where(x => x.material == materialType).ToList();
            foreach (var brew in brewsFromMaterial)
            {
                crafts.Add(ModContent.GetModItem(brew.result).Item);
            }
            return crafts.ToArray();
        }

        public bool IsMaterial(int material)
        {
            return _brews.Find(x => x.material == material) != null;
        }

        public bool IsMold(int itemType)
        {
            return _brews.Find(x => x.mold == itemType) != null;
        }

        public bool IsMaterialOrMold(int itemType)
        {
            return _brews.Find(x => x.material == itemType || x.mold == itemType) != null;
        }
        public bool CanBrewSomething(Item mold, Item material)
        {
            return CanBrewSomething(mold.type, material.type, material.stack);
        }

        public bool CanBrewSomething(int mold, int material, int materialCount)
        {
            return GetPossibleBrews(mold, material, materialCount).Any();
        }

        public CauldronBrew Craft(Item mold, Item material)
        {
            //Get all possible crafts
            List<CauldronBrew> possibleBrews;
            if (mold.IsAir)
            {
                //No mold, get something random
                possibleBrews = GetPossibleBrews(material.type, material.stack);
            }
            else
            {
                possibleBrews = GetPossibleBrews(mold.type, material.type, material.stack);
            }

            if (possibleBrews.Count == 0)
            {
                OnBrew?.Invoke(NothingBrew);
                return NothingBrew;
            }

            CauldronPlayer cauldronPlayer = Main.LocalPlayer.GetModPlayer<CauldronPlayer>();
            WeightedRandom<CauldronBrew> random = new WeightedRandom<CauldronBrew>();
            for (int i = 0; i < possibleBrews.Count; i++)
            {
                int itemResult = possibleBrews[i].result;
                if (cauldronPlayer.HasMadeItem(itemResult) && cauldronPlayer.CrystalStarCount > 0)
                    continue;
                random.Add(possibleBrews[i], possibleBrews[i].weight);
            }

            bool consumeStar = true;
            if (random.elements.Count == 0)
            {
                consumeStar = false;
                for (int i = 0; i < possibleBrews.Count; i++)
                {
                    int itemResult = possibleBrews[i].result;
                    random.Add(possibleBrews[i], possibleBrews[i].weight);
                }
            }


            //Get the result
            CauldronBrew result = random;
            mold.stack -= 1;
            material.stack -= result.materialAmount;





            int starCount = cauldronPlayer.CrystalStarCount;
            if (cauldronPlayer.CrystalStarCount > 0 && consumeStar)
            {
                cauldronPlayer.Make(ModContent.GetModItem(result.result).Item);
                cauldronPlayer.CrystalStarCount -= 1;
            }
            else
            {
                bool getNothingFailed = Main.rand.NextFloat(0, 100) <= cauldronPlayer.NothingFailChance;
                bool inkFailed = Main.rand.NextFloat(0, 100) <= cauldronPlayer.InkFailChance;

                if (getNothingFailed)
                {
                    result = NothingBrew;
                }
                else if (inkFailed && Main.hardMode)
                {
                    result = InkBrew;
                }

                if (!getNothingFailed)
                {
                    cauldronPlayer.Make(ModContent.GetModItem(result.result).Item);
                }
            }

            //Crafting Quest
            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            var starterQuest = QuestLoader.GetInstance<CauldronCrafting>();
            questPlayer.CompleteQuest(starterQuest);

            JustCrafted = result;
            return result;
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            JustCrafted = null;
        }
    }
}
