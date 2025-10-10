using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class LeadWand : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 8;
            Item.shootSpeed = 10;
            Item.useTime = 18;
            Item.useAnimation = 36;
            Size = 8;
            TrailLength = 16;
        }

        public override int GetNormalSlotCount()
        {
            return 2;
        }

        public override int GetTimedSlotCount()
        {
            return 0;
        }
    }
}

