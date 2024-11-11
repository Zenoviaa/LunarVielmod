using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;

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

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.LightGoldenrodYellow, endRadius: 96);
            }
        }
    }
}