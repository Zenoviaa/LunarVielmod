using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
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