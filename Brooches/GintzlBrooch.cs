using Terraria;

namespace Stellamod.Brooches
{
    public class GintzlBrooch : BroochDefaultProjectile
	{
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasGintzlBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}