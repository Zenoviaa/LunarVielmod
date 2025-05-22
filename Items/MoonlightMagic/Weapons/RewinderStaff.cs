using Stellamod.Items.MoonlightMagic.Forms;
namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class RewinderStaff : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 24;
            Item.shootSpeed = 12;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Size = 20;
            TrailLength = 64;
            Form = FormRegistry.Circle.Value;
        }


        public override int GetNormalSlotCount()
        {
            return 0;
        }

        public override int GetTimedSlotCount()
        {
            return 5;
        }
    }
}
