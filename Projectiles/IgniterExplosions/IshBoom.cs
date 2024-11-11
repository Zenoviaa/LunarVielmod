using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class IshBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 61;

        public override bool BlackIsTransparency => false;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.White, endRadius: 128);
            }
        }
    }
}