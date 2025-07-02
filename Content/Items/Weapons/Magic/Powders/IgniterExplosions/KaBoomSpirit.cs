using Microsoft.Xna.Framework;
using Terraria;
using Stellamod.Core.Helpers;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    public class KaBoomSpirit : BaseIgniterExplosion
    {
        public override int FrameCount => 16;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.ArmorPenetration += 20;
        }
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
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Purple);
            }
        }
    }
}