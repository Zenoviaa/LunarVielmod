using Terraria;

namespace Stellamod.Brooches
{
    public class BearBrooch : BroochDefaultProjectile
	{
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasBearBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}