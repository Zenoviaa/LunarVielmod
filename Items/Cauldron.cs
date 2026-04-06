
using MonoMod.Cil;
using Stellamod.Buffs;
using Stellamod.Common.QuestSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Quests.ZuiQuest;
using Stellamod.Items.Ores;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Stellamod.Items
{
    public class CauldronPlayer : ModPlayer
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

    public class CauldronBrew
    {
        public int result;
        public int mold;
        public int material;
        public int materialAmount;
        public float weight = 1.0f;
        public int yield = 1;
    }

    public static class BrewExtension
    {
        public static CauldronBrew RegisterBrew<Material, Mold>(this ModItem result, float weight = 1.0f, int yield = 1)
            where Material : ModItem
            where Mold : SirestiasMold
        {
            Cauldron cauldron = ModContent.GetInstance<Cauldron>();
            int materialType = ModContent.ItemType<Material>();
            int moldType = ModContent.ItemType<Mold>();

            Cauldron.IsBrewingMaterial[materialType] = true;
            Cauldron.IsBrewingMold[moldType] = true;
            Cauldron.MaterialRarity[result.Type] = ItemLoader.GetItem(materialType).Item.rare;
            return cauldron.AddBrew(result.Item.type, moldType, materialType, 10, weight, yield);
        }

        public static CauldronBrew RegisterBrew(this ModItem result, int mold, int material, float weight = 1.0f, int yield = 1)
        {
            Cauldron cauldron = ModContent.GetInstance<Cauldron>();

            Cauldron.IsBrewingMaterial[material] = true;
            Cauldron.IsBrewingMold[mold] = true;
            Cauldron.MaterialRarity[result.Type] = ItemLoader.GetItem(material).Item.rare;
            return cauldron.AddBrew(result.Item.type, mold, material, 10, weight, yield);
        }
    }

    public class Cauldron : ModSystem
    {
        public static int[] MaterialRarity = ItemID.Sets.Factory.CreateIntSet(0);
        public static bool[] IsBrewingMaterial = ItemID.Sets.Factory.CreateBoolSet();
        public static bool[] IsBrewingMold = ItemID.Sets.Factory.CreateBoolSet();
        private List<CauldronBrew> _brews = new List<CauldronBrew>()
        {

        };

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
        public override void OnModUnload()
        {
            base.OnModUnload();
            _brews.Clear();
        }

        public CauldronBrew AddBrew(int result, int mold, int material, int materialCount, float weight = 1.0f, int yield = 1)
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
            return brew;
        }
        private List<CauldronBrew> GetPossibleBrews(List<int> molds, int material, int materialCount)
        {
            List<CauldronBrew> possibleBrews = _brews.Where
                (x => molds.Contains(x.mold) && x.material == material && materialCount >= x.materialAmount).ToList();
            return possibleBrews;
        }

        private List<CauldronBrew> GetPossibleBrews(int material, int materialCount)
        {
            List<CauldronBrew> possibleBrews = _brews.Where
                (x => x.material == material && materialCount >= x.materialAmount).ToList();
            return possibleBrews;
        }
        public Item FindMaterial(Item item)
        {
            foreach (var brew in _brews)
            {
                if (brew.result == item.type)
                    return ModContent.GetModItem(brew.material).Item;
            }
            Item r = new Item();
            r.SetDefaults(ItemID.None);
            return r;
        }
        public Item FindMold(Item item)
        {
            //TODO: optimize this to O(1) lookup time by making an array
            foreach (var brew in _brews)
            {
                if (brew.result == item.type)
                    return ModContent.GetModItem(brew.mold).Item;
            }
            Item r = new Item();
            r.SetDefaults(ItemID.None);
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
            return IsBrewingMaterial[material];
        }

        public bool IsMold(int itemType)
        {
            return IsBrewingMold[itemType];
        }

        public bool IsAir(Item[] molds)
        {
            for(int i = 0; i < molds.Length; i++)
            {
                if (!molds[i].IsAir)
                    return false;
            }
            return true;
        }

        public CauldronBrew Craft(Item[] molds, Item material)
        {
            //Get all possible crafts
            List<CauldronBrew> possibleBrews;
            if (IsAir(molds))
            {
                //No mold, get something random
                possibleBrews = GetPossibleBrews(material.type, material.stack);
            }
            else
            {
                List<int> moldTypes = new List<int>();
                foreach(Item mold in molds)
                {
                    moldTypes.Add(mold.type);
                }

                possibleBrews = GetPossibleBrews(moldTypes, material.type, material.stack);
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
            for(int i = 0; i < molds.Length; i++)
            {
                if (result.mold != molds[i].type)
                    continue;
                molds[i].stack -= 1;
            }

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
            var starterQuest = ModContent.GetInstance<CraftAtCauldron>();
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
