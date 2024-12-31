
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Spears
{
    public class TridentOfTheSeasProg : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Trident Of The Seas");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = 2;
			Projectile.timeLeft = 240;
			Projectile.height = 48;
			Projectile.width = 48;
			AIType = ProjectileID.Bullet;
			Projectile.extraUpdates = 1;
		}
		int counter;
		public override void AI()
		{
			Projectile.velocity /= 0.99f;
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;

			counter++;
			if (counter >= 1440)
			{
				counter = -1440;
			}
			for (int i = 0; i > 4; i++)
			{
				float x = Projectile.Center.X + Projectile.velocity.X * 20f * i;
				float y = Projectile.Center.Y + Projectile.velocity.Y * 20f * i;
			}
		}
		public override bool PreDraw(ref Color lightColor)
        {


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
		}
		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
			for (int i = 0; i < 10; i++)
			{
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, -2f, 0, default(Color), 1.1f);
				Main.dust[num].noGravity = true;
				Dust expr_62_cp_0 = Main.dust[num];
				expr_62_cp_0.position.X = expr_62_cp_0.position.X + (Main.rand.Next(-30, 31) / 20 - 1.5f);
				Dust expr_92_cp_0 = Main.dust[num];
				expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + (Main.rand.Next(-30, 31) / 20 - 1.5f);
				if (Main.dust[num].position != Projectile.Center)
				{
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
				}
			}
		}
	}
}