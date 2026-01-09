using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class SnowflakeWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 12;
            Item.shootSpeed = 5;
            Size = 32;
            TrailLength = 64;
            Form = FormRegistry.FourPointedStar.Value;
            normalSlotCount = 2;
            timedSlotCount = 1;
        }
    }
}
