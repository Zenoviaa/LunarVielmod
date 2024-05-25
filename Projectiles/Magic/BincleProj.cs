using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
	public class BincleProj : ModProjectile
	{
		private ref float SwordRotation => ref Projectile.ai[0];
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("SalfaCirle");
			Main.projFrames[Projectile.type] = 24;
		}
		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 250;
			Projectile.height = 250;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			DrawOriginOffsetY = 0;
			Projectile.damage = 0;
			Projectile.timeLeft = 48;

		}
		public override bool PreAI()
		{

			Projectile.tileCollide = false;

			return true;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
		}

		public int It = 0;
		public override void AI()
		{

			It++;
			Player player = Main.player[Projectile.owner];

			if (It >= 6)
            {

				float speedX = Projectile.velocity.X * 12;
				float speedY = Projectile.velocity.Y * 12;

				Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center.X, player.Center.Y, speedX, speedY, ModContent.ProjectileType<BrincShot>(), (int)(Projectile.damage), 0f, Projectile.owner, 0f, 0f);
				It = 0;
            }

		
			if (player.noItems || player.CCed || player.dead || !player.active)
				Projectile.Kill();

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
			if (Main.myPlayer == Projectile.owner)
			{
				player.ChangeDir(Projectile.direction);
				SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
				if (!player.channel)
					Projectile.Kill();
				Projectile.netUpdate = true;
			}

			Projectile.velocity = SwordRotation.ToRotationVector2();

			Projectile.spriteDirection = player.direction;
			if (Projectile.spriteDirection == 1)
				Projectile.rotation = Projectile.velocity.ToRotation();
			else
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

			Projectile.Center = playerCenter + new Vector2(120, 0).RotatedBy(SwordRotation);

			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 24)
				{
					Projectile.frame = 0;
				}
			}
		}
	}
}



