using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.Fenix.Projectiles
{
	public class Yumikoo : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Verlia's Moon blade");
			Main.projFrames[Projectile.type] = 1;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 100;
			Projectile.width = 55;
			Projectile.height = 55;
			Projectile.light = 1.5f;
			Projectile.friendly = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.hostile = true;
			Projectile.scale = 2f;

		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public float Timer2;

		public override void AI()
		{

			Timer++;
			Timer2++;
			Projectile.velocity *= 1.05f;

			float speedXabc = -Projectile.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
			float speedYabc = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.00f + Main.rand.Next(0, 0) * 0.0f;


		




			float maxDetectRadius = 1f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 9f; // The speed at which the projectile moves towards the target

			if (Timer < 6)
			{


				projSpeed = 2f;
				maxDetectRadius = 2000f;


			}
			if (Timer > 6 && Timer < 12)
			{


				projSpeed = 9f;
				maxDetectRadius = 2000f;


			}

			if (Timer > 12 && Timer < 18)
			{



				maxDetectRadius = 0f;


			}

			if (Timer > 19 && Timer < 26)
			{



				maxDetectRadius = 2000f;


			}

			if (Timer > 27 && Timer < 34)
			{



				maxDetectRadius = 0f;


			}

			if (Timer > 35 && Timer < 41)
			{



				maxDetectRadius = 2000f;


			}

			if (Timer > 42 && Timer < 49)
			{



				maxDetectRadius = 0f;


			}

			if (Timer > 49)
            {
				Timer = 0;
            }

			// Trying to find NPC closest to the projectile
			Player closestplayer = FindClosestNPC(maxDetectRadius);
			if (closestplayer == null)
				return;



			Projectile.rotation = (Projectile.position - closestplayer.Center).ToRotation() + MathHelper.PiOver2;


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
			float baseWidth = Projectile.scale * (Projectile.width / 4) * 1.3f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}
		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.LightSkyBlue, Color.Transparent, completionRatio) * 0.7f;
		}
		public override bool PreDraw(ref Color lightColor)
		{


			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
			TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);


			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = new Color(255, 255, 255, 255);

			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale / 2, SpriteEffects.None, 0);

			return false;
		}

	}
}
