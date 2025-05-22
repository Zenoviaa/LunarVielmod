using Stellamod.Items.MoonlightMagic.Forms;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class Snowflake : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 12;
            Item.shootSpeed = 5;
            Size = 32;
            TrailLength = 64;
            Form = FormRegistry.FourPointedStar.Value;
        }


        public override int GetNormalSlotCount()
        {
            return 2;
        }

        public override int GetTimedSlotCount()
        {
            return 1;
        }
    }
}
