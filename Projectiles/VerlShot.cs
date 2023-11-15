using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles
{
    public class VerlShot : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Auroran Bullet");
			Main.projFrames[Projectile.type] = 1;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			//The recording mode
		}

		float alphaCounter = 0;
		int afterImgCancelDrawCount = 0;
		float t = 0;
		public override void SetDefaults()
		{

			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.light = 1.5f;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 360;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 4;
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public float Timer2;
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Timer++;
			player.RotatedRelativePoint(Projectile.Center);

			Projectile.rotation -= 0.4f;
			Projectile.velocity *= 0.97f;

			alphaCounter += 0.08f; 
			Timer++;	
			if (t > 3)
			{
				afterImgCancelDrawCount++;
			}

			t += 0.01f;
			if (alphaCounter > 5)
			{
				for (int r = 0; r < 37; r++)
				{
					Vector2 speed2 = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speed2 * 8, ParticleManager.NewInstance<BurnParticle>(), Color.Aqua, Main.rand.NextFloat(0.2f, 0.8f));
				}

				for (int i = 0; i < 130; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
					var d = Dust.NewDustPerfect(Projectile.Center, DustID.FrostHydra, speed * 8, Scale: 1f);
					d.noGravity = true;
				}

				Projectile.Kill();
				ShakeModSystem.Shake = 3;
			}

			alphaCounter += 0.04f;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Color afterImgColor = Main.hslToRgb(Projectile.ai[1], 1, 0.5f);
			//float opacityForSparkles = 1 - (float)afterImgCancelDrawCount / 30;
			afterImgColor.A = 100;
			afterImgColor.B = 125;
			afterImgColor.G = 100;
			afterImgColor.R = 48;
			Main.instance.LoadProjectile(ProjectileID.SparkleGuitar);
			Texture2D texture = TextureAssets.Projectile[ProjectileID.SparkleGuitar].Value;
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

			Texture2D texture2D4 = Request<Texture2D>("Stellamod/Trails/DimLight").Value;
			Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(90f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (alphaCounter + 0.6f), SpriteEffects.None, 0f);

			Texture2D texture2D5 = Request<Texture2D>("Stellamod/Trails/DimLight").Value;
			Main.spriteBatch.Draw(texture2D5, Projectile.Center - Main.screenPosition, null, new Color((int)(90f * alphaCounter), (int)(45f * alphaCounter), (int)(170f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (alphaCounter + 1.6f), SpriteEffects.None, 0f);

			Texture2D texture2D6 = Request<Texture2D>("Stellamod/Trails/DimLight").Value;
			Main.spriteBatch.Draw(texture2D6, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(45f * alphaCounter), (int)(90f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (alphaCounter + 0.3f), SpriteEffects.None, 0f);

			Texture2D texture2D7 = Request<Texture2D>("Stellamod/Trails/DimLight").Value;
			Main.spriteBatch.Draw(texture2D7, Projectile.Center - Main.screenPosition, null, new Color((int)(45f * alphaCounter), (int)(103f * alphaCounter), (int)(103f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (alphaCounter + 0.2f), SpriteEffects.None, 0f);
			return false;
		}
	}
}
