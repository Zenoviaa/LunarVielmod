using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Projectiles.IgniterExplosions
{
    internal class GrassExSps : BaseIgniterExplosion
    {
        public override int FrameCount => 30;

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
