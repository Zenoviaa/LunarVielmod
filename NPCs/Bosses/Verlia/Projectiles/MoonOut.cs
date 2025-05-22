using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
    public class MoonOut : ModProjectile
	{
		public int timer = 0;
		public int timer2 = 0;
		float t = 0;
		int afterImgCancelDrawCount = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Great Moon Blow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

		}
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = false;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 300;
			Projectile.ignoreWater = true;
			Projectile.hostile = true;
			Projectile.aiStyle = 2;
			Projectile.usesIDStaticNPCImmunity = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 120;
			Projectile.Size = new Vector2(10, 10);
		}
		public override void AI()
		{

			if (t > .75)
			{

				afterImgCancelDrawCount++;
			}
			
			t += 0.01f;
			Projectile.ai[0]++;
			
		}
		public override bool PreAI()
		{
			timer++; 


			Projectile.tileCollide = false;

			if (timer == 2)
            {
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0f, 0f);
				Main.dust[dust].scale = 0.6f;

				for (int i = 0; i < 1; i++)
				{
					Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DeepSkyBlue, 1f).noGravity = true;
				}
				for (int i = 0; i < 1; i++)
				{
					Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 0.3f).noGravity = true;
				}

				timer = 0;
			}
			
		
			

			return true;
		}


		public override bool PreDraw(ref Color lightColor)
		{
			Color afterImgColor = Main.hslToRgb(Projectile.ai[1], 1, 0.5f);
			float opacityForSparkles = 1 - (float)afterImgCancelDrawCount / 30;
			afterImgColor.A = 70;
			afterImgColor.B = 107;
			afterImgColor.G = 107;
			afterImgColor.R = 107;
			Main.instance.LoadProjectile(ProjectileID.SuperStar);
			Texture2D texture = TextureAssets.Projectile[ProjectileID.SuperStar].Value;
			for (int i = afterImgCancelDrawCount + 1; i < Projectile.oldPos.Length; i++)
			{
				//if(i % 2 == 0)
				float rotationToDraw;
				Vector2 interpolatedPos;
				for (float j = 0; j < 1; j += 0.25f)
				{
					if (i == 0)
					{
						rotationToDraw = Utils.AngleLerp(Projectile.rotation, Projectile.oldRot[0], j);
						interpolatedPos = Vector2.Lerp(Projectile.Center, Projectile.oldPos[0] + Projectile.Size / 2, j);
					}
					else
					{
						interpolatedPos = Vector2.Lerp(Projectile.oldPos[i - 1] + Projectile.Size / 2, Projectile.oldPos[i] + Projectile.Size / 2, j);
						rotationToDraw = Utils.AngleLerp(Projectile.oldRot[i - 1], Projectile.oldRot[i], j);
					}
					Main.EntitySpriteDraw(texture, interpolatedPos - Main.screenPosition + Projectile.Size / 2, null, afterImgColor * (1 - i / (float)Projectile.oldPos.Length), rotationToDraw, texture.Size() / 2, 1, SpriteEffects.None, 0);
				}
			}

			return false;
		}
	}




	



}