using Microsoft.Xna.Framework;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
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
            if (rarityType != 0)
            {
                entity.rare = rarityType;
            }
        }


        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            //Ehh kinda goofy
            /*
            if (item.rare == ModContent.RarityType<FableScrapRarity>() && line.Name.Contains("ItemName"))
            {
                Color gColor = Color.Lerp(Color.White, Color.Red, ExtraMath.Osc(0f, 1f));
                Color pColor = Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f);
                pColor = Color.Lerp(pColor, Color.DarkRed, ExtraMath.Osc(0f, 1f));
                EnchantmentDrawHelper.DrawGlowingRarityLine(Main.spriteBatch, item, line, ref yOffset,
                    glowColor: gColor,
                    primaryColor: pColor,
                    noiseColor: new Color(206, 101, 0), TrailRegistry.LightningTrail2);
                return false;
            }*/

            return base.PreDrawTooltipLine(item, line, ref yOffset);
        }
    }


    public abstract class LunarRarity : ModRarity
    {
        public abstract void DrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset);
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
                return ModContent.RarityType<FableScrapRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
    public class FableScrapRarity : ModRarity
    {
        public override Color RarityColor => new Color(223, 176, 100);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
                  return ModContent.RarityType<WinterbornShardRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
    public class WinterbornShardRarity : ModRarity
    {
        public override Color RarityColor => new Color(138, 225, 255);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
                  return ModContent.RarityType<TerrorFragmentRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
    public class TerrorFragmentRarity : ModRarity
    {
        public override Color RarityColor => new Color(255, 99, 99);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
                  return ModContent.RarityType<GintzlMetalRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
    public class GintzlMetalRarity : ModRarity
    {
        public override Color RarityColor => Color.Silver;
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
              //    return ModContent.RarityType<ExampleHigherTierModRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
                return ModContent.RarityType<CinderscrapRarity>();
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
    public class CinderscrapRarity : ModRarity
    {
        public override Color RarityColor => new Color(255, 241, 41);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
              //    return ModContent.RarityType<ExampleHigherTierModRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }

    public class RadiantNectarRarity : ModRarity
    {
        public override Color RarityColor => new Color(254, 231, 97);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
              //    return ModContent.RarityType<ExampleHigherTierModRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
    public class MarshScrapRarity : ModRarity
    {
        public override Color RarityColor => new Color(124, 38, 7);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
              //    return ModContent.RarityType<ExampleHigherTierModRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
    public class ConvulgingMatterRarity : ModRarity
    {
        public override Color RarityColor => new Color(49, 39, 124);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
              //    return ModContent.RarityType<ExampleHigherTierModRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
    public class IllurineScaleRarity : ModRarity
    {
        public override Color RarityColor => new Color(53, 171, 213);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
              //    return ModContent.RarityType<ExampleHigherTierModRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
    public class MechanizedSoulRarity : ModRarity
    {
        public override Color RarityColor => new Color(218, 238, 240);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            { // If the offset is 1 or 2 (a positive modifier).
              //    return ModContent.RarityType<ExampleHigherTierModRarity>(); // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
    public class ShopRarity : ModRarity
    {
        public override Color RarityColor => Color.Gold;
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
