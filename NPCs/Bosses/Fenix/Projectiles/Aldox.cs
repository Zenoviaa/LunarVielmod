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

namespace Stellamod.NPCs.Bosses.Fenix.Projectiles
{
	public class Aldox : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Shot");
			Main.projFrames[Projectile.type] = 25;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 100;
			Projectile.width = 256;
			Projectile.height = 256;
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
			if (Timer == 50)
			{
				float rotation = MathHelper.ToRadians(45);
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 25, 0, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, -25, 0, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 25 * 0, -25, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 25 * 0, 25, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Yumiko3"));
			}

			if (Timer == 100)
			{
				if (StellaMultiplayer.IsHost)
				{
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 25, 25, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, -25, 25, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, -25, -25, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 25, -25, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                }

				Timer = 0;

				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Yumiko4"));
			}

			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 25)
				{
					Projectile.frame = 0;
				}
			}

			float maxDetectRadius = 1f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 4f; // The speed at which the projectile moves towards the target



			if (Timer2 < 350)
			{
				maxDetectRadius = 2000f;

			}

			if (Timer2 == 350)
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
					ParticleManager.NewParticle(Projectile.Center, speedab * 7, ParticleManager.NewInstance<VoidParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

				}
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/STARGROP"));


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
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 80f);
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
			
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)

			TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
			TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

			return true;
		}




	}
}
