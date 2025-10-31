using Microsoft.Xna.Framework;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content
{
    public class RaritySystem : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            base.SetDefaults(entity);
            Cauldron cauldron = ModContent.GetInstance<Cauldron>();
            int rarityType = Cauldron.MaterialRarity[entity.type];
            if(rarityType != 0)
            {
                entity.rare = rarityType;
            }
        }
    }



    public class SpringMushroomRarity : ModRarity
    {
        public override Color RarityColor => new Color(235, 150, 135);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
                  return ModContent.RarityType<IvythornRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }

    public class IvythornRarity : ModRarity
    {
        public override Color RarityColor => new Color(63, 98, 32);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
            //    return ModContent.RarityType<ExampleHigherTierModRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
}
