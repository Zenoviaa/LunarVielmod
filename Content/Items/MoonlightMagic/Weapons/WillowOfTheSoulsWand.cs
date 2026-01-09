using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class WillowOfTheSoulsWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 16;
            Item.shootSpeed = 7;
            Size = 24;
            TrailLength = 55;
            Form = FormRegistry.Crescent.Value;
            normalSlotCount = 3;
            timedSlotCount = 2;
        }
    }
}
