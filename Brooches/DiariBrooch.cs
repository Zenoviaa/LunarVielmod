using Terraria;

namespace Stellamod.Brooches
{
    public class DiariBrooch : BroochDefaultProjectile
	{
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasDiariBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}