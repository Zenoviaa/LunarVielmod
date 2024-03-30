using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.UI.Systems;
using Terraria.Audio;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Bosses.STARBOMBER.Projectiles;

namespace Stellamod.NPCs.Illuria
{
	public class HoloBuster : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Shot");
			Main.projFrames[Projectile.type] = 1;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
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
			Projectile.velocity *= 1.04f;

			Timer++;
			Player playerToHomeTo = Main.player[Main.myPlayer];
			float closestDistance = Vector2.Distance(Projectile.position, playerToHomeTo.position);
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player player = Main.player[i];
				float distanceToPlayer = Vector2.Distance(Projectile.position, player.position);
				if (distanceToPlayer < closestDistance)
				{
					closestDistance = distanceToPlayer;
					playerToHomeTo = player;
				}
			}




			float maxDetectRadius = 1f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 10f; // The speed at which the projectile moves towards the target


			
			if (Timer2 < 20)
			{
				maxDetectRadius = 2000f;
				Projectile.rotation = Projectile.DirectionTo(playerToHomeTo.Center).ToRotation() - MathHelper.PiOver2;
			}



			Player closestplayer = FindClosestNPC(maxDetectRadius);
			// Trying to find NPC closest to the projectile

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


		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			// Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.Lerp(new Color(205, 100, 255), new Color(151, 46, 175), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			return false;
		}

	}
}
