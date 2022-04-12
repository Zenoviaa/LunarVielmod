using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
	public class PlantDaggerS : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Splantdagger Dagger");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			ProjectileID.Sets.MinionShot[Projectile.type] = true;
			Main.projFrames[Projectile.type] = 4;
		}

		private readonly int maxtimeleft = 30;
		public override void SetDefaults()
		{
			Projectile.timeLeft = maxtimeleft;
			Projectile.friendly = true;
			Projectile.height = 12;
			Projectile.width = 12;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			Projectile.extraUpdates = 1;
			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 5;
		}

		float alphaCounter;
		public override void AI()
		{
			alphaCounter += 0.08f;
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

			if (Projectile.timeLeft > 3 * (maxtimeleft / 4))
				Projectile.alpha = Math.Max(Projectile.alpha - (255 / (maxtimeleft / 4)), 0);

			else if (Projectile.timeLeft < (maxtimeleft / 4))
				Projectile.alpha = Math.Min(Projectile.alpha + (255 / (maxtimeleft / 4)), 255);

			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = (Projectile.frame + 1) % 4;
			}
		}



		public void AdditiveCall(SpriteBatch spriteBatch)
		{
			float sineAdd = (float)Math.Sin(alphaCounter) + 2f;
			{
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Color color = new Color(255, 179, 246) * 0.75f * Projectile.Opacity * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

					float scale = Projectile.scale;
					Texture2D tex = ModContent.Request<Texture2D>("Stellamod/Projectiles/SacrificialDagger/PlantDagger_Trail").Value;
					Texture2D glowtex = ModContent.Request<Texture2D>("Stellamod/Projectiles/SacrificialDagger/PlantDagger_Trail").Value;
					spriteBatch.Draw(glowtex, Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition, null, color * sineAdd, Projectile.rotation, glowtex.Size() / 2, scale, default, default);
					
				}
			}
		}

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor); 
        }
    }
}