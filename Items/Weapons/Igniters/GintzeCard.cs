using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class GintzeCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 1;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 2;
        }
    }
}