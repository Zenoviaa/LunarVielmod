using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Zui.Projectiles
{
	public class GroundpoundSpike : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("GroudSpike");
			Main.projFrames[Projectile.type] = 38;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.width = 23;
			Projectile.height = 74;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 38;
			Projectile.scale = 1.1f;
			Projectile.damage = 90;
			Projectile.hostile = true;

		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public override void AI()
		{
			Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
			Vector3 RGB = new(2.55f, 2.55f, 0.94f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
			Projectile.spriteDirection = Projectile.direction;
		}

		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 38)
				{
					Projectile.frame = 0;
				}
			}
			return true;


		}

	}
}