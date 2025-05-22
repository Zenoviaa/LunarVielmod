using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusRework
{
    public class LanternOfHope : ModNPC
	{


		public int moveSpeed = 0;
		public int moveSpeedY = 0;
		public int counter;
		public ref float AI_State => ref NPC.ai[0];
	
		public float AiTimer
		{
			get => NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		public ref float AI_FlutterTime => ref NPC.ai[2];

		public int frame = 0;
		public int timer = 0;
		public int timer2 = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Viola");
			Main.npcFrameCount[NPC.type] = 1; // make sure to set this for your modNPCs.

			// Specify the debuffs it is immune to
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 78; // The width of the NPC's hitbox (in pixels)
			NPC.height = 110; // The height of the NPC's hitbox (in pixels)
			NPC.aiStyle = -1; // This NPC has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.// The amount of damage that this NPC deals // The amount of defense that this NPC has // The amount of health that this NPC has
			NPC.value = 0f; // How many copper coins the NPC will drop when killed.
			NPC.friendly = false;
			NPC.lifeMax = 200;
			NPC.noGravity = true;
			NPC.knockBackResist = 0f;
			NPC.damage = 1; // The amount of damage that this NPC deals
			NPC.defense = 5; // The amount of defense that this NPC has
		

		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.Player;
			//if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust && !Main.pumpkinMoon && !Main.snowMoon))
			//{
		//		return spawnInfo.Player.ZoneFable() ? 1.6f : 0f;
		//	}


			return 0f;
		}
		// Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
		public override void AI()
		{
			NPC.damage = 0;
		


			float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
			float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
			Player player = Main.player[NPC.target];

			timer2++;

			if (timer2 <= 300)
			{
				NPC.aiStyle = 22;
				AIType = NPCID.Pixie;

				
			}

			if (timer2 == 310)
            {
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 10, NPC.position.Y + speedY - 260, speedX * 0, speedY - 2 * 2, ModContent.ProjectileType<LanturnSpear>(), 12, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 10, NPC.position.Y + speedY - 260, speedX * 0, speedY + 2 * 2, ModContent.ProjectileType<LanturnSpear>(), 12, 0f, 0, 0f, 0f);
				

			}

			if (timer2 == 330)
			{
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 10, NPC.position.Y + speedY - 260, speedX * 0, speedY - 2 * 2, ModContent.ProjectileType<LanturnSpear>(), 12, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 10, NPC.position.Y + speedY - 260, speedX * 0, speedY + 2 * 2, ModContent.ProjectileType<LanturnSpear>(), 12, 0f, 0, 0f, 0f);
			

			}

			if (timer2 == 350)
			{
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 10, NPC.position.Y + speedY - 260, speedX * 0, speedY - 2 * 2, ModContent.ProjectileType<LanturnSpear>(), 12, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 10, NPC.position.Y + speedY - 260, speedX * 0, speedY + 2 * 2, ModContent.ProjectileType<LanturnSpear>(), 12, 0f, 0, 0f, 0f);
				timer2 = 0;

			}
			timer++;
		




			Vector3 RGB = new(2.55f, 0.45f, 0.94f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);




		}
		
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);

			// This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
			Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
			float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
			Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)






			Vector2 frameOrigin = NPC.frame.Size();
			Vector2 offset = new Vector2(NPC.width - frameOrigin.X, NPC.height - NPC.frame.Height);
			Vector2 drawPos = NPC.position - screenPos + frameOrigin + offset;

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

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, NPC.frame, new Color(220, 173, 255, 100), NPC.rotation, frameOrigin, NPC.scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, NPC.frame, new Color(140, 66, 255, 100), NPC.rotation, frameOrigin, NPC.scale, SpriteEffects.None, 0);
			}

			// Using a rectangle to crop a texture can be imagined like this:
			// Every rectangle has an X, a Y, a Width, and a Height
			// Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
			// Our Width and Height values specify how big of an area we want to sample starting from X and Y
			
			
			return false;
        }
        
		
	}
}
