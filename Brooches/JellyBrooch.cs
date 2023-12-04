using Terraria;

namespace Stellamod.Brooches
{
    public class JellyBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasJellyBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}