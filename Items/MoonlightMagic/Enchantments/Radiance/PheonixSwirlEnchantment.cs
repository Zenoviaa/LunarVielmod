using Microsoft.Xna.Framework;
using Stellamod.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Radiance
{
    internal class PheonixSwirlEnchantment : BaseEnchantment
    {
        private float Countertimer;
        private Vector2 _velocity;
        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;
            if (Countertimer == 1)
            {
                _velocity = Projectile.velocity;
            }

            Vector2 newVelocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(10));
            Projectile.velocity = newVelocity;
            Projectile.Center += _velocity * 0.3f;

            // Projectile.a greater than time then start homing, we'll just swap the movement type of the projectile

        }


        public override float GetStaffManaModifier()
        {
            return 0.3f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<RadianceElement>();
        }
    }
}
