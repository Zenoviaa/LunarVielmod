using Stellamod.Content.Items.MoonlightMagic.Elements;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Guuts
{
    public class KillerEnchantment : BaseEnchantment
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 30;
        }

        public override void AI()
        {
            base.AI();
            Countertimer++;
            if (Countertimer == time)
                Projectile.Kill();
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<GuutElement>();
        }

    }
}
