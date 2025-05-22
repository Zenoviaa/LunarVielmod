using Stellamod.Items.MoonlightMagic.Forms;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class Ghettingberg : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 14;
            Item.shootSpeed = 10;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Size = 10;
            TrailLength = 32;
            Form = FormRegistry.Vase.Value;
        }


        public override int GetNormalSlotCount()
        {
            return 1;
        }

        public override int GetTimedSlotCount()
        {
            return 3;
        }
    }
}
