using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Minibosses
{
	public class GoliathCryogenicJellyfish : ModNPC
	{
		// States
		

		public enum ActionState
		{

			Speed,
			Wait
		}
		// Current state
		public int frameTick;
		// Current state's timer
		public float timer;


		// Current frame
		public int frameCounter;
		// Current frame's progress
		public float Mainframe;

		// AI counter
		public int counter;
		public ActionState State = ActionState.Wait;
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 60;

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
		}
		public override void SetDefaults()
		{
			NPC.width = 115; // The width of the npc's hitbox (in pixels)
			NPC.height = 85; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 200; // The amount of damage that this npc deals
			NPC.defense = 10; // The amount of defense that this npc has
			NPC.lifeMax = 4000; // The amount of health that this npc has
			NPC.HitSound = SoundID.NPCHit25; // The sound the NPC will make when being hit.
			NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
			NPC.value = 500f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.scale = 2f;
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			/*
			bool npcAlreadyExists = false;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.type == ModContent.NPCType<GoliathJellyfish>() || npc.type == ModContent.NPCType<GoliathCryogenicJellyfish>())
				{
					npcAlreadyExists = true;
					break;
				}
			}

			if (npcAlreadyExists)
			{
				return 0f;
			}

			if (spawnInfo.Player.ZoneSnow || spawnInfo.Player.InModBiome<AurelusBiome>())
			{
				return SpawnCondition.Cavern.Chance * 0.01f;
			}*/
			return 0f;
		}
		int invisibilityTimer;
		int invsTimer;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 1, -1f, 1, default, .61f);
			}


		}


        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
			for (int k = 0; k < 5; k++)
			{



				float speedXB = NPC.velocity.X * Main.rand.NextFloat(-0.5f, 0.5f);
				float speedY = NPC.velocity.Y + Main.rand.Next(-4, 4);
				int fireball = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXB * 1.5f, speedY * 3, ProjectileID.CrystalShard, 13, 0f, 0, 0f, 0f);
				Projectile ichor = Main.projectile[fireball];
				ichor.hostile = true;
				ichor.friendly = false;
			}
		}

        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			invsTimer++;
			Mainframe++;


			if (invsTimer < 255)
			{
				NPC.alpha++;
			}

			if (invsTimer > 255)
			{
				NPC.alpha--;
			}

			if (invsTimer > 510)
			{
				invsTimer = 0;
			}

			invisibilityTimer++;
			if (invisibilityTimer >= 100)
			{
				Speed();

				for (int k = 0; k < 11; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, NPC.direction, -1f, 1, default, .61f);


				invisibilityTimer = 0;
			}



			if (Mainframe == 2)
			{
			
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/TheWondering"));			
				ShakeModSystem.Shake = 4;
				Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 4f);


			}

			switch (State)
			{

				case ActionState.Wait:
					counter++;
					Wait();
					NPC.aiStyle = 10;
					NPC.velocity *= 0.95f;
					break;

				case ActionState.Speed:
					counter++;
					Speed();
					NPC.aiStyle = 10;
					NPC.velocity *= 0.98f;
				
					break;


				default:
					counter++;
					break;
			}
		}


		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.FrostburnArrow, 1, 1, 9999));
			npcLoot.Add(ItemDropRule.Common(ItemID.IceSkates, 2, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyfishTissue>(), 2, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrileOre>(), 1, 1, 2000));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizMetal>(), 2, 1, 20));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MorrowedJelliesBroochA>(), 3, 1, 1));

		}


		public void Wait()
		{
			timer++;

			if (timer > 50)
			{

				NPC.oldVelocity *= 0.99f;



			}
			if (timer == 60)
			{
				State = ActionState.Speed;
				timer = 0;
			}
		}

		public void Speed()
		{
			timer++;
			if (timer == 9)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(4))
				{
					case 0:
						NPC.velocity = new Vector2(NPC.direction * 0, -5f);
						break;
					case 1:
						NPC.velocity = new Vector2(NPC.direction * 0, -5f);
						break;
					case 2:
						NPC.velocity = new Vector2(NPC.direction * 0, -5f);
						break;
					case 3:

						NPC.velocity = new Vector2(NPC.direction * 0, -5f);
						break;

				}



				for (int k = 0; k < 5; k++)
				{


					float speedXB = NPC.velocity.X * Main.rand.NextFloat(-0.5f, 0.5f);
					float speedY = NPC.velocity.Y + Main.rand.Next(-4, 4);
                    int fireball = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXB * 1.5f, speedY * 3, ProjectileID.CrystalShard, 13, 0f, 0, 0f, 0f);       
					Projectile ichor = Main.projectile[fireball];
					ichor.hostile = true;
					ichor.friendly = false;
				}
			}
				if (timer > 50)
			{

				for (int k = 0; k < 5; k++)
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, NPC.direction, -1f, 1, default, .61f);
				}


				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

				for (int k = 0; k < 5; k++)
				{


					float speedXB = NPC.velocity.X * Main.rand.NextFloat(-0.5f, 0.5f);
					float speedY = NPC.velocity.Y + Main.rand.Next(-4, 4);
					int fireball = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXB * 3f, speedY * 3, ProjectileID.CrystalShard, 13, 0f, 0, 0f, 0f);
					Projectile ichor = Main.projectile[fireball];
					ichor.hostile = true;
					ichor.friendly = false;
				}

				// GeneralStellaUtilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, 0, -10, ModContent.ProjectileType<VRay>(), 50, 0f, -1, 0, NPC.whoAmI);

			}

				
					
				



			

			if (timer == 100)
			{
				State = ActionState.Wait;
				timer = 0;
			}

		}
	}
}