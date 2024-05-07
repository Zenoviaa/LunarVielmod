using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Bosses.singularityFragment.Phase1;
using Stellamod.Projectiles.Summons;
using Stellamod.Trails;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using static Terraria.ModLoader.ModContent;



namespace Stellamod.Items.Accessories
{
	public class RadiantBomb : ModProjectile
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
			Main.projFrames[Projectile.type] = 60;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 106;
			Projectile.height = 106;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = int.MaxValue;
	
		
		}

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			bool hasSetBonus = player.GetModPlayer<MyPlayer>().RadiantBomb;
			if (!hasSetBonus)
			{
				Projectile.Kill();
				return;
			}


			Timer++;
			Projectile.rotation += 0.05f;
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
				//Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedXa * 0.1f, speedYa * 0.1f, ProjectileID.SpiritFlame, 10, 0f, Projectile.owner, 0f, 0f);
				TimerR = 0;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
		public PrimDrawer TrailDrawer { get; private set; } = null;
		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * (Projectile.width/ 2) * 0.5f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}
		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.SpringGreen, Color.Transparent, completionRatio) * 1f;
		}
		int counter = 1;
		float alphaCounter = 1;
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
			Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(25f * alphaCounter), (int)(5f * alphaCounter), 0), Projectile.rotation, new Vector2(106 / 2, 106 / 2), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(05f * alphaCounter), (int)(25f * alphaCounter), (int)(5f * alphaCounter), 0), Projectile.rotation, new Vector2(106 / 2, 106 / 2), 0.2f * (counter + 0.3f * 2), SpriteEffects.None, 0f);
			
			
			Vector2 frameSize = Projectile.Frame().Size();


			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, Projectile.Frame(), Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
			TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.WhispyTrail);
			TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);


			// just return false if you want only trail locked on player
			return true;
		}


		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 60)
				{
					Projectile.frame = 0;
				}
			}
			return true;


		}

		//Could probably do something like:
		//DrawHelper.AfterImagePreDraw(Projectile projectile, float saturation, float luminosity, Color afterImageColor);
		//return false;

	}
}