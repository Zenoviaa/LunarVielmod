using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class Logger : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Logger");
			Main.projFrames[Projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.aiStyle = 3;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.penetrate = 2;
			Projectile.timeLeft = 600;
			Projectile.extraUpdates = 1;
		}
		public override void AI()
		{
			if (++Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 5)
				{
					Projectile.frame = 0;
				}
			}

			float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 6f; // The speed at which the projectile moves towards the target

			// Trying to find NPC closest to the projectile
			NPC closestNPC = FindClosestNPC(maxDetectRadius);
			if (closestNPC == null)
				return;

			// If found, change the velocity of the projectile and turn it in the direction of the target
			// Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
			Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
			Projectile.rotation = Projectile.velocity.ToRotation();
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
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Firework_Yellow, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.TintableDustLighted, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust2].noGravity = true;
				Main.dust[dust2].velocity *= 0f;
				Main.dust[dust2].velocity *= 0f;
				Main.dust[dust2].scale = 0.3f;
				Main.dust[dust].scale = 0.3f;

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
			if (Main.rand.NextBool(5))
				target.AddBuff(ModContent.BuffType<Wounded>(), 180);
		}
		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
			// SpriteEffects helps to flip texture horizontally and vertically
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.None;

			// Getting texture of projectile
			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

			// Calculating frameHeight and current Y pos dependence of frame
			// If texture without animation frameHeight is always texture.Height and startY is always 0
			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;

			// Get this frame on texture
			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

			// Alternatively, you can skip defining frameHeight and startY and use this:
			// Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

			Vector2 origin = sourceRectangle.Size() / 2f;

			// If image isn't centered or symmetrical you can specify origin of the sprite
			// (0,0) for the upper-left corner
			float offsetX = 20f;
			origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;

			// If sprite is vertical
			// float offsetY = 20f;
			// origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);


			// Applying lighting and draw current frame
			Color drawColor = Projectile.GetAlpha(lightColor);
			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

			// It's important to return false, otherwise we also draw the original texture.
			return false;
		}
		public override bool PreAI()
		{
			Projectile.tileCollide = true;
			Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.TintableDustLighted, 0f, 0f);
			return base.PreAI();
		}
	}
}