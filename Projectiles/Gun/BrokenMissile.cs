

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    public class BrokenMissile : ModProjectile
	{
		public bool OptionallySomeCondition { get; private set; }

		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Broken Missile");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Bullet);
			AIType = ProjectileID.Bullet;
			Projectile.penetrate = 3;
			Projectile.width = 15;
			Projectile.height = 15;
		}
		public override void AI()
		{
			Projectile.velocity /= 0.99f;
		}
		public override bool PreAI()
		{
			int num1222 = 74;
			for (int k = 0; k < 2; k++)
			{
				int index2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CopperCoin, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				Main.dust[index2].position = Projectile.Center - Projectile.velocity / num1222 * k;
				Main.dust[index2].scale = .95f;
				Main.dust[index2].velocity *= 0f;
				Main.dust[index2].noGravity = true;
				Main.dust[index2].noLight = false;
			}
			return true;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.timeLeft = 1;
			return false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.Lerp(new Color(253, 255, 31), new Color(182, 83, 38), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
			for (int i = 0; i < 180; i++)
			{
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[num].noGravity = true;
				Main.dust[num].scale = 1.9f;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				{
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 10f;
				}
			}
			for (int i = 0; i < 80; i++)
			{
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, -2f, 0, default(Color), 2.5f);
				Main.dust[num].noGravity = true;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				{
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 10f;
				}
			}
		}
	}
}