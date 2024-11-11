namespace Stellamod.Projectiles.IgniterExplosions
{
    public class EldritchBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 8;

        public override void SetDefaults()
        {
            base.SetDefaults();
            FrameSpeed = 0.5f;
        }
    }
}