using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.NPCs.Bosses.STARBOMBER.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Particles;
using Terraria.DataStructures;
using Stellamod.Trails;
using Stellamod.Utilis;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Stellamod.UI.Systems;

namespace Stellamod.NPCs.Bosses.Fenix
{
	public class FoxRusher : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Shot");
			Main.projFrames[Projectile.type] = 60;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 100;
			Projectile.width = 58 / 2;
			Projectile.height = 88 / 2;
			Projectile.light = 1.5f;
			Projectile.friendly = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
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
			Projectile.velocity *= 1.03f;
			Projectile.velocity.Y *= 1.3f;
			Timer++;
			if (Timer == 75)
			{



				var entitySource = Projectile.GetSource_FromThis();
				NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
				NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y + 400, ModContent.NPCType<STARBOMBERLASERWARN>());
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));

				Timer = 0;


			}

			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 60)
				{
					Projectile.frame = 0;
				}
			}

			float maxDetectRadius = 1f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 9f; // The speed at which the projectile moves towards the target



			if (Timer2 < 150)
			{
				maxDetectRadius = 2000f;

			}

			if (Timer2 == 150)
			{
				Projectile.Kill();
				for (int i = 0; i < 150; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
					var d = Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, speed * 5, Scale: 3f);
					;
					d.noGravity = true;

					Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
					var da = Dust.NewDustPerfect(Projectile.Center, DustID.CoralTorch, speeda * 5, Scale: 3f);
					;
					da.noGravity = false;

					Vector2 speedab = Main.rand.NextVector2CircularEdge(5f, 5f);
					var dab = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, speeda * 7, Scale: 3f);
					;
					dab.noGravity = false;
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

		public PrimDrawer TrailDrawer { get; private set; } = null;
		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * (58 / 2) * 1.3f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}
		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.LightPink, Color.Transparent, completionRatio) * 0.7f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 center = Projectile.Center + new Vector2(0f, Projectile.height * -0.1f);

			// This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
			Vector2 direction = Main.rand.NextVector2CircularEdge(Projectile.width * 0.6f, Projectile.height * 0.6f);
			float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
			Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)





			Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
			Vector2 frameOrigin = frame.Size() / 2;
			Vector2 offset = new Vector2(Projectile.width - frameOrigin.X);
			Vector2 drawPos = Projectile.position - Main.screenPosition + frameOrigin + offset;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

			time %= 4f;
			time /= 2f;

			if (time >= 1f)
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;

			for (float i = 0f; i < 1f; i += 0.25f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(220, 70, 255, 80), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(96, 190, 70, 77), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
			}



		
			TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
			TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
			
			return true;
		}

		
		

	}
}
