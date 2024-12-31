using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class GothiviasCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 3;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 6;
        }
    }
}