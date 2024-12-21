using Stellamod.Items.MoonlightMagic.Forms;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class HexxingStaff : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 10;
            Item.shootSpeed = 16;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Size = 8;
            TrailLength = 30;
            Form = FormRegistry.Snowglobe.Value;
        }


        public override int GetNormalSlotCount()
        {
            return 3;
        }

        public override int GetTimedSlotCount()
        {
            return 0;
        }
    }
}
