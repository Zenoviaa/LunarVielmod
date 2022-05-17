using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static Terraria.ModLoader.ModContent;
using ParticleLibrary;
using Stellamod.Particles;


namespace Stellamod.Particles
{


	public class ZappedParticle : Particle
	{
		int frameCount;
		int frameTick;
		public override void SetDefaults()
		{
			width = 34;
			height = 34;
			scale = 1f;
			timeLeft = 15;
			oldPos = new Vector2[10];
			oldRot = new float[1];
			texture = Request<Texture2D>("Stellamod/Particles/ZappedParticle").Value;
		
		}
		public override void AI()
		{
			Vector3 RGB = new Vector3(1.45f, 2.55f, 0.94f);
			float multiplier = 1;
			float max = 2.25f;
			float min = 1.0f;
			RGB *= multiplier;
			if (RGB.X > max)
			{
				multiplier = 0.5f;
			}
			if (RGB.X < min)
			{
				multiplier = 1.5f;
			}
			Lighting.AddLight(position, RGB.X, RGB.Y, RGB.Z);
			scale *= 1.1f;
			for (int i = oldPos.Length - 1; i > 0; i--)
			{
				oldPos[i] = oldPos[i - 1];
			}
			oldPos[0] = position;
			for (int i = oldRot.Length - 1; i > 0; i--)
			{
				oldRot[i] = oldRot[i - 1];
			}
			oldRot[0] = rotation;
			if (ai[0] == 0)
			{
				ai[1] = Main.rand.NextFloat(2f, 8f) / 1f;
				ai[2] = Main.rand.Next(0, 2);
				ai[3] = Main.rand.NextFloat(0f, 20f);
				timeLeft = (int)ai[4] > 0 ? (int)ai[4] : timeLeft;
			}

			rotation += Utils.Clamp(velocity.X * 0f, -ai[0], ai[0]);
			velocity *= 0.98f;
			color = Color.Lerp(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), Color.Multiply(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), 0.5f), (360f - timeLeft) / 360f);

			if (scale <= 0f)
				active = false;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
		{
			Texture2D tex = Request<Texture2D>("Stellamod/Particles/ZappedParticle").Value;
			Texture2D tex2 = Request<Texture2D>("Stellamod/Particles/ZappedParticle").Value;
			Texture2D tex3 = Request<Texture2D>("Stellamod/Particles/ZappedParticle").Value;
			float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
			if (alpha < 0f) alpha = 0f;
			Color color = Color.Multiply(new(0.50f, 2.05f, 0.5f, 0), alpha);
			spriteBatch.Draw(tex2, position - Main.screenPosition, new Rectangle(0, 0, tex2.Width, tex2.Height), color, ai[0].InRadians().AngleLerp((ai[0] * 180f).InRadians(), (120f - timeLeft) / 120f), new Vector2(tex2.Width / 2f, tex2.Height / 2f), 0.1f * scale, SpriteEffects.None, 0f);
			//spriteBatch.Draw(tex3, position - Main.screenPosition, new Rectangle(0, 0, tex3.Width, tex3.Height), color, ai[0].InRadians().AngleLerp((ai[0] + 90f).InRadians(), (120f - timeLeft) / 120f), new Vector2(tex3.Width / 2f, tex3.Height / 2f), 0.2f * scale, SpriteEffects.None, 0f);
			//spriteBatch.Draw(tex, position - Main.screenPosition, tex.AnimationFrame(ref frameCount, ref frameTick, 4, 7, true), color, 0f, new Vector2(width / 2, height / 2), 1f, SpriteEffects.None, 0f);
			return false;
		}
	}
}