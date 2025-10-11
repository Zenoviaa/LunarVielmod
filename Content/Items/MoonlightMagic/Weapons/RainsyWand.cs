using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class RainsyWand : BaseStaff
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
