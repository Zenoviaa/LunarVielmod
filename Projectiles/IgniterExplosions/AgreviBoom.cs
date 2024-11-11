namespace Stellamod.Projectiles.IgniterExplosions
{
    public class AgreviBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 15;
        public override void SetDefaults()
        {
            base.SetDefaults();
            DrawScale = 1f;
        }
    }
}