using Terraria;

namespace Stellamod.Brooches
{
    public class MorrowedBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasMorrowedBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}