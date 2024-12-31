using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Fire.BlazingSerpent
{
	// These three class showcase usage of the WormHead, WormBody and WormTail classes from Worm.cs
	[AutoloadBossHead]
	internal class BlazingSerpentHead : WormHead
	{
		public override int BodyType => ModContent.NPCType<BlazingSerpentBody>();

		public override int TailType => ModContent.NPCType<BlazingSerpentTail>();

		public override void SetStaticDefaults()
		{

			Main.npcFrameCount[NPC.type] = 30;

		}

		public override void SetDefaults()
		{
			// Head is 10 defence, body 20, tail 30.
			NPC.CloneDefaults(NPCID.DiggerHead);
			NPC.width = 66;
			NPC.height = 86;
			NPC.damage = 70;
			NPC.defense = 10;
			NPC.lifeMax = 7500;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 5000f;
			NPC.knockBackResist = 0f;
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.aiStyle = -1;
			NPC.noGravity = true;
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/CatacombsBoss");
			}
            NPC.BossBar = ModContent.GetInstance<MiniBossBar>();

            NPC.aiStyle = -1;
		}

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Burining with passion I guess."))
			});
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			base.ModifyNPCLoot(npcLoot);
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TreasureBoxFire>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
		}


		public override void Init()
		{
			// Set the segment variance
			// If you want the segment length to be constant, set these two properties to the same value
			MinSegmentLength = 24;
			MaxSegmentLength = 36;

			CommonWormInit(this);
		}

		// This method is invoked from ExampleWormHead, ExampleWormBody and ExampleWormTail
		internal static void CommonWormInit(Worm worm)
		{
			// These two properties handle the movement of the worm
			worm.MoveSpeed = 13f;
			worm.Acceleration = 0.1f;
		}

		private int attackCounter;
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(attackCounter);
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			attackCounter = reader.ReadInt32();
		}
		public float ty = 0;
		public float Spawner = 0;
		public override void AI()
		{
			Spawner++;
			/*
			Player players = Main.player[NPC.target];
			if (Spawner == 2)

			{



				int distanceY = Main.rand.Next(-250, -250);
				NPC.position.X = players.Center.X;
				NPC.position.Y = players.Center.Y + distanceY;

			}*/
			ty++;

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (attackCounter > 0)
				{
					attackCounter--; // tick down the attack counter.
				}

				Player target = Main.player[NPC.target];

				if (ty == 100)
                {
				
					if (StellaMultiplayer.IsHost)
                    {
                        var entitySource = NPC.GetSource_FromThis();
                        int index = NPC.NewNPC(entitySource, (int)target.Center.X + Main.rand.Next(-40, 40), (int)target.Center.Y,
							ModContent.NPCType<BlazeBeamWarn>());
                    }
			
					ty = 0;
				}
				// If the attack counter is 0, this NPC is less than 12.5 tiles away from its target, and has a path to the target unobstructed by blocks, summon a projectile.
				if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 300 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1))
				{
					Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
					direction = direction.RotatedByRandom(MathHelper.ToRadians(10));

					int projectile = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 3, 
						ProjectileID.BallofFire, 60, 0, Main.myPlayer);
					int projectile2 = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 2, 
						ProjectileID.BallofFire, 60, 0, Main.myPlayer);
					Main.projectile[projectile].timeLeft = 300;
					Projectile ichor = Main.projectile[projectile];
					ichor.hostile = true;
					ichor.friendly = false;

					
					attackCounter = 500;
					NPC.netUpdate = true;
				}
			}
		}


		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedBlazingSerpent, -1);
		}
	}

	internal class BlazingSerpentBody : WormBody
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 30;
		}

		public override void SetDefaults()
		{
			NPC.CloneDefaults(NPCID.DiggerBody);
			NPC.aiStyle = -1;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void Init()
		{
			BlazingSerpentHead.CommonWormInit(this);
		}
	}

	internal class BlazingSerpentTail : WormTail
	{
		public override void SetStaticDefaults()
		{

			Main.npcFrameCount[NPC.type] = 30;

		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void SetDefaults()
		{
			NPC.CloneDefaults(NPCID.DiggerTail);
			NPC.width = 66;
			NPC.height = 86;
			NPC.aiStyle = -1;
		}

		public override void Init()
		{
			BlazingSerpentHead.CommonWormInit(this);
		}
	}
}