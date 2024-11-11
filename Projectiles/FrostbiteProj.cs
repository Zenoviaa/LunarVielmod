using Stellamod.Projectiles.IgniterExplosions;

namespace Stellamod.Projectiles
{
    public class FrostbiteProj : BaseIgniterExplosion
    {
        public override int FrameCount => 30;
        public override bool BlackIsTransparency => false;

        public override void SetDefaults()
        {
            base.SetDefaults();
            DrawScale = 1f;
        }

        public override void AI()
        {
            base.AI();
            DrawScale *= 0.98f;
        }
    }
}