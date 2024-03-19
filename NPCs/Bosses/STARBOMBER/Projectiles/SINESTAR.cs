using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.Projectiles.IgniterExplosions;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.NPCs.Bosses.STARBOMBER.Projectiles
{
    public class SINESTAR : ModProjectile
	{
		float distance = 8;
		int rotationalSpeed = 4;
		int afterImgCancelDrawCount = 0;
		float t = 0;
		public override void SetStaticDefaults()
		{

			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 1000;
			//projectile.extraUpdates = 1;
			Projectile.width = Projectile.height = 64;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.damage = 90;
		}

		bool initialized = false;
		Vector2 initialSpeed = Vector2.Zero;	
		public float Timer = 0;
		public override void AI()
		{
			Timer++;
			if (Timer == 3)
			{
				if(Main.myPlayer == Projectile.owner)
				{
             
                    int fireball = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<AlcaricMushBoom>(), Projectile.damage * 0, 0f, Projectile.owner);
                    Projectile ichor = Main.projectile[fireball];
                    ichor.hostile = true;
                    ichor.friendly = false;
					ichor.netUpdate = true;
                    Timer = 0;
                }
			}

			Projectile.velocity *= 0.991f;
			int rightValue = (int)Projectile.ai[1] - 1;
			if (rightValue < (double)Main.projectile.Length && rightValue != -1)
			{
				Projectile other = Main.projectile[rightValue];
				Vector2 direction9 = other.Center - Projectile.Center;
				direction9.Normalize();
			}

			if (!initialized)
			{
				initialSpeed = Projectile.velocity;
				initialized = true;
			}
			if (initialSpeed.Length() < 20)
				initialSpeed *= 1.01f;
			Projectile.spriteDirection = 1;
			if (Projectile.ai[0] > 0)
			{
				Projectile.spriteDirection = 0;
			}
			
			distance += 0.4f;
			Projectile.ai[0] += rotationalSpeed;

			Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
			offset.Normalize();
			offset *= (float)(Math.Cos(Projectile.ai[0] * (Math.PI / 180)) * (distance / 3));
			Projectile.velocity = initialSpeed + offset;
			Projectile.rotation -= 0.5f;

			if (t > 2)
			{
				
				afterImgCancelDrawCount++;
			}

			t += 0.01f;

			Projectile.ai[0]++;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Color afterImgColor = Main.hslToRgb(Projectile.ai[1], 1, 0.5f);
			afterImgColor.A = 90;
			afterImgColor.B = 223;
			afterImgColor.G = 188;
			afterImgColor.R = 157;
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
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
					Main.EntitySpriteDraw(texture, (interpolatedPos - Main.screenPosition + Projectile.Size / 2) - new Vector2(32, 32) , null, afterImgColor * (1 - i / (float)Projectile.oldPos.Length), rotationToDraw, texture.Size() / 2, 1, SpriteEffects.None, 0);
				}
			}

			return false;
		}
	}
}