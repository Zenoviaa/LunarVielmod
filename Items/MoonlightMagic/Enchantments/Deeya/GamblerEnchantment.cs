using Stellamod.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Deeya
{
    internal class GamblerEnchantment : BaseEnchantment
    {
        bool HasSwapped;
        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<DeeyaElement>();
        }

        public override void AI()
        {
            base.AI();
            if (!HasSwapped)
            {
                var enchantmentsToSpawn = AllEnchantments;
                BaseEnchantment enchantmentToSwapTo = enchantmentsToSpawn[Main.rand.Next(0, enchantmentsToSpawn.Length)];
                int indexOfThisEnchantment = MagicProj.IndexOfEnchantment(this);

                MagicProj.ReplaceEnchantment(enchantmentToSwapTo, indexOfThisEnchantment);
                HasSwapped = true;
            }
        }
    }
}
