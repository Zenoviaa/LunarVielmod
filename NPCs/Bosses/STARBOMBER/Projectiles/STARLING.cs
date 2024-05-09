using Microsoft.Xna.Framework;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.NPCs.Bosses.STARBOMBER.Projectiles
{
    public class STARLING : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 28;
		}

		public enum ActionState
		{
			Speed,
			Wait
		}

		// Current state
		public int frameTick;
		// Current state's timer
		public float timer;
		public int PrevAtack;

		// AI counter
		public int counter;

		public ActionState State = ActionState.Wait;
		public override void SetDefaults()
		{
			NPC.width = 46;
			NPC.height = 50;
			NPC.damage = 70;
			NPC.defense = 30;
			NPC.lifeMax = 1000;
			NPC.HitSound = SoundID.NPCHit32;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 0f;
			NPC.knockBackResist = .45f;
			NPC.aiStyle = 85;
			AIType = NPCID.StardustCellBig;
			NPC.noTileCollide = true;
			NPC.scale = 0.5f;
			NPC.noGravity = true;
		}
		
		int invisibilityTimer;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BoneTorch, 1, -1f, 1, default, .61f);
			}
		}
		

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.45f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}
		public float Shooting = 0f;
		public override void AI()
		{

			timer++;
			if(timer == 1 && StellaMultiplayer.IsHost)
			{
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
					ModContent.ProjectileType<SPARKLYSTARBOOM>(), 0, 0, Main.myPlayer);
			}

			NPC.spriteDirection = NPC.direction;
			Shooting++;

			invisibilityTimer++;
			if (invisibilityTimer >= 100)
			{
				Speed();
				for (int k = 0; k < 11; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BoneTorch, NPC.direction, -1f, 1, default, .61f);

				invisibilityTimer = 0;
			}

			if (Shooting == 80)
			{
				if (StellaMultiplayer.IsHost)
                {
                    float speedYb = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                    float speedXBb = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 1, speedYb - 2 * 1,
						ProjectileID.BombSkeletronPrime, 30, 0f, Owner: Main.myPlayer);
                }
	
				for (int k = 0; k < 5; k++)
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BoneTorch, NPC.direction, -1f, 1, default, .61f);
				}
				Shooting = 0;
			}

			switch (State)
			{

				case ActionState.Wait:
					counter++;
					Wait();
					break;

				case ActionState.Speed:
					counter++;
					Speed();
					NPC.velocity *= 0.98f;
					break;


				default:
					counter++;
					break;
			}
		}


	

		public void Wait()
		{
			timer++;
			if (timer > 50)
			{

			



			}
			else if (timer == 60)
			{
				State = ActionState.Speed;
				timer = 0;
			}
		}
	

		public void Speed()
		{
			timer++;
			if (timer == 100)
			{
				State = ActionState.Wait;
				timer = 0;
			}
		}

        public override bool? CanFallThroughPlatforms()
        {
			return true;
        }
    }
}