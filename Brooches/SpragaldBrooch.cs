using Terraria;

namespace Stellamod.Brooches
{
    public class SpragaldBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasSpragaldBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}