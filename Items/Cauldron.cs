using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Items.Weapons.Summon.Orbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Items
{
    internal class CauldronPlayer : ModPlayer
    {
        public float NothingFailChance;
        public float InkFailChance;

        public override void ResetEffects()
        {
            base.ResetEffects();

            //So I'm thinking we just have these variables
            NothingFailChance = 5;
            InkFailChance = 25;
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
        public override void PostAddRecipes()
        {
            base.PostAddRecipes();
            _brews = new List<CauldronBrew>();


            //Define all the combos here
            //Using Aurorean Starball for testing this system
            AddBrew(
                result: ModContent.ItemType<AuroreanStarball>(),
                mold: ModContent.ItemType<BlankOrb>(),
                material: ModContent.ItemType<AuroreanStarI>(),
                materialCount: 10,
                weight: 1.0f,
                yield: 1);
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

            WeightedRandom<CauldronBrew> random = new WeightedRandom<CauldronBrew>();
            for (int i = 0; i < possibleBrews.Count; i++)
            {
                random.Add(possibleBrews[i], possibleBrews[i].weight);
            }

            //Get the result
            CauldronBrew result = random;
            mold.stack -= 1;
            material.stack -= result.materialAmount;




            CauldronPlayer cauldronPlayer = Main.LocalPlayer.GetModPlayer<CauldronPlayer>();
            bool getNothingFailed = Main.rand.NextFloat(0, 100) <= cauldronPlayer.NothingFailChance;
            bool inkFailed = Main.rand.NextFloat(0, 100) <= cauldronPlayer.InkFailChance;

            if (getNothingFailed)
            {
                result = NothingBrew;
            } 
            else if (inkFailed)
            {
                result = InkBrew;
            }

            OnBrew?.Invoke(result);
            return result;
        }

        public int Craft(int mold, int material, int materialCount)
        {
            //Get all possible crafts
            List<CauldronBrew> possibleBrews = GetPossibleBrews(mold, material, materialCount);
            if (possibleBrews.Count == 0)
                return -1;
            WeightedRandom<CauldronBrew> random = new WeightedRandom<CauldronBrew>();
            for(int i = 0; i < possibleBrews.Count; i++)
            {
                random.Add(possibleBrews[i], possibleBrews[i].weight);
            }

            //Get the result
            CauldronBrew result = random;
            OnBrew?.Invoke(result);
            return result.result;
        }
    }
}
