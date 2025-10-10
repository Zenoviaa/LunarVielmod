using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class CelestiaWand : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 14;
            Item.shootSpeed = 12;
            Item.useTime = 15;
            Item.useAnimation = 30;
            Size = 12;
            TrailLength = 18;
        }

        public override int GetNormalSlotCount()
        {
            return 2;
        }

        public override int GetTimedSlotCount()
        {
            return 2;
        }
    }
}