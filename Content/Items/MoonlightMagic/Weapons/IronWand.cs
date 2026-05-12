
using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class IronWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 30;
            Item.mana = 50;
            Item.shootSpeed = 12;
            Size = 12;
            TrailLength = 18;
            normalSlotCount = 2;
            timedSlotCount = 0;
        }
    }
}
