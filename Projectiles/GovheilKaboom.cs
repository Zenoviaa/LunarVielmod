using Stellamod.Projectiles.IgniterExplosions;

namespace Stellamod.Projectiles
{
    public class GovheilKaboom : BaseIgniterExplosion
    {
        public override int FrameCount => 16;

        public override void SetDefaults()
        {
            base.SetDefaults();
            FrameSpeed = 0.5f;
        }
    }
}