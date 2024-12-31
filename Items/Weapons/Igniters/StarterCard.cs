using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class StarterCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 0;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 1;
            
        }

        public override int GetPowderSlotCount()
        {
            
            return 1;
        }
    }
}