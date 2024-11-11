using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Visual;
using Terraria;
using Stellamod.Helpers;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class JungleBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 10;
        public override void SetDefaults()
        {
            base.SetDefaults();
            FrameSpeed = 0.5f;
        }

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