using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class SalfaCircle3 : ModProjectile
	{
		private ref float SwordRotation => ref Projectile.ai[1];
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("SalfaCirle3");
			Main.projFrames[Projectile.type] = 9;
		}
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.penetrate = 100;
			Projectile.scale = 1.7f;
			DrawOriginOffsetY = -55;
			Projectile.damage = 0;
			Projectile.timeLeft = 120;
			DrawOriginOffsetY = -20;
			Projectile.rotation = 45;
		}
		public override bool PreAI()
		{
			Projectile.scale *= 0.96f;
			Projectile.tileCollide = false;

			int evenmoredust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SalfaceDust>());
			int moredust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BubbleBurst_Green);
			Main.dust[evenmoredust].scale = 0.5f;
			Main.dust[moredust].scale = 0.6f;

			return true;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(26, 38, 22, 0) * (1f - Projectile.alpha / 50f);
		}
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			if (player.noItems || player.CCed || player.dead || !player.active)
				Projectile.Kill();

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
			if (Main.myPlayer == Projectile.owner)
			{
				player.ChangeDir(Projectile.direction);
				SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
				Projectile.netUpdate = true;
				if (!player.channel)
					Projectile.Kill();
			}
			Projectile.velocity = SwordRotation.ToRotationVector2();

			Projectile.spriteDirection = player.direction;
			if (Projectile.spriteDirection == 1)
				Projectile.rotation = Projectile.velocity.ToRotation();
			else
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

			Projectile.Center = playerCenter + new Vector2(100, 0).RotatedBy(SwordRotation);

			if (++Projectile.frameCounter >= 10)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 9)
				{
					Projectile.frame = 0;
				}
			}
		}
	}
}



