using Stellamod.Items.MoonlightMagic.Forms;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class FoxNMagic : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 39;
            Item.shootSpeed = 13;
            Item.useTime = 30;
            Item.useAnimation = 60;
            Size = 12;
            TrailLength = 20;
            Form = FormRegistry.Swirl.Value;
        }


        public override int GetNormalSlotCount()
        {
            return 5;
        }

        public override int GetTimedSlotCount()
        {
            return 4;
        }
    }
}
