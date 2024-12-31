using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;



namespace Stellamod.Projectiles.IgniterExplosions
{
	public class BongoBoom : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 60;
		}

		private int _frameCounter;
		private int _frameTick;
		public override void SetDefaults()
		{
			Projectile.localNPCHitCooldown = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;
			Projectile.width = 128;
			Projectile.height = 128;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 60;
			Projectile.scale = 1f;
			Projectile.tileCollide = false;
		}

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public override void AI()
		{

			Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
			Projectile.tileCollide = false;
		}



		public override bool PreAI()
		{
			if (++_frameTick >= 1)
			{
				_frameTick = 0;
				if (++_frameCounter >= 60)
				{
					_frameCounter = 0;
				}
			}
			return true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 drawPosition = Projectile.Center - Main.screenPosition;

			float width = 128;
			float height = 128;
			Vector2 origin = new Vector2(width / 2, height / 2);
			int frameSpeed = 1;
			int frameCount = 60;
			SpriteBatch spriteBatch = Main.spriteBatch;
			spriteBatch.Draw(texture, drawPosition,
				texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
				(Color)GetAlpha(lightColor), 0f, origin, 1f, SpriteEffects.None, 0f);
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
		}


	}

}