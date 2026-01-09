using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class CelestiaWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 14;
            Item.shootSpeed = 12;
            Item.useTime = 15;
            Item.useAnimation = 30;
            Size = 12;
            TrailLength = 18;
            normalSlotCount = 2;
            timedSlotCount = 2;
        }
    }
}