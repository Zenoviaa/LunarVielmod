using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class LeadWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 8;
            Item.shootSpeed = 10;
            Item.useTime = 18;
            Item.useAnimation = 36;
            Size = 8;
            TrailLength = 16;
            normalSlotCount = 2;
            timedSlotCount = 0;
        }
    }
}

