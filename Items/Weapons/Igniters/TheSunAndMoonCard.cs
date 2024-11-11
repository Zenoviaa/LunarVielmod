using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class TheSunAndMoonCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 5;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 9;
        }
    }
}