
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Swords;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Reavestor
{
	public class ReavBig : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Wind Sythe");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.penetrate--;
			if (Projectile.penetrate <= 0)
				Projectile.Kill();
			else
			{

				if (Projectile.velocity.X != oldVelocity.X)
					Projectile.velocity.X = -oldVelocity.X;

				if (Projectile.velocity.Y != oldVelocity.Y)
					Projectile.velocity.Y = -oldVelocity.Y;


			}
			for (int i = 0; i < 15; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin);
			}
			return false;
		}

		public override void AI()
		{
			Projectile.ai[1]++;
			if (Projectile.ai[1] <= 1)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SkyrageShasher"), Projectile.position);
			}
			Projectile.rotation += 0.1f;
			if (Projectile.timeLeft >= 250 && Projectile.timeLeft <= 310)
			{
				Projectile.alpha -= 10;
			}

			Projectile.velocity *= 0.99f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			ShakeModSystem.Shake = 4;
			float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
			float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GoldsSpawnEffect>(), Projectile.damage * 0, 0f, Projectile.owner, 0f, 0f);
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GoldsSlashProj>(), (int)(Projectile.damage * 1.1), 0f, Projectile.owner, 0f, 0f);
			SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.position);
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

		public override void SetDefaults()
		{
			Projectile.scale = 1f;
			Projectile.rotation += 0.1f;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 310;
			Projectile.tileCollide = true;
			Projectile.penetrate = 6;
			Projectile.alpha = 130;
			Projectile.width = 56;
			Projectile.height = 70;
			AIType = ProjectileID.WoodenBoomerang;
		}
	}
}


