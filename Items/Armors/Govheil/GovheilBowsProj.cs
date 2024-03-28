using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.Govheil
{
    public class GovheilBowsProj : ModProjectile
	{
		float alphaCounter = 0;
		//public bool[] hitByThisStardustExplosion = new bool[200] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };
		float ta = 0;
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
			Projectile.timeLeft = 200;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
		}

		public override void AI()
		{
			ta++;
			if (ta == 1)
            {
				for (int i = 0; i < 130; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
					var d = Dust.NewDustPerfect(Projectile.Center, DustID.FrostHydra, speed * 8, Scale: 1f);
					;
					d.noGravity = true;
				}
			}

			float maxDetectRadius = 2000f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 8f; // The speed at which the projectile moves towards the target

			// Trying to find NPC closest to the projectile
			NPC closestNPC = FindClosestNPCS(maxDetectRadius);
			if (closestNPC == null)
				return;

			// If found, change the velocity of the projectile and turn it in the direction of the target
			// Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
			Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
			Projectile.tileCollide = false;
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (alphaCounter > 9)
			{
				for (int r = 0; r < 37; r++)
				{
					Vector2 speed2 = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speed2 * 8, ParticleManager.NewInstance<BurnParticle>(), Color.Aqua, Main.rand.NextFloat(0.2f, 0.8f));
				}

				Projectile.Kill();
				ShakeModSystem.Shake = 3;
			}

			alphaCounter += 0.2f;
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
			for (int r = 0; r < 37; r++)
			{
				Vector2 speed2 = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
				ParticleManager.NewParticle(Projectile.Center, speed2 * 8, ParticleManager.NewInstance<BurnParticle>(), Color.Aqua, Main.rand.NextFloat(0.2f, 0.8f));
			}

			ShakeModSystem.Shake = 3;
			SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, target.position);
			Projectile.Kill();
			base.OnHitNPC(target, hit, damageDone);	
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture2D4 = Request<Texture2D>("Stellamod/Trails/DimLight").Value;
			Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(90f * alphaCounter), (int)(90f * alphaCounter), (int)(90f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (alphaCounter + 0.6f), SpriteEffects.None, 0f);

			Texture2D texture2D5 = Request<Texture2D>("Stellamod/Trails/DimLight").Value;
			Main.spriteBatch.Draw(texture2D5, Projectile.Center - Main.screenPosition, null, new Color((int)(90f * alphaCounter), (int)(45f * alphaCounter), (int)(170f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (alphaCounter + 1.6f), SpriteEffects.None, 0f);

			Texture2D texture2D6 = Request<Texture2D>("Stellamod/Trails/DimLight").Value;
			Main.spriteBatch.Draw(texture2D6, Projectile.Center - Main.screenPosition, null, new Color((int)(90f * alphaCounter), (int)(90f * alphaCounter), (int)(90f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (alphaCounter + 0.3f), SpriteEffects.None, 0f);

			Texture2D texture2D7 = Request<Texture2D>("Stellamod/Trails/DimLight").Value;
			Main.spriteBatch.Draw(texture2D7, Projectile.Center - Main.screenPosition, null, new Color((int)(45f * alphaCounter), (int)(103f * alphaCounter), (int)(103f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (alphaCounter + 0.2f), SpriteEffects.None, 0f);
			return false;
		}
	}
}
		
	
