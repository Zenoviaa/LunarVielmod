using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;


namespace Stellamod.Projectiles.Magic
{
	public class RadiantOrb : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private ref float SwordRotation => ref Projectile.ai[1];
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("SalfaCirle");
			Main.projFrames[Projectile.type] = 72;
		}

		private int _frameCounter;
		private int _frameTick;
		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 99;
			Projectile.height = 99;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			DrawOriginOffsetY = 0;
			Projectile.damage = 0;
			Projectile.timeLeft = 72;

		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
		}

		public int It = 0;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 drawPosition = Projectile.Center - Main.screenPosition;

			float width = 99;
			float height = 99;
			Vector2 origin = new Vector2(width / 2, height / 2);
			int frameSpeed = 1;
			int frameCount = 72;
			SpriteBatch spriteBatch = Main.spriteBatch;
			spriteBatch.Draw(texture, drawPosition,
				texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
				(Color)GetAlpha(lightColor), 0f, origin, 1.8f, SpriteEffects.None, 0f);
			return false;
		}


		public override bool PreAI()
		{
			if (++_frameTick >= 1)
			{
				_frameTick = 0;
				if (++_frameCounter >= 72)
				{
					_frameCounter = 0;
				}
			}

			Projectile.tileCollide = false;
			return true;
		}

		public override void AI()
		{

			It++;
			Player player = Main.player[Projectile.owner];

			if (It >= 6)
			{

				float speedX = Projectile.velocity.X * 12;
				float speedY = Projectile.velocity.Y * 12;

				Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center.X, player.Center.Y, speedX, speedY, ModContent.ProjectileType<GoldenHoes>(), (int)(Projectile.damage), 0f, Projectile.owner, 0f, 0f);
				It = 0;
			}


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

			Projectile.Center = playerCenter + new Vector2(65, 0).RotatedBy(SwordRotation);

			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 72)
				{
					Projectile.frame = 0;
				}
			}
		}
	}
}



