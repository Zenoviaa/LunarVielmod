using Stellamod.Content.Items.MoonlightMagic.Elements;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Guuts
{
    public class RotisserieChickenEnchantment : BaseEnchantment
    {

        public override void AI()
        {
            base.AI();
            MagicProj.extraRotation += 0.05f;
        }

        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<GuutElement>();
        }

    }
}
