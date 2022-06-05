using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Stellamod.Buffs.Dusteffects;

namespace Stellamod.Projectiles.IgniterEx
{
	public class IgniterStart : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("IgniterStart");
		}
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 30;


		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI()
		{

			























				float maxDetectRadius = 2000f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 22f; // The speed at which the projectile moves towards the target

			// Trying to find NPC closest to the projectile
			NPC closestNPC = FindClosestNPCS(maxDetectRadius);
			if (closestNPC == null)
				return;

			// If found, change the velocity of the projectile and turn it in the direction of the target
			// Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
			Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
			Projectile.tileCollide = false;
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null
		public NPC FindClosestNPCS(float maxDetectDistance)
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
				if (target.CanBeChasedBy() && target.HasBuff<Dusted>())
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
				if (Projectile.velocity.Y > 16f)
				{
					Projectile.velocity.Y = 16f;
				}
			}
			return closestNPC;
		}
		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{

			NPC npc = target;
			if (npc.active && npc.HasBuff<ArcaneDust>())
			{
				ShakeModSystem.Shake = 8;

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ArcaneExplode"));
				for (int j = 0; j < 20; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(Projectile.Center, speed2 * 10, ParticleManager.NewInstance<BurnParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					ParticleManager.NewParticle(Projectile.Center, speed * 7, ParticleManager.NewInstance<BurnParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					npc.StrikeNPC(damage * 2, 1, 1, false, false, true);
					npc.RequestBuffRemoval(ModContent.BuffType<FlameDust>());
				}
			}



			if (npc.active && npc.HasBuff<FlameDust>())
			{			

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/HeatExplosion"));
				for (int j = 0; j < 20; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);				
					ParticleManager.NewParticle(Projectile.Center, speed * 4, ParticleManager.NewInstance<FlameParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));					
					target.AddBuff(BuffID.OnFire, 720);
					target.AddBuff(ModContent.BuffType<EXPtime>(), 1000);

					Projectile.timeLeft = 250;
					Timer++;
					if (Timer == 150)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Burnbefore"));

					}


					if (Timer == 170)
                    {
						Vector2 velocity = npc.velocity;
						if (npc.active && npc.HasBuff<EXPtime>())
                        {
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"));
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity * 0, ProjectileID.DaybreakExplosion, damage, knockback);
							Projectile.scale = 1.5f;
							ShakeModSystem.Shake = 10;
							npc.StrikeNPC(damage * 15, 1, 1, false, false, true);
							npc.StrikeNPC(damage * 15, 1, 1, false, false, true);
							float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
							float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;					
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<KaBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);



						}

					}
					if (Timer == 200)
					{
						Projectile.Kill();						
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}
				}







				base.OnHitNPC(target, damage, knockback, crit);
			}
		}
	}
}