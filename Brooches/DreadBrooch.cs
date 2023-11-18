using Terraria;

namespace Stellamod.Brooches
{
    public class DreadBrooch : BroochDefaultProjectile
	{
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasDreadBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}