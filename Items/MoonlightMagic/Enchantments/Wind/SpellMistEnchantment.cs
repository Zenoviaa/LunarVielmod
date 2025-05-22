using Stellamod.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Wind
{
    internal class SpellMistEnchantment : BaseEnchantment
    {
        public override void AI()
        {
            base.AI();

            //Count up
            Projectile.tileCollide = false;
        }


        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<WindElement>();
        }
    }
}
