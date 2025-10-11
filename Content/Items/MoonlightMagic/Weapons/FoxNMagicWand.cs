using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class FoxNMagicWand : BaseStaff
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
