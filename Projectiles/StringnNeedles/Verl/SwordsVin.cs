
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.StringnNeedles.Verl
{
    // - ModProjectile - the minion itself

    // It is not recommended to put all these classes in the same file. For demonstrations sake they are all compacted together so you get a better overwiew.
    // To get a better understanding of how everything works together, and how to code minion AI, read the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
    // This is NOT an in-depth guide to advanced minion AI

    // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    // This minion shows a few mandatory things that make it behave properly.
    // Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
    // If the player targets a certain NPC with right-click, it will fly through tiles to it
    // If it isn't attacking, it will float near the player with minimal movement
    public class SwordsVin : ModProjectile
	{
		NPC target;
		int afterImgCancelDrawCount = 0;
		//int afterImgCancelDrawCount2 = 0;
		Vector2 endPoint;
		Vector2 controlPoint1;
		Vector2 controlPoint2;
		Vector2 initialPos;
		Vector2 wantedEndPoint;
		bool initialization = false;
		float AoERadiusSquared = 36000;//it's squared for less expensive calculations
		public bool[] hitByThisStardustExplosion = new bool[200] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };
        float t = 0;
		public bool Hitbebeans;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spragald");
			// Sets the amount of frames this minion has on its spritesheet

			// This is necessary for right-click targeting
		
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			

		 // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = false; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public int Timer = 0;

		public override void SetDefaults()
		{
			Projectile.originalDamage = (int)5f;
			Projectile.width = 28;
			Projectile.height = 28;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely
			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
		 // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 0f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.timeLeft = 2000;
			Projectile.scale = 0.8f;


		}
		// Here you can decide if your minion breaks things like grass or pots
		
		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
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
				if (Projectile.localAI[0] >= 60 * num996)
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
		public static Vector2 CubicBezier(Vector2 start, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 end, float t)
		{
			float tSquared = t * t;
			float tCubed = t * t * t;
			return
				-(start * (-tCubed + (3 * tSquared) - (3 * t) - 1) +
				controlPoint1 * ((3 * tCubed) - (6 * tSquared) + (3 * t)) +
				controlPoint2 * ((-3 * tCubed) + (3 * tSquared)) +
				end * tCubed);
		}

		public override void AI()
		{








			if (!initialization)
			{
				initialPos = Projectile.Center;
				endPoint = Projectile.Center;
			}
			float distanceSQ = float.MaxValue;
			if (target == null || !target.active)
				for (int i = 0; i < Main.npc.Length; i++)
				{
					if ((target == null || Main.npc[i].DistanceSQ(Projectile.Center) < distanceSQ) && Main.npc[i].active && !Main.npc[i].friendly && !Main.npc[i].dontTakeDamage && Main.npc[i].type != NPCID.CultistBossClone)
					{
						target = Main.npc[i];
						distanceSQ = Projectile.Center.DistanceSQ(target.Center);
					}
				}
			if (target != null && target.DistanceSQ(Projectile.Center) < 10000000 && target.active && !hitByThisStardustExplosion[target.whoAmI])
			{
				wantedEndPoint = initialPos - (target.Center - initialPos);
				if (Projectile.ai[0] < 10)
				{
					endPoint = wantedEndPoint;
				}
			}
			if (!initialization)
			{
				controlPoint1 = Projectile.Center + Main.rand.NextVector2CircularEdge(1000, 1000);
				controlPoint2 = endPoint + Main.rand.NextVector2CircularEdge(1000, 1000);
				//controlPoint2 = Vector2.Lerp(endPoint, initialPos, 0.33f) + Main.player[Projectile.owner].velocity * 70;
				//if (target != null)
				//    controlPoint1 = Vector2.Lerp(endPoint, initialPos, 0.66f) + target.velocity * 70;
				//else
				//    Projectile.Kill();
				initialization = true;
			}
			Projectile.velocity = Vector2.Zero;
			Projectile.rotation = (Projectile.Center - CubicBezier(initialPos, controlPoint1, controlPoint2, endPoint, t + 0.025f)).ToRotation() - MathHelper.PiOver2;
			endPoint = endPoint.MoveTowards(wantedEndPoint, 32);
			if (t > 4)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.Center.DistanceSQ(Projectile.Center) < AoERadiusSquared && !npc.dontTakeDamage && !hitByThisStardustExplosion[npc.whoAmI])
					{
						hitByThisStardustExplosion[npc.whoAmI] = true;
						NPC.HitInfo hitInfo = new();
						hitInfo.Damage = Projectile.damage;
						//(int)Main.player[Projectile.owner].GetDamage(DamageClass.Summon).ApplyTo(Projectile.damage)
						hitInfo.DamageType = DamageClass.Melee;
						npc.StrikeNPC(hitInfo);
					}
				}
				afterImgCancelDrawCount++;
			}
			else if (target != null)
			{
				Projectile.Center = CubicBezier(initialPos, controlPoint1, controlPoint2, endPoint, t);
			}
			

			t += 0.01f;

			Projectile.ai[0]++;

		}

		public override bool PreDraw(ref Color lightColor)
		{



			Color afterImgColor = Main.hslToRgb(Projectile.ai[1], 1, 0.5f);
			float opacityForSparkles = 1 - (float)afterImgCancelDrawCount / 30;
			afterImgColor.A = 40;
			afterImgColor.B = 255;
			afterImgColor.G = 215;
			afterImgColor.R = 96;




			Main.instance.LoadProjectile(ProjectileID.RainbowRodBullet);
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
					Main.EntitySpriteDraw(texture, interpolatedPos - Main.screenPosition + Projectile.Size / 2, null, afterImgColor * (1 - i / (float)Projectile.oldPos.Length), rotationToDraw, texture.Size() / 2, 1, SpriteEffects.None, 0);
				}
			}


			return false;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			/*
			Projectile.ai[0] = 1f;
			Projectile.ai[1] = (float)target.whoAmI;

			Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
			Projectile.netUpdate = true;
			Projectile.damage = 0;

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
			*/
            
				
					float speedXa = (Projectile.velocity.X / 6) + Main.rand.NextFloat(-10f, 10f);
					float speedYa = (Projectile.velocity.Y / 6) + Main.rand.Next(-10, 10);


					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa, speedYa, ModContent.ProjectileType<SwordsArmy2>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);

					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0.7f, speedYa * 0.6f, ModContent.ProjectileType<SwordsArmy3>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0.5f, speedYa * 0.3f, ModContent.ProjectileType<SwordsArmy>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 1.3f, speedYa * 0.3f, ModContent.ProjectileType<SwordsArmy3>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 1f, speedYa * 1.4f, ModContent.ProjectileType<SwordsArmy>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
					ShakeModSystem.Shake = 4;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 1.5f, speedYa * 0.6f, ModContent.ProjectileType<SwordsArmy>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
			SoundEngine.PlaySound(SoundID.Item110, Projectile.position);
				

			
		



		/*	if (num32 >= array2.Length)
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

		*/
	}
		
		

	}
}
