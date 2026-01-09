using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class MirrorWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 35;
            Item.shootSpeed = 19;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Size = 20;
            TrailLength = 50;
            Form = FormRegistry.Lantern.Value;
            normalSlotCount = 3;
            timedSlotCount = 4;
        }
    }
}
