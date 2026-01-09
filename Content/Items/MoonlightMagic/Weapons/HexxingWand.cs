using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class HexxingWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 10;
            Item.shootSpeed = 16;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Size = 8;
            TrailLength = 30;
            Form = FormRegistry.Snowglobe.Value;
            normalSlotCount = 3;
            timedSlotCount = 0;
        }

    }
}
