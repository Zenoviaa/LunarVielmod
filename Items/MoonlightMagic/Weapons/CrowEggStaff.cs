using Stellamod.Items.MoonlightMagic.Forms;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class CrowEggStaff : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 20;
            Item.shootSpeed = 13;
            Item.useTime = 30;
            Item.useAnimation = 60;
            Size = 12;
            TrailLength = 20;
            Form = FormRegistry.Circle.Value;
        }


        public override int GetNormalSlotCount()
        {
            return 5;
        }

        public override int GetTimedSlotCount()
        {
            return 1;
        }
    }
}
