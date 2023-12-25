using Terraria;

namespace Stellamod.Brooches
{
    public class MagicalBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasMagicalBrooch || !broochPlayer.hasAdvancedBrooches)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}