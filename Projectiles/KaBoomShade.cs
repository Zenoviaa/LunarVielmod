using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;

namespace Stellamod.Projectiles
{
    public class KaBoomShade : BaseIgniterExplosion
    {
        public override int FrameCount => 33;

        public override bool BlackIsTransparency => false;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Purple);
            }
        }
    }
}