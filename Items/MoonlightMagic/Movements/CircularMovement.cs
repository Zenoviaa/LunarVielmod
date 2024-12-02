using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Items.MoonlightMagic.Movements
{
    internal class CircularMovement : BaseMovement
    {
        // public float maxHomingDetectDistance = 512;
        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(9));
        }
    }
}
