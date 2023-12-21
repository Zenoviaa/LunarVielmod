using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Trap.Cogwork
{
    internal class IronNailLodged : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Projectile.light = 0.75f;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			Vector2 drawOrigin = new Vector2(58 / 2, 16 / 2);

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
			float rotationOffset = VectorHelper.Osc(1f, 2f, 5);
			time %= 4f;
			time /= 2f;

			if (time >= 1f)
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;
			SpriteEffects effects = SpriteEffects.None;
			Color rotatedColor = new Color(60, 0, 118, 75);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			for (float i = 0f; i < 2f; i += 0.25f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				Vector2 rotatedPos = drawPos + new Vector2(0f, 8f * rotationOffset).RotatedBy(radians) * time;
				Main.spriteBatch.Draw(texture, rotatedPos,
					null,
				rotatedColor, 0f, drawOrigin, 1f, effects, 0f);
			}

			for (float i = 0f; i < 2f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				Vector2 rotatedPos = drawPos + new Vector2(0f, 16f * rotationOffset).RotatedBy(radians) * time;
				Main.spriteBatch.Draw(texture, rotatedPos,
					null,
					rotatedColor, 0f, drawOrigin, 1f, effects, 0f);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			return base.PreDraw(ref lightColor);
        }

		public override void AI()
        {
            //Pretty sure projectiles automatically have regular velocity so we don't need to do anything here.
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 32; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Iron, speed, Scale: 2f);
                d.noGravity = true;
            }
        }
    }
}
