using Terraria;

namespace Stellamod.Brooches
{
    public class StoneBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasStoneBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}