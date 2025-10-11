using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class WillowOfTheSoulsWand : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 16;
            Item.shootSpeed = 7;
            Size = 24;
            TrailLength = 55;
            Form = FormRegistry.Crescent.Value;
        }


        public override int GetNormalSlotCount()
        {
            return 3;
        }

        public override int GetTimedSlotCount()
        {
            return 1;
        }
    }
}
