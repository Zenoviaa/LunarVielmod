using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using Terraria;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    internal class SepsisExSps : BaseIgniterExplosion
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
