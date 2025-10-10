using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic.Movements
{
    public class CircularMovement : BaseMovement
    {
        // public float maxHomingDetectDistance = 512;
        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(9));
        }
    }
}
