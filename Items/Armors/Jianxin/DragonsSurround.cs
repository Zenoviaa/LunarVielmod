using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;



namespace Stellamod.Items.Armors.Jianxin
{
	public class DragonsSurround : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 15;
		}

		private int _frameCounter;
		private int _frameTick;
		public override void SetDefaults()
		{
			Projectile.localNPCHitCooldown = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;
			Projectile.width = 350;
			Projectile.height = 350;
			Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
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
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.Center;

            bool hasSetBonus = owner.GetModPlayer<MyPlayer>().Waterwhisps;
            if (!hasSetBonus)
            {
                Projectile.Kill();
                return;
            }



            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

		}



		public override bool PreAI()
		{
			if (++_frameTick >= 3)
			{
				_frameTick = 0;
				if (++_frameCounter >= 15)
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

			float width = 350;
			float height = 350;
			Vector2 origin = new Vector2(width / 2, height / 2);
			int frameSpeed = 3;
			int frameCount = 15;
			SpriteBatch spriteBatch = Main.spriteBatch;
			spriteBatch.Draw(texture, drawPosition,
				texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
				(Color)GetAlpha(lightColor), 0f, origin, 0.5f, SpriteEffects.None, 0f);
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
		}


	}

}