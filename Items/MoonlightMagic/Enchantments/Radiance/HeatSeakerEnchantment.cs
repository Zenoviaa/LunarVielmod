using Stellamod.Items.MoonlightMagic.Elements;
using Stellamod.Items.MoonlightMagic.Movements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Radiance
{
    internal class HeatSeakerEnchantment : BaseEnchantment
    {

        public override void SetDefaults()
        {
            base.SetDefaults();

        }

        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;

            //If greater than time then start homing, we'll just swap the movement type of the projectile
            if (Countertimer == 1)
            {

                MagicProj.Movement = new HomingLongMovement();
            }
            MagicProj.Size *= 1.005f;
            Projectile.velocity *= 1.005f;
        }

        public override float GetStaffManaModifier()
        {
            return 0.4f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<RadianceElement>();
        }
    }
}
