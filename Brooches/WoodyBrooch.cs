using Terraria;

namespace Stellamod.Brooches
{
    internal class WoodyBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasWoodyBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}
