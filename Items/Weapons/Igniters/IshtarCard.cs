using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class IshtarCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 19;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 20;
        }
    }
}