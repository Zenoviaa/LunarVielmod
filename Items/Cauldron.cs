using Stellamod.Items.Materials;
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
    internal class CauldronBrew
    {
        public int result;
        public int mold;
        public int material;
        public int materialAmount;
        public float weight = 1.0f;
    }

    internal class Cauldron : ModSystem
    {
        private List<CauldronBrew> _brews;
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
                weight: 1.0f);
        }

        private void AddBrew(int result, int mold, int material, int materialCount, float weight = 1.0f)
        {
            CauldronBrew brew = new CauldronBrew
            {
                result = result,
                mold = mold,
                material = material,
                materialAmount = materialCount,
                weight = weight
            };

            _brews.Add(brew);
        }

        private List<CauldronBrew> GetPossibleBrews(int mold, int material, int materialCount)
        {
            List<CauldronBrew> possibleBrews = _brews.Where
                (x => x.mold == mold && x.material == material && materialCount >= x.materialAmount).ToList();
            return possibleBrews;
        }

        public bool IsMaterial(int material)
        {
            return _brews.Find(x => x.material == material) != null;
        }

        public bool CanBrewSomething(Item mold, Item material)
        {
            return CanBrewSomething(mold.type, material.type, material.stack);
        }

        public bool CanBrewSomething(int mold, int material, int materialCount)
        {
            return GetPossibleBrews(mold, material, materialCount).Any();
        }

        public int Craft(Item mold, Item material)
        {
            //Get all possible crafts
            List<CauldronBrew> possibleBrews = GetPossibleBrews(mold.type, material.type, material.stack);
            if (possibleBrews.Count == 0)
                return -1;
            WeightedRandom<CauldronBrew> random = new WeightedRandom<CauldronBrew>();
            for (int i = 0; i < possibleBrews.Count; i++)
            {
                random.Add(possibleBrews[i], possibleBrews[i].weight);
            }

            //Get the result
            CauldronBrew result = random;
            mold.stack -= 1;
            material.stack -= result.materialAmount;
            OnBrew?.Invoke(result);
            return result.result;
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
