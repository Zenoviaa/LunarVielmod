using Stellamod.Items.MoonlightMagic.Forms;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class ChalicemanSepter : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 28;
            Item.shootSpeed = 14;
            Item.useTime = 20;
            Item.useAnimation = 40;
            Size = 10;
            TrailLength = 22;
            Form = FormRegistry.Triangle.Value;
        }


        public override int GetNormalSlotCount()
        {
            return 2;
        }

        public override int GetTimedSlotCount()
        {
            return 4;
        }
    }
}
