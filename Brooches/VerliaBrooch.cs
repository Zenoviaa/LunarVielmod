using Terraria;

namespace Stellamod.Brooches
{
    public class VerliaBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasVerliaBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}