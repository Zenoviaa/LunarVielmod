using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
    public class FrostShot2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Shot");
			Main.projFrames[Projectile.type] = 1;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 100;
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.light = 1.5f;
			Projectile.friendly = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 1000;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.hostile = true;
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public float Timer2;

		public override void AI()
		{
			Timer2++;


			Timer++;
			if (Timer == 3)
			{
				if(Main.myPlayer == Projectile.owner)
				{
                    float speedXabc = -Projectile.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYabc = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.00f + Main.rand.Next(0, 0) * 0.0f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabc - 20, Projectile.position.Y + speedYabc - 20, speedXabc * 0, speedYabc * 0,
                        ModContent.ProjectileType<FrostShotIN>(), Projectile.damage * 0, 0f, Projectile.owner, 0f, 0f);
                }

				Timer = 0;


			}


			float maxDetectRadius = 3f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 15f; // The speed at which the projectile moves towards the target

			

			if (Timer2 == 1)
			{
				maxDetectRadius = 2000f;

			}

			if (Timer2 == 6)
			{
				maxDetectRadius = 0f;
			}


			// Trying to find NPC closest to the projectile
			Player closestplayer = FindClosestNPC(maxDetectRadius);
			if (closestplayer == null)
				return;

			// If found, change the velocity of the projectile and turn it in the direction of the target
			// Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
			Projectile.velocity = (closestplayer.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;

		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{

			behindNPCs.Add(index);

		}
		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null
		public Player FindClosestNPC(float maxDetectDistance)
		{
			Player closestplayer = null;

			// Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			// Loop through all NPCs(max always 200)
			for (int k = 0; k < Main.maxPlayers; k++)
			{
				Player target = Main.player[k];
				// Check if NPC able to be targeted. It means that NPC is
				// 1. active (alive)
				// 2. chaseable (e.g. not a cultist archer)
				// 3. max life bigger than 5 (e.g. not a critter)
				// 4. can take damage (e.g. moonlord core after all it's parts are downed)
				// 5. hostile (!friendly)
				// 6. not immortal (e.g. not a target dummy)

				// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
				float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

				// Check if it is within the radius
				if (sqrDistanceToTarget < sqrMaxDetectDistance)
				{
					sqrMaxDetectDistance = sqrDistanceToTarget;
					closestplayer = target;
				}

			}


			return closestplayer;
		}
		
		


	}
}
