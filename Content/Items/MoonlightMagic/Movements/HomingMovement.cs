using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic.Movements
{
    public class HomingMovement : BaseMovement
    {
        public float maxHomingDetectDistance = 512;
        public override void AI()
        {
            NPC npcToChase = ProjectileHelper.FindNearestEnemy(Projectile.Center, maxHomingDetectDistance);
            if (npcToChase != null)
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, npcToChase.Center, degreesToRotate: 10);
        }
    }
}
