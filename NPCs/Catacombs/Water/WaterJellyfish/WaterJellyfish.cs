using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Water.WaterJellyfish
{
    [AutoloadBossHead]
	public class WaterJellyfish : ModNPC
	{
		// States
		public enum AttackState
		{
			Idle,
			Anger,
			Lightning_Attack_1,
			Lightning_Attack_2,
			Lightning_Attack_3
		}

		// Current state
		public int frameTick;

		// Current state's timer
		public float timer;

		// Current frame
		public int frameCounter;

		// AI counter
		public int counter;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 60;
		}

		public override void SetDefaults()
		{
			NPC.width = 110; // The width of the npc's hitbox (in pixels)
			NPC.height = 82; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = 10; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 100; // The amount of damage that this npc deals
			NPC.defense = 10; // The amount of defense that this npc has
			NPC.lifeMax = 6000; // The amount of health that this npc has
			NPC.HitSound = SoundID.NPCHit25; // The sound the NPC will make when being hit.
			NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.scale = 2f;

			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.knockBackResist = 0f;
			NPC.npcSlots = 10f;
			NPC.value = Item.buyPrice(gold: 10);
			NPC.BossBar = ModContent.GetInstance<BossBarTest2>();
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/CatacombsBoss");
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Water, 1, -1f, 1, default, .61f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, 1, -1f, 1, default, .61f);
			}
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		private Vector2 _nextLightningPosition;
		private float ai_Counter;
		private float ai_State;
		private float ai_Attack_Counter;

		private void SwitchState(AttackState attackState)
        {
			ai_Counter = 0;
			ai_State = (float)attackState;
        }

		public override void AI()
		{           
			//No contact damage
			NPC.damage = 0;
			NPC.spriteDirection = NPC.direction;
			if (!NPC.HasValidTarget)
            {
				NPC.TargetClosest();
				NPC.EncourageDespawn(120);
				return;
			}
				

			Player target = Main.player[NPC.target];
			AttackState attackState = (AttackState)ai_State;
			if (ai_Counter % 4 == 0)
			{
				var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(512, 512), DustID.Electric, Main.rand.NextVector2CircularEdge(4f, 4f), Scale: 2f);
				d.noGravity = true;
				var d2 = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(512, 512), DustID.Water, Main.rand.NextVector2CircularEdge(4f, 4f), Scale: 2f);
				d2.noGravity = true;
			}

			switch (attackState)
            {
				case AttackState.Idle:
					ai_Counter++;
			
					NPC.aiStyle = 10;
					if(ai_Counter > 120)
                    {
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain"));
						SwitchState(AttackState.Anger);
                    }
                    break;

                case AttackState.Anger:
					ai_Counter++;
					NPC.velocity *= 0.5f;
					if (ai_Counter > 30)
                    {
                        switch (ai_Attack_Counter)
                        {
							case 0:
								SwitchState(AttackState.Lightning_Attack_1);
								ai_Attack_Counter = 1;
								break;
                            case 1:
								SwitchState(AttackState.Lightning_Attack_2);
								ai_Attack_Counter = 2;
								break;
							case 2:
								SwitchState(AttackState.Lightning_Attack_3);
								ai_Attack_Counter = 0;
								break;
                        }
                    }
                    break;

                case AttackState.Lightning_Attack_1:
					NPC.velocity *= 0.25f;
					if (ai_Counter == 0)
					{
						Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(target.Center, 1024f, 32f);
					}

					ai_Counter++;
					if(ai_Counter % 10 == 0)
                    {			
						Vector2 velocity = target.DirectionFrom(_nextLightningPosition) * 3;

						Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), _nextLightningPosition, velocity,
							ProjectileID.DD2LightningBugZap, 29, 1);
						projectile.tileCollide = false;
						projectile.timeLeft = 60;
						for (int i = 0; i < 16; i++)
						{
							Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
							var d = Dust.NewDustPerfect(_nextLightningPosition, DustID.Electric, speed, Scale: 1.5f);
							d.noGravity = true;
						}

						SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap);
					}
					else if (ai_Counter % 5 == 0)
					{
						Vector2 circleOffset = Main.rand.NextVector2CircularEdge(256, 256);
						_nextLightningPosition = target.Center + circleOffset;

						for (int i = 0; i < 16; i++)
						{
							Vector2 speed = Main.rand.NextVector2CircularEdge(1, 1);
							var d = Dust.NewDustPerfect(_nextLightningPosition, DustID.Electric, speed, Scale: 1.5f);
							d.noGravity = true;
						}

						SoundEngine.PlaySound(SoundID.DD2_LightningBugZap);
					}
					else if (ai_Counter > 180)
                    {
						SwitchState(AttackState.Idle);
                    }

					break;

				case AttackState.Lightning_Attack_2:
					NPC.velocity *= 0.25f;
					if (ai_Counter == 0)
					{
						Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(target.Center, 1024f, 32f);
						SoundEngine.PlaySound(SoundID.Item121);
					}

					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(0, -32), Vector2.Zero,
						ProjectileID.CultistBossLightningOrb, 40, 1);
					SwitchState(AttackState.Idle);
					break;
				case AttackState.Lightning_Attack_3:
					NPC.velocity *= 0.25f;
					if (ai_Counter == 0)
					{
						Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(target.Center, 1024f, 32f);
						SoundEngine.PlaySound(SoundID.Item121);
					}

					ai_Counter++;
					if (ai_Counter % 10 == 0)
					{
						Vector2 velocity = target.DirectionFrom(_nextLightningPosition) * 6;

						Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), _nextLightningPosition, velocity,
							ProjectileID.DD2LightningBugZap, 40, 1);
						projectile.tileCollide = false;
						projectile.timeLeft = 60;
						for (int i = 0; i < 16; i++)
						{
							Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
							var d = Dust.NewDustPerfect(_nextLightningPosition, DustID.Electric, speed, Scale: 1.5f);
							d.noGravity = true;
						}

						SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap);
					}
					else if (ai_Counter % 5 == 0)
					{
						_nextLightningPosition = target.Center + new Vector2(0, -256);

						for (int i = 0; i < 16; i++)
						{
							Vector2 speed = Main.rand.NextVector2CircularEdge(1, 1);
							var d = Dust.NewDustPerfect(_nextLightningPosition, DustID.Electric, speed, Scale: 1.5f);
							d.noGravity = true;
						}

						SoundEngine.PlaySound(SoundID.DD2_LightningBugZap);
					}
					else if (ai_Counter > 180)
					{
						SwitchState(AttackState.Idle);
					}
					break;
            }
        }

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TreasureBoxWater>(), 1, 1, 1));
		}


		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedWaterJellyfish, -1);
		}
	}
}