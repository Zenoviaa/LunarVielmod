using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class SepsisExSps : BaseIgniterExplosion
    {
        public override int FrameCount => 23;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Orange);
            }
        }
    }
}
