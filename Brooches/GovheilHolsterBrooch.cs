using Terraria;

namespace Stellamod.Brooches
{
    public class GovheilHolsterBrooch : BroochDefaultProjectile
	{
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasGovheilHolsterBrooch || !broochPlayer.hasAdvancedBrooches)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}