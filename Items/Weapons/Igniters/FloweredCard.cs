using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class FloweredCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 2;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 3;
        }
    }
}