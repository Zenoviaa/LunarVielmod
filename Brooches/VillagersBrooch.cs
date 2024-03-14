using Terraria;

namespace Stellamod.Brooches
{
    public class VillagersBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasVillagersBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}