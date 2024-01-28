using Microsoft.Xna.Framework;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Particles;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Utilis;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;


namespace Stellamod.NPCs.Bosses.STARBOMBER.Projectiles
{
	public class STRIKEBULLET : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Shot");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			Main.projFrames[Projectile.type] = 1;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 100;
			Projectile.width = 80;
			Projectile.height = 80;
			Projectile.light = 1.5f;
			Projectile.friendly = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = true;
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
			Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
			Timer++;
			if (Timer == 3)
			{
				if(Main.myPlayer == Projectile.owner)
				{
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0,
						ModContent.ProjectileType<STARSHOTT>(), Projectile.damage * 0, 0f, Projectile.owner, 0f, 0f);
                    p.rotation = direction.ToRotation();
					p.netUpdate = true;
                }
	
				Timer = 0;
			}


			float maxDetectRadius = 4f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 28f; // The speed at which the projectile moves towards the target

			if (Timer2 == 6)
			{
				maxDetectRadius = 0f;
			}
			
			Player closestplayer = FindClosestNPC(maxDetectRadius);
			if (closestplayer == null)
				return;

			if (Timer2 == 1)
			{
			
				Projectile.rotation = Projectile.DirectionTo(closestplayer.Center).ToRotation() - MathHelper.PiOver2;
			}

			if (Timer2 < 30)
            {
				for (int j = 0; j < 10; j++)
				{
					Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
					vector2 += -Vector2.UnitY.RotatedBy(j * 3.141591734f / 6f, default) * new Vector2(8f, 16f);
					vector2 = vector2.RotatedBy(Projectile.rotation - 1.57079637f, default);
					int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.BoneTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
					Main.dust[num8].scale = 1.3f;
					Main.dust[num8].noGravity = true;
					Main.dust[num8].position = Projectile.Center + vector2;
					Main.dust[num8].velocity = Projectile.velocity * 0.1f;
					Main.dust[num8].noLight = true;
					Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
				}
			}

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

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Kill();
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STAREXPULSION"));
			for (int j = 0; j < 10; j++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
				ParticleManager.NewParticle(Projectile.Center, speed * 3, ParticleManager.NewInstance<BurnParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
			}

			for (int i = 0; i < 7; i++)
			{
				ParticleManager.NewParticle(Projectile.Center, (Vector2.One * Main.rand.Next(1, 10)).RotatedByRandom(10.0), ParticleManager.NewInstance<ShadeParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
			}

			return false;
		}
	}
}
