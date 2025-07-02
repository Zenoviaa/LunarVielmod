using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using Terraria;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    public class CrystalBloom : BaseIgniterExplosion
    {
        public override int FrameCount => 60;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.ArmorPenetration += 10;
        }

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Purple, endRadius: 70);
            }
        }
    }
}