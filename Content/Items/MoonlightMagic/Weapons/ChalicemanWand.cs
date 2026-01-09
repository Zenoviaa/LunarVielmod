using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class ChalicemanWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 28;
            Item.shootSpeed = 14;
            Item.useTime = 20;
            Item.useAnimation = 40;
            Size = 10;
            TrailLength = 22;
            Form = FormRegistry.Triangle.Value;
            normalSlotCount = 2;
            timedSlotCount = 4;
        }
    }
}
