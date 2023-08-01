using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Projectiles.Weapons.Magic
{
	public class TorrentialLanceP : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Torrential Lance");
		}

		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 34;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 900;
			Projectile.tileCollide = true;
			Projectile.damage = 45;
            Projectile.aiStyle = -1;
			Projectile.DamageType = DamageClass.Magic;
		}

		public override bool PreAI()
		{
			Projectile.alpha -= 40;
			if (Projectile.alpha < 0)
				Projectile.alpha = 0;

			Projectile.spriteDirection = Projectile.direction;
			return true;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
		public override void AI()
		{
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 20)
			{
				Projectile.frameCounter = 0;
				float rotation = (float)(Main.rand.Next(0, 361) * (Math.PI / 180));
				Vector2 velocity = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
				int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, velocity.X, velocity.Y, ProjectileID.FlaironBubble, Projectile.damage, Projectile.owner, 0, 0f);
				Main.projectile[proj].friendly = true;
				Main.projectile[proj].hostile = false;
				Main.projectile[proj].velocity *= 7f;
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
			Projectile.localAI[0] += 1f;
			if (Projectile.localAI[0] == 16f)
			{
				Projectile.localAI[0] = 0f;
				for (int j = 0; j < 10; j++)
				{
					Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
					vector2 += -Utils.RotatedBy(Vector2.UnitY, ((float)j * 3.141591734f / 6f), default(Vector2)) * new Vector2(8f, 16f);
					vector2 = Utils.RotatedBy(vector2, (Projectile.rotation - 1.57079637f), default(Vector2));
					int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 172, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
					Main.dust[num8].scale = 1.3f;
					Main.dust[num8].noGravity = true;
					Main.dust[num8].position = Projectile.Center + vector2;
					Main.dust[num8].velocity = Projectile.velocity * 0.1f;
					Main.dust[num8].noLight = true;
					Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
				}
			}
			int num1222 = 74;
			for (int k = 0; k < 2; k++)
			{
				int index2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 172, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				Main.dust[index2].position = Projectile.Center - Projectile.velocity / num1222 * (float)k;
				Main.dust[index2].scale = .95f;
				Main.dust[index2].velocity *= 0f;
				Main.dust[index2].noGravity = true;
				Main.dust[index2].noLight = false;
			}
		}
		public override void PostDraw(Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, (Projectile.height / Main.projFrames[Projectile.type]) * 0.5f);
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Projectiles/Weapons/Magic/TorrentialLanceP").Value;
            float num108 = 4;
			float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + 0.5f;
			float num106 = 0f; SpriteEffects spriteEffects3 = (Projectile.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Vector2 vector33 = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
			Color color29 = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Blue);
			for (int num103 = 0; num103 < 4; num103++)
			{
				Color color28 = color29;
				color28 = Projectile.GetAlpha(color28);
				color28 *= 1f - num107;
				Vector2 vector29 = new Vector2(Projectile.Center.X, Projectile.Center.Y) + ((float)num103 / (float)num108 * 6.28318548f + Projectile.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * (float)num103;
				Main.spriteBatch.Draw(texture2D4, vector29, new Microsoft.Xna.Framework.Rectangle?(TextureAssets.Projectile[Projectile.type].Value.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color28, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects3, 0f);
			}
		}
		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 172, 0f, -2f, 0, default(Color), .8f);
				Main.dust[num1].noGravity = true;
				Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				if (Main.dust[num1].position != Projectile.Center)
					Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 172, 0f, -2f, 0, default(Color), .8f);
				Main.dust[num].noGravity = true;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
				SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			}
		}
	}

}