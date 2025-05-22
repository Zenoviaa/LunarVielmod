using Microsoft.Xna.Framework;
using Stellamod.Assets.Biomes;
using Stellamod.Dusts;
using Stellamod.Items.Armors.Illurian;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Illuria
{
	public class IllurianGuard : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 14;
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

		// AI counter
		public int counter;

		public ActionState State = ActionState.Wait;
		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 56;
			NPC.damage = 30;
			NPC.defense = 50;
			NPC.lifeMax = 1055;
			NPC.HitSound = SoundID.NPCHit32;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 563f;
			NPC.knockBackResist = .45f;
			NPC.aiStyle = 3;
			AIType = NPCID.SnowFlinx;
		}

		

		int invisibilityTimer;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Plantera_Green, 1, -1f, 1, default, .61f);
			}


		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			
			npcLoot.Add(ItemDropRule.OneFromOptions(2,
			ModContent.ItemType<IllurianWarriorChestplate>(),
			ModContent.ItemType<IllurianWarriorGreaves>(),
			ModContent.ItemType<IllurianWarriorHelm>()
			));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IllurineScale>(), minimumDropped: 1, maximumDropped: 2));
		}
	
		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.15f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

        public override bool CheckActive()
        {
			//Returning false here makes them not despawn
			return false;
        }

        public override void AI()
		{
			NPC.TargetClosest(true);

			// Now we check the make sure the target is still valid and within our specified notice range (500)
			
			timer++;
			NPC.spriteDirection = NPC.direction;

			invisibilityTimer++;
			if (invisibilityTimer >= 100)
			{
				Speed();

				for (int k = 0; k < 11; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BlueMoss, NPC.direction, -1f, 1, default, .61f);


				for (int i = 0; i < 8; i++)
				{
					Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.DeepSkyBlue, 0.5f).noGravity = true;
				}
				for (int i = 0; i < 4; i++)
				{
					Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.AliceBlue, 0.5f).noGravity = true;
				}

				if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 150f)
				{
					// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
					
						
					
				}

				invisibilityTimer = 0;
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

				NPC.oldVelocity *= 0.99f;



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

			

				if (timer > 50)
				{

					NPC.velocity.X *= 6f;
					NPC.velocity.Y *= 0.5f;
					





				}
			

                if (timer == 100)
			{
				State = ActionState.Wait;
				timer = 0;
			}

		}
	}
}