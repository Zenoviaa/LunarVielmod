using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
    public class VerliaText2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sigil");

		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.width = 120;
			Projectile.height = 43;
			Projectile.penetrate = -1;
			Projectile.scale = 1.5f;
			Projectile.timeLeft = 220;

		}
		public int Timer = 0;

		public override void AI()
		{
			Timer++;

			Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
			

				Projectile.alpha++;

			


		}




	}
}