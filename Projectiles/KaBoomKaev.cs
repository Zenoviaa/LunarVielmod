using Stellamod.Projectiles.IgniterExplosions;

namespace Stellamod.Projectiles
{
    public class KaBoomKaev : BaseIgniterExplosion
    {
        public override int FrameCount => 8;
        public override void SetDefaults()
        {
            base.SetDefaults();
            FrameSpeed = 0.5f;
        }
    }
}