using Stellamod.Content.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Hex
{
    public class GravityUpEnchantment : BaseEnchantment
    {
        public override int GetElementType()
        {
            return ModContent.ItemType<HexElement>();
        }

        public override void AI()
        {
            base.AI();
            float maxSpeed = 15;
            if (Projectile.velocity.Y > -maxSpeed)
            {
                Projectile.velocity.Y -= 0.4f;
            }
        }
    }
}
