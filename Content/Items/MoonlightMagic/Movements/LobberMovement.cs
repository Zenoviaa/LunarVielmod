using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic.Movements
{
    public class LobberMovement : BaseMovement
    {
        public override void AI()
        {

            Projectile.velocity.Y += 0.2f;
        }
    }
}
