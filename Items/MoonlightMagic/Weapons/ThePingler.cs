using Stellamod.Items.MoonlightMagic.Forms;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class ThePingler : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 6;
            Item.shootSpeed = 7;
            Size = 16;
            TrailLength = 12;
            Form = FormRegistry.Circle.Value;
        }

        public override int GetNormalSlotCount()
        {
            return 2;
        }

        public override int GetTimedSlotCount()
        {
            return 0;
        }
    }
}
