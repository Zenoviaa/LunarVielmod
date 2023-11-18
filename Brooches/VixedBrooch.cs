using Terraria;

namespace Stellamod.Brooches
{
    public class VixedBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasVixedBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}