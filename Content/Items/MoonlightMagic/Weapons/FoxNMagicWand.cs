using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class FoxNMagicWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 1000;
            Item.shootSpeed = 13;
            Item.useTime = 30;
            Item.useAnimation = 60;
            Size = 12;
            TrailLength = 20;
            Form = FormRegistry.Swirl.Value;
            normalSlotCount = 5;
            timedSlotCount = 4;
        }
    }
}
