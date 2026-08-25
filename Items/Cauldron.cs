using Stellamod.Buffs;
using Stellamod.Common.QuestSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Quests.ZuiQuest;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Cinematics;
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
            Crafts.Add(item);
            Crafts = Crafts.DistinctBy(x => x.type).ToList();
        }

        public int CountCraftsInMaterial(int materialType)
        {
         
          //  Console.WriteLine(Crafts.Count);
            int count = 0;
            var cauldron = ModContent.GetInstance<Cauldron>();
            foreach(var craft in Crafts)
            {
                int type = craft.type;
                if (cauldron.GetMaterial(type) == materialType)
                {
                    count++;
                }
            }
            return count;
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

    public struct StoredBrewingMaterial
    {
        public int item;
        public int stack;
    }
    public class Cauldron : ModSystem
    {
        public static int[] MaterialOrder = ItemID.Sets.Factory.CreateIntSet(0);
        public static int[] MaterialRarity = ItemID.Sets.Factory.CreateIntSet(0);
        public static bool[] IsBrewingMaterial = ItemID.Sets.Factory.CreateBoolSet();
        public static bool[] IsBrewingMold = ItemID.Sets.Factory.CreateBoolSet();
        private Queue<StoredBrewingMaterial> _results;
        private List<StoredBrewingMaterial> _brewingMaterials;
        public List<StoredBrewingMaterial> InsideCauldron
        {
            get
            {
                _brewingMaterials ??= new List<StoredBrewingMaterial>();
                return _brewingMaterials;
            }
        }

        public Queue<StoredBrewingMaterial> Results
        {
            get
            {
                _results ??= new Queue<StoredBrewingMaterial>();
                return _results;
            }
        }

        private List<CauldronBrew> _brews = new List<CauldronBrew>()
        {

        };

        public CauldronBrew NothingBrew
        {
            get
            {
                CauldronBrew brew = new CauldronBrew();
                brew.result = 0;
                return brew;
            }
        }

        public static event Action<CauldronBrew> OnBrew;
        public CauldronBrew JustCrafted { get; set; }
        public bool IsDirty;
        public override void OnModUnload()
        {
            base.OnModUnload();
            _brews.Clear();
        }

        private static int _material;
        public static void SetMaterial(int material)
        {
            _material = material;
        }

        public static CauldronBrew VanillaBrew(int result, float weight = 1.0f, int yield = 1)
        {
            int mold = ModContent.ItemType<BlankVanilla>();
            Cauldron cauldron = ModContent.GetInstance<Cauldron>();

            Cauldron.IsBrewingMaterial[_material] = true;
            Cauldron.IsBrewingMold[mold] = true;
            Cauldron.MaterialRarity[result] = (new Item(result).rare);
            //    Cauldron.MaterialRarity[result.Type] = ItemLoader.GetItem(material).Item.rare;
            return cauldron.AddBrew(result, mold, _material, 10, weight, yield);
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
        public int CountCraftsInMaterial(int materialType)
        {
            return _brews.Count(x => x.material == materialType);
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
                crafts.Add(new Item(brew.result));
                //   crafts.Add(ModContent.GetModItem(brew.result).Item);
            }
     
            return crafts.ToArray();
        }

        public int GetMaterial(int craft)
        {
            foreach (var brew in _brews)
            {
                if(brew.result == craft)
                {
                    return brew.material;
                }
            }
            return 0;
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
            for (int i = 0; i < molds.Length; i++)
            {
                if (!molds[i].IsAir)
                    return false;
            }
            return true;
        }

        public CauldronBrew Mix(Item mold, Item material)
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
                List<int> moldTypes = new List<int>();
                moldTypes.Add(mold.type);
                possibleBrews = GetPossibleBrews(moldTypes, material.type, material.stack);
            }

            if (possibleBrews.Count == 0)
            {
                OnBrew?.Invoke(NothingBrew);
                return NothingBrew;
            }

            CauldronPlayer cauldronPlayer = Main.LocalPlayer.GetModPlayer<CauldronPlayer>();
            WeightedRandom<CauldronBrew> random = new WeightedRandom<CauldronBrew>(Main.rand.Next(0, int.MaxValue));
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
            int starCount = cauldronPlayer.CrystalStarCount;
            if (cauldronPlayer.CrystalStarCount > 0 && consumeStar)
            {
                cauldronPlayer.Make(new Item(result.result));
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

                if (!getNothingFailed)
                {
                    cauldronPlayer.Make(new Item(result.result));
                }
            }

            //Crafting Quest
            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            var starterQuest = ModContent.GetInstance<CraftAtCauldron>();
            questPlayer.CompleteQuest(starterQuest);

            JustCrafted = result;
            return result;
        }

        public void AddToBrew(int item, int stack)
        {
            bool found = false;
            for (int i = 0; i < _brewingMaterials.Count; i++)
            {
                StoredBrewingMaterial sbm = _brewingMaterials[i];
                if (sbm.item == item)
                {
                    sbm.stack += stack;
                    _brewingMaterials[i] = sbm;
                    found = true;
                    break;
                }

            }

            if (!found)
            {
                StoredBrewingMaterial sbm = new StoredBrewingMaterial
                {
                    item = item,
                    stack = stack
                };
                _brewingMaterials.Add(sbm);
            }
            IsDirty = true;
            SendSyncPacket();
            /*
            if (CanMix())
            {
                GetResultsFromMixture();
            }*/
        }
        public bool CanMix()
        {
            return _brewingMaterials.Count > 0;
        }

        public void MixDaCauldron()
        {
            List<Item> moldWith = new List<Item>();
            Queue<Item> brewWith = new Queue<Item>();
            for (int i = 0; i < _brewingMaterials.Count; i++)
            {
                StoredBrewingMaterial sbm = _brewingMaterials[i];
                while (IsMaterial(sbm.item) && sbm.stack >= 10)
                {
                    brewWith.Enqueue(new Item(sbm.item, sbm.stack));
                    sbm.stack -= 10;
                }
                while (IsMold(sbm.item) && sbm.stack >= 1)
                {
                    moldWith.Add(new Item(sbm.item));
                    sbm.stack -= 1;
                }
                _brewingMaterials[i] = sbm;
            }

            Item air = new Item(0);
            air.TurnToAir();
            while (brewWith.Count > 0)
            {
                Item mold = air;
                if (moldWith.Count > 0)
                    mold = moldWith[Main.rand.Next(moldWith.Count)];
                Item material = brewWith.Dequeue();
                CauldronBrew result = Mix(mold, material);
                StoredBrewingMaterial brew = new StoredBrewingMaterial
                {
                    item = result.result,
                    stack = 1
                };
                Results.Enqueue(brew);
            }

            for (int i = 0; i < _brewingMaterials.Count; i++)
            {
                StoredBrewingMaterial sbm = _brewingMaterials[i];
                if (IsMold(sbm.item))
                    continue;

                if (sbm.stack <= 0)
                    continue;

                Results.Enqueue(sbm);
            }
            _brewingMaterials.Clear();
            SendSyncPacket();
        }
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            JustCrafted = null;
        }

        public void SendSyncPacket()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                int length = _brewingMaterials.Count;
                object[] data = new object[length * 2 + 1];
                int index = 0;
                data[index++] = length;
                for (int i = 0; i < length; i++)
                {
                    data[index++] = _brewingMaterials[i].item;
                    data[index++] = _brewingMaterials[i].stack;
                }
                Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.CauldronSync, data)
                    .Send(-1);
            }
        }

        public void HandleSyncPacket(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            List<StoredBrewingMaterial> materials = new List<StoredBrewingMaterial>();
            for (int i = 0; i < length; i++)
            {
                int item = reader.ReadInt32();
                int stack = reader.ReadInt32();
                StoredBrewingMaterial material = new StoredBrewingMaterial
                {
                    item = item,
                    stack = stack
                };
                materials.Add(material);
            }
            _brewingMaterials = materials;
        }
    }

}
