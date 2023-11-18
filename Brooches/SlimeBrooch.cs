using Terraria;

namespace Stellamod.Brooches
{
    public class SlimeBrooch : BroochDefaultProjectile
    {
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasSlimeBrooch)
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}