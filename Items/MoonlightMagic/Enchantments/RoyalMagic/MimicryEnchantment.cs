using Stellamod.Items.MoonlightMagic.Elements;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.RoyalMagic
{
    internal class MimicryEnchantment : BaseEnchantment
    {
        bool HasSwapped;
        public override float GetStaffManaModifier()
        {
            return 0.5f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<RoyalMagicElement>();
        }

        public override void AI()
        {
            base.AI();
            if (!HasSwapped)
            {
                int indexOfThisEnchantment = MagicProj.IndexOfEnchantment(this);
                if (indexOfThisEnchantment > 0)
                {
                    MagicProj.ReplaceEnchantment(MagicProj.Enchantments[indexOfThisEnchantment - 1], indexOfThisEnchantment);
                }

                HasSwapped = true;
            }
        }
    }
}
