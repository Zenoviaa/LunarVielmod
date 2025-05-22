using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    public class DragonWaveFriendly : ModProjectile
	{
		public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragon Wave");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.alpha = 255;
			Projectile.width = 80;
			Projectile.height = 80;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 10;
			Projectile.timeLeft = 900;
			Projectile.tileCollide = true;
			Projectile.damage = 45;
			Projectile.aiStyle = -1;
			Projectile.scale = 0.9f;
		}

		public override bool PreAI()
		{
			Projectile.alpha -= 40;
			if (Projectile.alpha < 0)
				Projectile.alpha = 0;

			Projectile.spriteDirection = Projectile.direction;
			return true;
		}
		float alphaCounter;
		public override void AI()
		{
			Projectile.alpha -= 2;
			alphaCounter += 0.04f;
			Projectile.velocity /= 0.99f;
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
			Projectile.localAI[0] += 1f;
		}
		public override bool PreDraw(ref Color lightColor)
		{
            Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.Blue) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
	}
}