using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class MothwingWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 9;
            Item.shootSpeed = 13;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Size = 8;
            TrailLength = 38;
            Form = FormRegistry.Fairy.Value;
            normalSlotCount = 2;
            timedSlotCount = 1;
        }
    }
}
