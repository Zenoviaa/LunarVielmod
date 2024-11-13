using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class IllureCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 9;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 18;
        }
    }
}