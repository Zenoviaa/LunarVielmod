using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class GhettingbergWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 14;
            Item.shootSpeed = 10;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Size = 10;
            TrailLength = 32;
            Form = FormRegistry.Vase.Value;
            normalSlotCount = 1;
            timedSlotCount = 3;
        }
    }
}
