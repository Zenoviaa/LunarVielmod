using Stellamod.Core.PowderSystem;
using Terraria;

namespace Stellamod.Content.Items.Weapons.Magic.Cards
{
    internal class BOMBERCard : BaseIgniterCard
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 10;
        }
    }
}