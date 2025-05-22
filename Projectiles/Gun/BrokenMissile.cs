

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    public class BrokenMissile : ModProjectile
	{
		private ref float Style => ref Projectile.ai[1];
		public bool OptionallySomeCondition { get; private set; }
		private Color MainColor
		{
			get
			{
				if(Style == 1)
				{
					return Color.Cyan;
				}
				return Color.Orange;
			}
		}


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
			Projectile.penetrate = 1;
			Projectile.width = 15;
			Projectile.height = 15;
			Projectile.extraUpdates += 1;
		}
		public override void AI()
		{
			Projectile.velocity *= 1.01f;
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
				Color color = Projectile.GetAlpha(Color.Lerp(MainColor, Color.Transparent, 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (float f = 0; f < 30; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, MainColor, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: MainColor,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.02f, 0.16f),
					duration: Main.rand.NextFloat(12, 24));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
	}
}