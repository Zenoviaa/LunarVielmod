using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class MetallianCard : BaseIgniterCard
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
            Item.damage = 4;
        }

        public override int GetPowderSlotCount()
        {

            return 2;
        }
    }
}