using Terraria;

namespace Stellamod.Brooches
{
    public class FrileBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasFrileBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}