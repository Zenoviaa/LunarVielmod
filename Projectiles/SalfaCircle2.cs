using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
	public class SalfaCircle2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("SalfaCirle2");
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 48;
			Projectile.height = 55;
			Projectile.penetrate = 100;
			Projectile.scale = 1.3f;
			DrawOriginOffsetY = 10;
			Projectile.damage = 0;
			Projectile.timeLeft = 30;
		}
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			return true;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(0, 0, 0, 50) * (1f - (float)Projectile.alpha / 255f);
		}
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			if (player.noItems || player.CCed || player.dead || !player.active)
				Projectile.Kill();

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
			float swordRotation = 0f;
			if (Main.myPlayer == Projectile.owner)
			{
				player.ChangeDir(Projectile.direction);
				swordRotation = (Main.MouseWorld - player.Center).ToRotation();
				if (!player.channel)
					Projectile.Kill();
			}
			Projectile.velocity = swordRotation.ToRotationVector2();

			Projectile.spriteDirection = player.direction;
			if (Projectile.spriteDirection == 1)
				Projectile.rotation = Projectile.velocity.ToRotation();
			else
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

			Projectile.Center = playerCenter + new Vector2(120, 0).RotatedBy(swordRotation);

			if (++Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 4)
				{
					Projectile.frame = 0;
				}
			}
		}
	}
}



