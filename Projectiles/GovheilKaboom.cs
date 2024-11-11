using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;

namespace Stellamod.Projectiles
{
    public class GovheilKaboom : BaseIgniterExplosion
    {
        public override int FrameCount => 16;
        public override void SetExplosionDefaults()
        {
            base.SetExplosionDefaults();
            FrameSpeed = 0.5f;
        }

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.LightSeaGreen, endRadius: 70);
            }
        }
    }
}