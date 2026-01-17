using Stellamod.Common;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Deeya
{
    public class GamblerEnchantment : BaseEnchantment
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
                var enchantmentsToSpawn = ItemHelper.Enchantments;
                BaseEnchantment enchantmentToSwapTo = enchantmentsToSpawn[Main.rand.Next(0, enchantmentsToSpawn.Length)];
                if (EnchantmentHelper.SpecialEnchantments.Contains(enchantmentToSwapTo.Type))
                    return;

                int indexOfThisEnchantment = MagicProj.IndexOfEnchantment(this);

                MagicProj.ReplaceEnchantment(enchantmentToSwapTo, indexOfThisEnchantment);
                HasSwapped = true;
            }
        }
    }
}
