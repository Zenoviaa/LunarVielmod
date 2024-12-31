using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class ScissorianSlash : ModProjectile
	{
		private ref float SwordRotation => ref Projectile.ai[1];
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("SalfaCirle");
			Main.projFrames[Projectile.type] = 60;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 220;
			Projectile.height = 125;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			DrawOriginOffsetY = 0;
			Projectile.damage = 0;
			Projectile.timeLeft = 600;
			Projectile.localNPCHitCooldown = 5;
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

		public override void AI()
		{	
			Projectile.alpha++;
				
			Player player = Main.player[Projectile.owner];
			if (player.noItems || player.CCed || player.dead || !player.active)
				Projectile.Kill();

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
			if (Main.myPlayer == Projectile.owner)
			{

                SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
				Projectile.netUpdate = true;
				if (!player.channel)
					Projectile.Kill();
			}
			Projectile.velocity = SwordRotation.ToRotationVector2();

			
			if (Projectile.spriteDirection == 1)
				Projectile.rotation = Projectile.velocity.ToRotation();
			else
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

			Projectile.Center = playerCenter + new Vector2(180, 0).RotatedBy(SwordRotation);

			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 60)
				{
					Projectile.frame = 0;
				}
			}
		}
	}
}



