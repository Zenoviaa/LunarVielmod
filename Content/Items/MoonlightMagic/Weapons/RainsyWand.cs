using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class RainsyWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 12;
            Item.shootSpeed = 14;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Size = 10;
            TrailLength = 16;
            Form = FormRegistry.Pickaxe.Value;
            normalSlotCount = 3;
            timedSlotCount = 0;
        }
    }
}
