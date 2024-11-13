using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class LarvaedCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 7;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 13;
        }
    }
}