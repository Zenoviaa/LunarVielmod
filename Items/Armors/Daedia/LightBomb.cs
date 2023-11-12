using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Armors.Daedia
{
    public class LightBomb : ModProjectile
	{
		/*
		 
		//This is a lot of unused variables!!!!
	
		private int ProjectileSpawnedCount = 0;
		private int ProjectileSpawnedMax = 20;
		private bool MouseRightBool = false;
		private bool Morrowflames = false;
		private bool MouseLeftBool = true;
		private object player;
		NPC target;

		int afterImgCancelDrawCount2 = 0;
		Vector2 endPoint;
		Vector2 controlPoint1;
		Vector2 controlPoint2;
		Vector2 initialPos;
		Vector2 wantedEndPoint;
		bool initialization = false;
		float AoERadiusSquared = 36000;//it's squared for less expensive calculations
		public bool[] hitByThisStardustExplosion = new bool[200] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };
		*/

		int afterImgCancelDrawCount = 0;
		float ta = 0;
		float TimerR = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Spragald");
			Main.projFrames[Projectile.type] = 1;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{		
			Projectile.width = 43;
			Projectile.height = 23;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 600;
			Projectile.scale = 0.9f;
			DrawOriginOffsetX = -110;
		}

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI()
		{
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
			Projectile.tileCollide = false;
			if (ta > 150)
			{
				afterImgCancelDrawCount++;
			}

			ta += 0.01f;
			TimerR++;
			if (TimerR == 100)
            {
				float speedXa = (Projectile.velocity.X / 2) + Main.rand.NextFloat(-10f, 10f);
				float speedYa = (Projectile.velocity.Y / 6) + Main.rand.Next(-10, 10);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedXa * 0.1f, speedYa * 0.1f, ProjectileID.SpiritFlame, 10, 0f, Projectile.owner, 0f, 0f);
				TimerR = 0;
            }
		}
		

		public override bool PreDraw(ref Color lightColor)
		{
			Color afterImgColor = Main.hslToRgb(Projectile.ai[1], 1, 0.5f);
			//float opacityForSparkles = 1 - (float)afterImgCancelDrawCount / 30;
			afterImgColor.A = 40;
			afterImgColor.B = 125;
			afterImgColor.G = 125;
			afterImgColor.R = 125;
			afterImgColor.B--;

			Main.instance.LoadProjectile(ProjectileID.RainbowRodBullet);
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			for (int i = afterImgCancelDrawCount + 1; i < Projectile.oldPos.Length; i++)
			{
				//if(i % 2 == 0)
				float rotationToDraw;
				Vector2 interpolatedPos;
				for (float j = 0; j < 1; j += 0.25f)
				{
					if (i == 0)
					{
						rotationToDraw = Utils.AngleLerp(Projectile.rotation, Projectile.oldRot[0], j);
						interpolatedPos = Vector2.Lerp(Projectile.Center, Projectile.oldPos[0] + Projectile.Size / 2, j);
					}
					else
					{
						interpolatedPos = Vector2.Lerp(Projectile.oldPos[i - 1] + Projectile.Size / 2, Projectile.oldPos[i] + Projectile.Size / 2, j);
						rotationToDraw = Utils.AngleLerp(Projectile.oldRot[i - 1], Projectile.oldRot[i], j);
					}
					Main.EntitySpriteDraw(texture, interpolatedPos - Main.screenPosition + Projectile.Size / 2 + new Vector2(-20, -15), null, afterImgColor * (1 - i / (float)Projectile.oldPos.Length), rotationToDraw, texture.Size() / 2, 1, SpriteEffects.None, 0);
				}
			}


			//Could probably do something like:
			//DrawHelper.AfterImagePreDraw(Projectile projectile, float saturation, float luminosity, Color afterImageColor);
			//return false;
			return false;
		}
	}
}