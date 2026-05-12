using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class PlatinumWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 35;
            Item.shootSpeed = 10;
            Item.mana = 35;
            Size = 8;
            TrailLength = 16;
            normalSlotCount = 0;
            timedSlotCount = 3;
        }
    }
}