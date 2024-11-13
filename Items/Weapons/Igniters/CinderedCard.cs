using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class CinderedCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 5;
        }
    }
}