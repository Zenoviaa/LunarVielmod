using Stellamod.Items.MoonlightMagic.Forms;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class RainsyStaff : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 12;
            Item.shootSpeed = 14;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Size = 10;
            TrailLength = 16;
            Form = FormRegistry.Pickaxe.Value;
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
