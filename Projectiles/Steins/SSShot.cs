using Microsoft.Xna.Framework;
using Stellamod.Projectiles.IgniterExplosions;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;

using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

using static Terraria.ModLoader.ModContent;
using Stellamod.Projectiles.IgniterExplosions.Stein;
using Stellamod.Dusts;

namespace Stellamod.Projectiles.Steins
{
	public class SSShot : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("MeatBall");
			Main.projFrames[Projectile.type] = 1;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
			//The recording mode
		}
		private float alphaCounter = 0;
		public override void SetDefaults()
		{
			Projectile.damage = 12;
			Projectile.width = 5;
			Projectile.height = 5;
			Projectile.light = 1.5f;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 360;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public float Timer2;
		bool Moved;

		public override void OnKill(int timeLeft)
		{
			float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
			float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedXa * 0, speedYa * 0, ModContent.ProjectileType<SpidingShot>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
			SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Starexplosion"), Projectile.position);


			for (int i = 0; i < 12; i++)
			{
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Sparkle>(),
					(Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.White, 1f).noGravity = false;
			}

			for (int i = 0; i < Main.rand.Next(2, 4); i++)
			{
				Vector2 velocity = -Projectile.velocity;
				velocity = velocity.RotatedByRandom(MathHelper.PiOver4 + MathHelper.PiOver4 / 2);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, velocity,
							 ProjectileID.BabySpider, Projectile.damage, Projectile.knockBack, Projectile.owner);
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
		public PrimDrawer TrailDrawer { get; private set; } = null;
		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * Projectile.width * 1.3f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}
		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.ForestGreen, Color.Transparent, completionRatio) * 0.7f;
		}
		public override bool PreDraw(ref Color lightColor)
		{
			if (Main.rand.NextBool(5))
			{
				int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, 0f, 0f, 150, Color.White, 1f);
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
			TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
			TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
			Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(55f * alphaCounter), (int)(5f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(55f * alphaCounter), (int)(5f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(55f * alphaCounter), (int)(5f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
			Lighting.AddLight(Projectile.Center, Color.GreenYellow.ToVector3() * 1.0f * Main.essScale);
		}


		public override void AI()
		{
			Timer2++;



			Timer++;

			if (alphaCounter <= 2)
			{
				alphaCounter += 0.01f;
			}



			float maxDetectRadius = 5f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 30f; // The speed at which the projectile moves towards the target





			if (Timer > 100)
			{
				maxDetectRadius = 3000f;
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
