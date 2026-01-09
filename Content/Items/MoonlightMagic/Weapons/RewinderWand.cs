using Stellamod.Content.Items.MoonlightMagic.Forms;
namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class RewinderWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 24;
            Item.shootSpeed = 12;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Size = 20;
            TrailLength = 64;
            Form = FormRegistry.Circle.Value;
            normalSlotCount = 0;
            timedSlotCount = 5;
        }
    }
}
