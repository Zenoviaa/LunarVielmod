using Microsoft.Xna.Framework;
using Terraria;
using Stellamod.Core.Helpers;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
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
