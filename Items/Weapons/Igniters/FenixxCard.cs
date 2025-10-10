using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    public class FenixxCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 13;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 26;
        }
    }
}