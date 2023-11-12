
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    public class AcidBlast2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acid Blast");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 5;
			Projectile.timeLeft = 1000;
            Projectile.tileCollide = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.aiStyle = 1;
			AIType = ProjectileID.Bullet;
		}

		int timer;
		public override bool PreAI()
		{
            Projectile.ai[1]++;
            timer++;
            if (timer >= 100)
            {
                timer = 0;
            }

			if (Projectile.ai[1] == 1)
			{
				int Sound = Main.rand.Next(1, 3);
				if (Sound == 1)
				{
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedGreatBlade2"), Projectile.position);
				}
				else
				{
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedGreatBlade1"), Projectile.position);
				}
			}

            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 1.5f);
			Main.dust[dust].noGravity = true;

			int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 1.5f); 
			Main.dust[dust3].noGravity = true;
			return true;
		}

		public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(152, 208, 113), new Color(53, 107, 112), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch);
			int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch);

			//No need to set the velocity and then multiply by zero when you can just pass in 0 to the speed.
			//Main.dust[dust].velocity *= 0f;
			//Main.dust[dust1].velocity *= 0f;
			Main.dust[dust].noGravity = true;
			Main.dust[dust1].noGravity = true;

			//Returning true here kills the projectile, so no need to do it manually.
			return true;

			/*	
			Projectile.Kill();

			// This code just straight up doesn't execute
			// Main.rand.Next(1) == 1 will always be false, since rand is exclusive
			// The statement translates to if (0 == 1)

			if (Main.rand.Next(1) == 1)
			{
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				Main.dust[dust].velocity *= 0f;
				Main.dust[dust1].velocity *= 0f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust1].noGravity = true;
			}

			return false;
			*/
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 15; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch);
			}
			SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
		}
	}
}