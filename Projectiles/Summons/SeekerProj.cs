using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Summons.Glyph;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons
{
    public class SeekerProj : ModProjectile
	{
		public bool[] hitByThisStardustExplosion = new bool[200];// { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("IgniterStart");
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 50;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
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
				if (Projectile.velocity.Y > 16f)
				{
					Projectile.velocity.Y = 16f;
				}
			}
			return closestNPC;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			float speedXa = (Projectile.velocity.X / 6) + Main.rand.NextFloat(-10f, 10f);
			float speedYa = (Projectile.velocity.Y / 6) + Main.rand.Next(-10, 10);

			NPC npc = target;

			switch (Main.rand.Next(7))
			{


				case 0:

					
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X , Projectile.position.Y , speedXa * 1.1f, speedYa * -0.3f, ModContent.ProjectileType<Glyph1>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);

					for (int i = 0; i < 130; i++)
					{
						Vector2 speed = Main.rand.NextVector2CircularEdge(1.2f, 1.2f);
						var d = Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, speed * 8, Scale: 1f);
						;
						d.noGravity = true;
					}
					ShakeModSystem.Shake = 3;
					SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Projectile.position);
					Projectile.Kill();
					


					break;
				case 1:
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X , Projectile.position.Y , speedXa * 0f, speedYa * 1f, ModContent.ProjectileType<Glyph6>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);

					for (int i = 0; i < 130; i++)
					{
						Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
						var d = Dust.NewDustPerfect(Projectile.Center, DustID.SilverCoin, speed * 8, Scale: 1f);
						;
						d.noGravity = true;
					}

				

					for (int i = 0; i < 30; i++)
					{
						float speedXb = (Projectile.velocity.X / 6) + Main.rand.NextFloat(-2f, 2f);
						float speedYb = (Projectile.velocity.Y / 6) + Main.rand.Next(-3, 2);


						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y , speedXb, speedYb, ModContent.ProjectileType<Coinspa>(), Projectile.damage / 2, 0f, Projectile.owner, 0f, 0f);
					}
					ShakeModSystem.Shake = 3;
					SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.position);
					Projectile.Kill();
					

					break;
				case 2:
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X , Projectile.position.Y , speedXa * 0.5f, speedYa * 0.2f, ModContent.ProjectileType<Glyph7>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);

					for (int i = 0; i < 130; i++)
					{
						Vector2 speed = Main.rand.NextVector2CircularEdge(1.5f, 1.5f);
						var d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, speed * 8, Scale: 1f);
						;
						d.noGravity = true;
					}
					ShakeModSystem.Shake = 3;
					Projectile.Kill();
					SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse,  Projectile.position);


					break;
				case 3:
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X , Projectile.position.Y , speedXa * 1f, speedYa * 1f, ModContent.ProjectileType<Glyph4>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);

					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X , Projectile.position.Y , speedXa * 2f, speedYa * 0.2f, ProjectileID.BallofFire, Projectile.damage, 0f, Projectile.owner, 0f, 0f);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X , Projectile.position.Y , speedXa * -2f, speedYa * 0.2f, ProjectileID.BallofFire, Projectile.damage, 0f, Projectile.owner, 0f, 0f);

					for (int i = 0; i < 130; i++)
					{
						Vector2 speed = Main.rand.NextVector2CircularEdge(1.2f, 1.2f);
						var d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed * 8, Scale: 1f);
						;
						d.noGravity = true;
					}
					ShakeModSystem.Shake = 3;
					SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
					Projectile.Kill();
			


					break;
				case 4:

					
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X , Projectile.position.Y , speedXa * -0.3f, speedYa * 0.7f, ModContent.ProjectileType<Glyph3>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);

					for (int i = 0; i < 130; i++)
					{
						Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
						var d = Dust.NewDustPerfect(Projectile.Center, DustID.CoralTorch, speed * 8, Scale: 1f);
						;
						d.noGravity = true;
					}

					for (int i = 0; i < Main.maxNPCs; i++)
					{
						NPC npc2 = Main.npc[i];
						if (!npc2.dontTakeDamage 
							&& npc2.chaseable
							&& !npc2.townNPC
							&& NPCID.Sets.ActsLikeTownNPC[npc2.type] == false 
							&& !hitByThisStardustExplosion[npc2.whoAmI])
						{
							hitByThisStardustExplosion[npc2.whoAmI] = true;
							NPC.HitInfo hitInfo = new();
							hitInfo.Damage = Projectile.damage / 2;
					
							hitInfo.DamageType = DamageClass.Summon;
							npc2.StrikeNPC(hitInfo);
						}
					}

					SoundEngine.PlaySound(SoundID.DD2_DrakinShot, Projectile.position);
					ShakeModSystem.Shake = 3;

					Projectile.Kill();
					


					break;
				case 5:
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y , speedXa * 1f, speedYa * 2f, ModContent.ProjectileType<Glyph5>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);

					for (int i = 0; i < 130; i++)
					{
						Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
						var d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, speed * 8, Scale: 1f);
						;
						d.noGravity = true;
					}

					npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
					ShakeModSystem.Shake = 3;
					Projectile.Kill();
					

					break;
				case 6:
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y , speedXa * 0.3f, speedYa * -.5f, ModContent.ProjectileType<Glyph2>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);

					for (int i = 0; i < 130; i++)
					{
						Vector2 speed = Main.rand.NextVector2CircularEdge(1.8f, 1.8f);
						var d = Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, speed * 8, Scale: 1f);
						;
						d.noGravity = true;
					}
					ShakeModSystem.Shake = 3;


					target.AddBuff(BuffID.OnFire, 100);
					target.AddBuff(BuffID.Confused, 100);
					target.AddBuff(BuffID.Frostburn2, 100);
					SoundEngine.PlaySound(SoundID.Shatter, Projectile.position);
					Projectile.Kill();
				

					break;
			}
			



					base.OnHitNPC(target, hit, damageDone);
			
				
			
		}
	}
}
		
	
