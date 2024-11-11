using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class KaBoomSpirit : BaseIgniterExplosion
    {
        public override int FrameCount => 16;


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
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Purple);
            }
        }
    }
}