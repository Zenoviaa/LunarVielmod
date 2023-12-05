using Stellamod.Items.Accessories.Brooches;
using Terraria;

namespace Stellamod.Brooches
{
    public class BonedBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasBonedBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}