using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using Terraria.GameContent;
using Stellamod.UI.systems;
using Terraria.Audio;


namespace Stellamod.Projectiles
{
	
	public class PlantDaggerProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plant Dagger");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}


		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 30;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 11;
		}

		float alphaCounter;
		public override bool PreAI()
		{
			alphaCounter += 0.08f;
			if (Projectile.ai[0] == 0)
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			else
			{
				Projectile.ignoreWater = true;
				Projectile.tileCollide = false;
				int num996 = 15;
				bool flag52 = false;
				bool flag53 = false;
				Projectile.localAI[0] += 1f;
				if (Projectile.localAI[0] % 30f == 0f)
					flag53 = true;

				int num997 = (int)Projectile.ai[1];
				if (Projectile.localAI[0] >= (float)(60 * num996))
					flag52 = true;
				else if (num997 < 0 || num997 >= 200)
					flag52 = true;
				else if (Main.npc[num997].active && !Main.npc[num997].dontTakeDamage)
				{
					Projectile.Center = Main.npc[num997].Center - Projectile.velocity * 2f;
					Projectile.gfxOffY = Main.npc[num997].gfxOffY;
					if (flag53)
					{
						Main.npc[num997].HitEffect(0, 1.0);
					}
				}
				else
					flag52 = true;

				if (flag52)
					Projectile.Kill();
			}
			return true;
		}

		public void AdditiveCall(SpriteBatch spriteBatch)
		{
			float sineAdd = (float)Math.Sin(alphaCounter) + 2f;
			{
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Color color = new Color(191, 102, 255) * 0.85f * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

					float scale = Projectile.scale;
					Texture2D tex = ModContent.Request<Texture2D>("Stellamod/Projectiles/SacrificialDagger/PlantDagger_Trail").Value;

					spriteBatch.Draw(tex, Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition, null, color * sineAdd, Projectile.rotation, tex.Size() / 2, scale, default, default);
				}
			}
		}
		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Projectile.ai[0] = 1f;
			Projectile.ai[1] = (float)target.whoAmI;
			Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
			Projectile.netUpdate = true;
			Projectile.damage = 0;

			
			float speedX = Projectile.velocity.X * 2;
			float speedY = Projectile.velocity.Y * 2;
			Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0, speedY * 0, ProjectileID.DaybreakExplosion, (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f); //predictor trail, please pick a better dust Yuy
			Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0, speedY * 0, ModContent.ProjectileType<Spragald>(), (int)(Projectile.damage * 2), 0f, Projectile.owner, 0f, 0f); //predictor trail, please pick a better dust Yuy
			Projectile.Kill();



			int num31 = 1;
			Point[] array2 = new Point[num31];
			int num32 = 0;

			for (int n = 0; n < 1000; n++)
			{
				if (n != Projectile.whoAmI && Main.projectile[n].active && Main.projectile[n].owner == Main.myPlayer && Main.projectile[n].type == Projectile.type && Main.projectile[n].ai[0] == 1f && Main.projectile[n].ai[1] == target.whoAmI)
				{
					array2[num32++] = new Point(n, Main.projectile[n].timeLeft);
					if (num32 >= array2.Length)
						break;
				}
			}

			if (num32 >= array2.Length)
			{
				int num33 = 0;
				for (int num34 = 1; num34 < array2.Length; num34++)
				{
					if (array2[num34].Y < array2[num33].Y)
						num33 = num34;
				}
				Main.projectile[array2[num33].X].Kill();
			}
			Player player = Main.player[Projectile.owner];
			int num = -1;
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].CanBeChasedBy(player, false) && Main.npc[i] == target)
				{
					num = i;
				}
			}
			{
				player.MinionAttackTargetNPC = num;
			}
		}

		
       

	}
}