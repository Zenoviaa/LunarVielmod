using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.Visual;
using Terraria;

namespace Stellamod.Projectiles.IgniterExplosions
{
    internal class VerstiExSps : BaseIgniterExplosion
    {
        public override int FrameCount => 15;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Green);
            }
        }
    }
}
