using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusRework
{
    public class VoidBomb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Shot");
			Main.projFrames[Projectile.type] = 30;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 100;
			Projectile.width = 256;
			Projectile.height = 244;
			Projectile.light = 1.5f;
			Projectile.friendly = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.hostile = true;
			Projectile.scale = 1f;
	
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public float Timer2;
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(900, 900, 900, 0) * (1f - Projectile.alpha / 50f);
		}

		public override void OnSpawn(IEntitySource source)
		{
			ParticleManager.NewParticle(Projectile.Center , Projectile.velocity * 0, ParticleManager.NewInstance<VoidSuction>(), Color.Purple, 0.4f, Projectile.whoAmI);
			for (int i = 0; i < 150; i++)
			{
				Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
				var d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, speed * 11, Scale: 3f);
				;
				d.noGravity = true;
			}
		}
		public override void AI()
		{
			Timer2++;
			Projectile.velocity *= 0.98f;


		



			Timer++;
			if (Timer > 250)
			{

				Projectile.scale *= 0.96f;



			}


			float maxDetectRadius = 1f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 4f; // The speed at which the projectile moves towards the target

			

			if (Timer2 < 250)
			{
				maxDetectRadius = 2000f;
				for (int i = 0; i < Main.maxNetPlayers; i++)
				{
					Player npc = Main.player[i];

					if (npc.active)
					{
						float distance = Vector2.Distance(Projectile.Center, npc.Center);
						if (distance <= 250)
						{
							Vector2 direction = npc.Center - Projectile.Center;
							direction.Normalize();
							npc.velocity.X -= direction.X * 0.4f;
							npc.velocity.Y -= direction.Y * 0.4f;
						}
					}
				}

			}

			


			// Trying to find NPC closest to the projectile
			Player closestplayer = FindClosestNPC(maxDetectRadius);
			if (closestplayer == null)
				return;

			// If found, change the velocity of the projectile and turn it in the direction of the target
			// Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
			Projectile.velocity = (closestplayer.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;

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


		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 10)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 30)
				{
					Projectile.frame = 0;
				}
			}
			return true;


		}
	}
}
