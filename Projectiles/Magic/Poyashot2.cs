using Microsoft.Xna.Framework;
using Stellamod.Projectiles.IgniterExplosions;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
	public class Poyashot2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("MeatBall");
			Main.projFrames[Projectile.type] = 1;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 12;
			Projectile.width = 12;
			Projectile.height = 24;
			Projectile.light = 1.5f;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 360;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public float Timer2;
		bool Moved;
		public override void AI()
		{
			Timer2++;



			Timer++;


			if (Timer == 3)
			{
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
					ModContent.ProjectileType<AlcaricMushBoom>(), Projectile.damage * 1, 0f, Projectile.owner);
				Timer = 0;


			}


			float maxDetectRadius = 3f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 15f; // The speed at which the projectile moves towards the target

			if (Timer2 == 0)
			{
				maxDetectRadius = 0f;

			}

			if (Timer2 < 60)
			{

				if (Projectile.alpha >= 10)
				{
					Projectile.alpha -= 10;
				}

				Projectile.ai[1]++;
				if (!Moved && Projectile.ai[1] >= 0)
				{
					Moved = true;
				}
				if (Projectile.ai[1] == 1)
				{
					Projectile.alpha = 255;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.position.X = Main.rand.NextFloat(Projectile.position.X - 120, Projectile.position.X + 120);
                        Projectile.position.Y = Main.rand.NextFloat(Projectile.position.Y - 120, Projectile.position.Y + 120);
                        Projectile.netUpdate = true;
                    }
                }
				if (Projectile.ai[1] == 2)
				{
					Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
					Projectile.velocity = -Projectile.velocity;
				}
				if (Projectile.ai[1] >= 0 && Projectile.ai[1] <= 20)
				{
					Projectile.velocity *= .86f;

				}
				if (Projectile.ai[1] == 60)
				{
					Projectile.tileCollide = true;
				}
				if (Projectile.ai[1] == 20)
				{
					Projectile.velocity = -Projectile.velocity;
				}
				if (Projectile.ai[1] >= 21 && Projectile.ai[1] <= 60)
				{
					Projectile.velocity /= .90f;

				}
















			}

			if (Timer2 == 1)
			{

				for (int j = 0; j < 10; j++)
				{
					Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
					vector2 += -Utils.RotatedBy(Vector2.UnitY, (j * 3.141591734f / 6f), default(Vector2)) * new Vector2(8f, 16f);
					vector2 = Utils.RotatedBy(vector2, (Projectile.rotation - 1.57079637f), default(Vector2));
					int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
					Main.dust[num8].scale = 1.3f;
					Main.dust[num8].noGravity = true;
					Main.dust[num8].position = Projectile.Center + vector2;
					Main.dust[num8].velocity = Projectile.velocity * 0.1f;
					Main.dust[num8].noLight = true;
					Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
				}

			}

			if (Timer2 < 60)
			{
				maxDetectRadius = 2000f;

			}



			// Trying to find NPC closest to the projectile
			NPC closestNPC = FindClosestNPC(maxDetectRadius);
			if (closestNPC == null)
				return;

			// If found, change the velocity of the projectile and turn it in the direction of the target
			// Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
			Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
			Projectile.rotation = Projectile.velocity.ToRotation();
		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);

		}
		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null
		public NPC FindClosestNPC(float maxDetectDistance)
		{
			NPC closestNPC = null;

			// Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			// Loop through all NPCs(max always 200)
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				NPC target = Main.npc[k];
				// Check if NPC able to be targeted. It means that NPC is
				// 1. active (alive)
				// 2. chaseable (e.g. not a cultist archer)
				// 3. max life bigger than 5 (e.g. not a critter)
				// 4. can take damage (e.g. moonlord core after all it's parts are downed)
				// 5. hostile (!friendly)
				// 6. not immortal (e.g. not a target dummy)
				if (target.CanBeChasedBy())
				{
					// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					// Check if it is within the radius
					if (sqrDistanceToTarget < sqrMaxDetectDistance)
					{
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			Projectile.rotation += 0.1f;
			{


				Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
				Projectile.rotation = Projectile.velocity.ToRotation();
				if (Projectile.velocity.Y > 25f)
				{
					Projectile.velocity.Y = 25f;
				}
			}
			return closestNPC;
		}



	}
}
