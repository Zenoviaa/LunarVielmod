using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using static Terraria.ModLoader.ModContent;


namespace Stellamod.Particles
{
    public class ZappedParticle : Particle
	{
		public override void SetDefaults()
		{
			width = 34;
			height = 34;
			Scale = 1f;
			timeLeft = 15;
			oldPos = new Vector2[10];
			oldRot = new float[1];
			SpawnAction = Spawn;
		}
		public override void AI()
		{
			Vector3 RGB = new(1.45f, 2.55f, 0.94f);
			Lighting.AddLight(position, RGB.X, RGB.Y, RGB.Z);
			scale *= 1.1f;

			rotation += Utils.Clamp(velocity.X * 0f, -ai[0], ai[0]);
			velocity *= 0.98f;
			color = Color.Lerp(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), Color.Multiply(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), 0.5f), (360f - timeLeft) / 360f);

			if (Scale <= 0f)
				active = false;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
		{
			Texture2D tex = Request<Texture2D>("Stellamod/Particles/ZappedParticle2").Value;
			float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;

			if (alpha < 0f) 
				alpha = 0f;
			
			Color color = Color.Multiply(new(0.50f, 2.05f, 0.5f, 0), alpha);
			
			spriteBatch.Draw(tex, position - Main.screenPosition, new Rectangle(0, 0, tex.Width, tex.Height), color, MathHelper.ToRadians(ai[0]).AngleLerp(MathHelper.ToRadians((ai[0] * 180f)), (120f - timeLeft) / 120f), new Vector2(tex.Width / 2f, tex.Height / 2f), 0.1f * scale, SpriteEffects.None, 0f);
			return false;
		}
		public void Spawn()
		{
			ai[1] = Main.rand.NextFloat(2f, 8f) / 1f;
			ai[2] = Main.rand.Next(0, 2);
			ai[3] = Main.rand.NextFloat(0f, 20f);
			timeLeft = (int)ai[4] > 0 ? (int)ai[4] : timeLeft;
		}
	}
}