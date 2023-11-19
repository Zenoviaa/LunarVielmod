using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class Meatball4 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Balls");
			Main.projFrames[Projectile.type] = 32;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 45;
			Projectile.height = 45;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 32;
			Projectile.scale = 0.7f;
			
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
        public override void AI()
        {

			Vector3 RGB = new(2.55f, 2.55f, 0.94f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
			Timer++;
			Player player = Main.player[Projectile.owner];
			if (player.noItems || player.CCed || player.dead || !player.active)
				Projectile.Kill();

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
			float swordRotation = 0f;
			if (Main.myPlayer == Projectile.owner)
			{
				player.ChangeDir(Projectile.direction);
				swordRotation = (Main.MouseWorld - player.Center).ToRotation();
			}

			Projectile.velocity = swordRotation.ToRotationVector2();
			Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position			
			Projectile.rotation += 0.1f;
		}
		
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 32)
				{
					Projectile.frame = 0;
				}
			}
			

	


			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob2>(), 0f, 0f);
			Main.dust[dust].scale = 1f;
			return true;
		}

	}
	
	}
