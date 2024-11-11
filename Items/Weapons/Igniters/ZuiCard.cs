using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class ZuiCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 10;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 21;
        }
    }
}