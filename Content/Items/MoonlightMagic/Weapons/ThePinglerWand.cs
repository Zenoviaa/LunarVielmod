using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class ThePinglerWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 6;
            Item.shootSpeed = 7;
            Size = 16;
            TrailLength = 12;
            Form = FormRegistry.Circle.Value;
            normalSlotCount = 2;
            timedSlotCount = 0;
        }
    }
}
