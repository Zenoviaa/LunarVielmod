using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class ScorcheCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 4;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 7;
        }
    }
}
