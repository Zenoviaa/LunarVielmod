using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class LeadWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 32;
            Item.mana = 30;
            Item.shootSpeed = 8;
            Size = 16;
            TrailLength = 24;
            normalSlotCount = 0;
            timedSlotCount = 2;
        }
    }
}

