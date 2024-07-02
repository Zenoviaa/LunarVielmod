using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Wings;
using Stellamod.Items.Armors.Miracle;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia
{
    public class SyliaSkyPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (NPC.AnyNPCs(ModContent.NPCType<Sylia>()))
            {
				ActivateSyliaSky();

			}
            else
            {
				DeActivateSyliaSky();
            }
        }

		private void ActivateSyliaSky()
		{
			if (!SkyManager.Instance["Stellamod:SyliaSky"].IsActive())
			{
				Vector2 targetCenter = Player.Center;
				SkyManager.Instance.Activate("Stellamod:SyliaSky", targetCenter);
			}
		}

		private void DeActivateSyliaSky()
		{
			if (SkyManager.Instance["Stellamod:SyliaSky"].IsActive())
			{
				Vector2 targetCenter = Player.Center;
				SkyManager.Instance.Deactivate("Stellamod:SyliaSky", targetCenter);
			}
		}
	}

	[AutoloadBossHead]
	public partial class Sylia : ModNPC
	{
		public const float ArenaRadius = 768;
		private bool _resetAI;
        private float _teleportX;
		private float _teleportY;


		private ActionPhase _attackPhase;

		public enum ActionState
		{
			Idle,
			XScissor,
			XScissor_Horizontal,
			QuickSlash_V2,
			QuickSlash,
			Scissor_Rain
		}

		public enum ActionPhase
		{
			Intro,
			Phase_1,
			Phase_2_Transition,
			Phase_2,
			Fight_End
		}


		public ref float Timer => ref NPC.ai[0];
		public ref float TelegraphTimer => ref NPC.ai[1];
		public ActionState State
		{
			get
			{
				return (ActionState)NPC.ai[2];
			}
			private set
			{
				NPC.ai[2] = (float)value;
			}
		}

		public ActionPhase Phase { get; private set; }
		public int AttackCycle { get; private set; }
		public Vector2 ArenaCenter { get; private set; }
		public Player Target => Main.player[NPC.target];

		private void SwitchState(ActionState state)
		{
			State = state;
            ResetAI();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
			return false;
        }

        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 30;
			NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
			drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Sylia/SyliaPreview";
			drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
			drawModifiers.PortraitPositionYOverride = 0f;
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			NPCID.Sets.TrailCacheLength[Type] = 60;
			NPCID.Sets.TrailingMode[Type] = 2;
		}



        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Someone who was consumed by their dark magic, even more so than Fenix would bother with.")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Sylia, Merena's Sister.", "2"))
            });
        }

        //AI Values
        public override void SetDefaults()
		{
			NPC.Size = new Vector2(24, 48);

			NPC.damage = 40;
			NPC.defense = 26;
			NPC.lifeMax = 28000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 10);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.scale = 1f;

			// Take up open spawn slots, preventing random NPCs from spawning during the fight
			NPC.npcSlots = 10f;

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			NPC.aiStyle = -1;

			// Custom boss bar
			NPC.BossBar = ModContent.GetInstance<SyliaBossBar>();

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Sylia");
			}
		}

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
			writer.Write((float)Phase);
			writer.Write(AttackCycle);
			writer.WriteVector2(ArenaCenter);
            writer.WriteVector2(QuickSlashV2Start);
            writer.WriteVector2(QuickSlashV2Velocity);
			writer.Write(_teleportX);
			writer.Write(_teleportY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
			Phase = (ActionPhase)reader.ReadSingle();
			AttackCycle = reader.ReadInt32();
			ArenaCenter = reader.ReadVector2();
            QuickSlashV2Start = reader.ReadVector2();
            QuickSlashV2Velocity = reader.ReadVector2();
			_teleportX = reader.ReadSingle();
			_teleportY = reader.ReadSingle();
        }

        private void MoveTo(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player

            float distX = targetCenter.X - NPC.Center.X;
            if (NPC.Center.X < targetCenter.X && NPC.velocity.X < moveSpeed)
            {
                NPC.velocity.X += accel;
            }
            else if (NPC.Center.X > targetCenter.X && NPC.velocity.X > -moveSpeed)
            {
                NPC.velocity.X -= accel;
            }

            //Accelerate to being above the player.
            float distY = targetCenter.Y - NPC.Center.Y;
            if (NPC.Center.Y < targetCenter.Y && NPC.velocity.Y < moveSpeed)
            {
                NPC.velocity.Y += accel;
            }
            else if (NPC.Center.Y > targetCenter.Y && NPC.velocity.Y > -moveSpeed)
            {
                NPC.velocity.Y -= accel;
            }
        }

		private void ChargeVisuals(float timer, float maxTimer)
		{
            float progress = timer / maxTimer;
            float minParticleSpawnSpeed = 8;
            float maxParticleSpawnSpeed = 2;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(168, 168);
                    Vector2 vel = (NPC.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                    ParticleManager.NewParticle<VoidParticle>(pos, vel, Color.White, 1f);
                    if (i % 2 == 0)
                    {
                        var d = Dust.NewDustPerfect(pos, DustID.GemAmethyst, vel);
						d.noGravity = true;
                    }
                }
            }
        }

        private void FinishResetAI()
		{
			if (_resetAI)
            {
				Timer = 0;
				TelegraphTimer = 0;
                _resetAI = false;
			}
		}

        private void FinishTeleport()
        {
			if(_teleportX != 0 || _teleportY != 0)
			{
                NPC.position.X = _teleportX;
				NPC.position.Y = _teleportY;
                _teleportX = 0;
                _teleportY = 0;

                //Visuals on the teleport
                Dust.QuickDustLine(NPC.position, NPC.oldPosition, 100f, Color.Violet);
                for (int i = 0; i < 64; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    Particle p = ParticleManager.NewParticle(NPC.Center, speed, ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), 1 / 3f);
                    p.layer = Particle.Layer.BeforeProjectiles;
                }

                SoundEngine.PlaySound(SoundID.Item165, NPC.position);
            }
        }

        private void ResetAI()
		{
			if (StellaMultiplayer.IsHost)
			{
				_resetAI = true;
				NPC.netUpdate = true;
			}
		}


		private void Teleport(float x, float y)
		{
			if (StellaMultiplayer.IsHost)
			{
				_teleportX = x;
				_teleportY = y;
				NPC.netUpdate = true;
			}
		}

		public override void AI()
        {
			//Despawn code
			NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                //Despawn in 2 seconds
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, -8), 0.025f);
                NPC.EncourageDespawn(60);
                return;
            }

            switch (Phase)
			{
				case ActionPhase.Intro:
					AIIntro();
					break;
				case ActionPhase.Phase_1:
					AIPhase1();
					break;
                case ActionPhase.Phase_2_Transition:
                    AIPhase2Transition();
                    break;
                case ActionPhase.Phase_2:
					AIPhase2();
					break;
			}

			//We do everything the frame after so that it smoothly spawns on the client
			FinishResetAI();
			FinishTeleport();
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SyliaBag>()));
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.SyliaBossRel>()));

			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MiracleThread>(), minimumDropped: 30, maximumDropped: 40));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MiracleWings>(), chanceDenominator: 4));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SewingKit>(), chanceDenominator: 4));

            IItemDropRule armorRule = ItemDropRule.Common(ModContent.ItemType<MiracleHead>(), chanceDenominator: 4);
            armorRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MiracleBody>(), 1));
            notExpertRule.OnSuccess(armorRule);
            npcLoot.Add(notExpertRule);
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
			if(StellaMultiplayer.IsHost && NPC.life <= 0)
			{
				NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SyliaDeath>());
			}
        }
        public override void OnKill()
        {   
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedSyliaBoss, -1);
		}
    }
}
