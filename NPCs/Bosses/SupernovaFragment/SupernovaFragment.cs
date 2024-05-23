
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Mage.Stein;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Melee.Spears;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.NPCs.Bosses.DreadMire;
using Stellamod.NPCs.Bosses.Jack;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Bosses.singularityFragment.Phase1;
using Stellamod.NPCs.Bosses.STARBOMBER.Projectiles;
using Stellamod.NPCs.Bosses.SunStalker;
using Stellamod.NPCs.Bosses.Verlia;
using System;
using System.IO;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.SupernovaFragment
{
    [AutoloadBossHead]
    public class SupernovaFragment : ModNPC
    {
        private bool Dead;
        private bool _invincible;
        public bool PH2TP = false;
        public bool PH2 = false;
        public bool Spawned = false;
        public bool TP = false;
        public bool Lazer = false;
        public int Timer = 0;
        public int PrevAttac = 0;
        public int MaxAttac = 0;
        public static int LazerType = 0;
        public static int SingularityOrbs = -1;
        public int SingularityPhaze = 0;
        public static Vector2 SingularityPos;
        public static Vector2 SingularityStart;
        public bool Superpull = false;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 30;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/SupernovaFragment/SupernovaFragmentBestiary";
            drawModifiers.PortraitScale = 1f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffType<SFBuff>(), 200);
        }

        public override void SetDefaults()
        {
            NPC.scale = 0;
            NPC.width = 100;
            NPC.height = 60;
            NPC.damage = 2;
            NPC.defense = 33;
            NPC.lifeMax = 61000;
            NPC.scale = 0.9f;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/VoidDead1") with { PitchVariance = 0.1f };
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 10f;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SupernovaFragment");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/VoidHit") with { PitchVariance = 0.1f };
            NPC.BossBar = GetInstance<SUPBossBar>();
            NPC.aiStyle = 0;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.SolarPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement("A powerful gift from Lumi V. to her trusted sisters."),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Supernova Fragment")
            });
        }

        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 3)
            {
                frame++;
                NPC.frameCounter = 0;
            }
            if (frame >= 30)
            {
                frame = 0;
            }

            NPC.frame.Y = frameHeight * frame;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TempleKeyPart>()));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SupernovaBag>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.SupernovaBossRel>()));
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Iknoctstein>(), 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Dulahaun>()));
            //notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<StalkersTallon>(), 2));
         //   notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SunBlastStaff>(), 2));
            npcLoot.Add(notExpertRule);
        
    }

        public void CasuallyApproachChild()
        {
            Player player = Main.player[NPC.target];
            NPC.velocity.Y *= 0.94f;
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.040f), NPC.Center.Distance(player.Center) * 0.20f), 0.009f);
        }

        public int rippleCount = 20;
        public int rippleSize = 5;
        public int rippleSpeed = 15;
        public float distortStrength = 300f;

        int bee = 220;
        public Vector2 LastBacklash;
        public Vector2 LastDirection;
        public int Attack;
        public int SparkCount;
        public int SparkCountMax;
        public float Spawner = 0;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Attack);
            writer.Write(LazerType);
            writer.Write(_invincible);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Attack = reader.ReadInt32();
            LazerType = reader.ReadInt32();
            _invincible = reader.ReadBoolean();
        }

        public override void AI()
        {
            Spawner++;
            if (!Superpull)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player npcs = Main.player[i];

                    if (npcs.active)
                    {
                        float distancee = Vector2.Distance(NPC.Center, npcs.Center);
                        if (distancee <= 4000)
                        {
                            Vector2 direction = npcs.Center - NPC.Center;
                            direction.Normalize();
                            npcs.velocity -= direction * 0.1f;
                        }
                    }
                }
            }
           


            PH2 = NPC.life < NPC.lifeMax * 0.6f;
            if (PH2)
            {
                MaxAttac = 6;
            }
            else
            {
                MaxAttac = 4;
            }
            NPC.AddBuff(BuffType<StarSuper>(), 10);
            var entitySource = NPC.GetSource_FromThis();
            Player player = Main.player[NPC.target];
            bool expertMode = Main.expertMode;
            if (!NPC.HasPlayerTarget)
            {
                NPC.TargetClosest(false);
                Player player1 = Main.player[NPC.target];

                if (!NPC.HasPlayerTarget || NPC.Distance(player1.Center) > 3000f)
                {
                    NPC.velocity.Y -= 1f;
                    if (NPC.timeLeft > 30)
                        NPC.timeLeft = 30;
                    return;
                }
            }

            Player playerT = Main.player[NPC.target];
            int distance = (int)(NPC.Center - playerT.Center).Length();
            if (distance > 3000f || playerT.dead)
            {
                NPC.ai[2] = 5;
                Disappear();
            }
            NPC.rotation = NPC.velocity.X * 0.03f;

            if (NPC.ai[2] == 0)
            {
                SingularityStart = NPC.position;
                NPC.scale = 0;
                Spawned = false;
                NPC.ai[2] = 1;
            }
            SingularityPos = NPC.Center;

            NPC.dontTakeDamage = _invincible;
            NPC.dontCountMe = _invincible;
            if (Spawned)
            {
                if (NPC.ai[1] >= 5)
                {
                    NPC.damage = 0;
                }
                else
                {
                    NPC.damage = 9999;
                }
            }
            else
            {
                NPC.damage = 0;
                if (StellaMultiplayer.IsHost)
                {
                    _invincible = true;
                    NPC.netUpdate = true;
                }
            }

            if (NPC.ai[2] == 1)
                switch (NPC.ai[1])
                {
                    case 0:
                        // default attack, just moves above player, waits  seconds then does a random attack
                        NPC.ai[0]++;
                        if (NPC.ai[0] > 2)
                        {
                            if (!Spawned)
                            {
                                if (NPC.ai[0] == 0)
                                {
                                    NPC.scale = 0;
                                    NPC.position.X = player.Center.X - 30;
                                    NPC.position.Y = player.Center.Y - 200;
                                }
                                if (NPC.scale == 0)
                                {
                                    Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(NPC.Center, 8f);
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPIn"), NPC.position);
                                    NPC.ai[0] = 1;
                                }
                                if (NPC.ai[0] >= 1)
                                {
                                    NPC.velocity.Y -= 0.01f;
                                    NPC.scale += 0.010f;
                                    NPC.ai[0]++;
                                    if (NPC.scale >= 1)
                                    {
                                        if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                                        {
                                            Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(NPC.Center);

                                        }

                                        if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                                        {
                                            float progress = (180f - bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                                            Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));


                                        }
                                        // Icreasian disk
                                        if (StellaMultiplayer.IsHost)
                                        {
                                            var entitySource2 = NPC.GetSource_FromThis();
                                            NPC.NewNPC(entitySource2, (int)NPC.Center.X , (int)NPC.Center.Y, ModContent.NPCType<IncresianDisc>());
                                        }


                                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb_Explode"), NPC.position);
                                        Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1212f, 62f);
                                        for (int i = 0; i < 14; i++)
                                        {
                                            Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default, 4f).noGravity = true;
                                        }
                                        for (int i = 0; i < 40; i++)
                                        {
                                            Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default, 1f).noGravity = false;
                                        }
                                        for (int i = 0; i < 40; i++)
                                        {
                                            Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default, 6f).noGravity = true;
                                        }
                                        for (int i = 0; i < 20; i++)
                                        {
                                            Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default, 2f).noGravity = false;
                                        }
                                        Spawned = true;
                                        NPC.damage = 999;
                                        NPC.ai[0] = 0;
                                        NPC.ai[1] = 0;
                                    }
                                }
                            }
                            else
                            {
                                if (PH2)
                                {
                                    if (!PH2TP && SingularityPhaze == 0)
                                    {
                                        Lazer = false;
                                        TP = false;
                                        NPC.netUpdate = true;
                                        NPC.ai[1] = 15;
                                        PH2TP = true;
                                    }
                                    else
                                    {
                                        if(SingularityPhaze == 1)
                                        {
                                            _invincible = true;
                                            Superpull = true;
                                        }
                                        if (SingularityPhaze == 2)
                                        {
                                            _invincible = false;
                                            Superpull = false;
                                        }

                                        Attack = Main.rand.Next(1, 6);
                                        NPC.ai[1] = Attack;
                                        NPC.netUpdate = true;
                                        NPC.scale = 1;
                                        if(SingularityPhaze == 1 && SingularityOrbs == 0)
                                        {
                                            NPC.life = NPC.lifeMax / 3;
                                            NPC.DelBuff(NPC.FindBuffIndex(ModContent.BuffType<SupernovaChained>()));
                                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb_Explode"), NPC.position);
                                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1212f, 62f);
                                            for (int i = 0; i < 14; i++)
                                            {
                                                Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default, 4f).noGravity = true;
                                            }
                                            for (int i = 0; i < 40; i++)
                                            {
                                                Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default, 1f).noGravity = false;
                                            }
                                            for (int i = 0; i < 40; i++)
                                            {
                                                Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default, 6f).noGravity = true;
                                            }
                                            for (int i = 0; i < 20; i++)
                                            {
                                                Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default, 2f).noGravity = false;
                                            }
                                            PH2TP = false;
                                            Superpull = false;
                                            SingularityPhaze = 2;
                                        }
                                    }

                                }
                                else
                                {
                                    _invincible = false;
                                    Attack = Main.rand.Next(1, 4);
                                    NPC.ai[1] = Attack;
                                    NPC.netUpdate = true;
                                    NPC.scale = 1;
                                }
               
                            }
                        }
                        break;
                    case 1:
                        NPC.ai[0]++;
                        if (PH2TP)
                        {
                            NPC.velocity *= 0.92f;
                        }
                        else
                        {
                            CasuallyApproachChild();
                        }
          
                        if (NPC.ai[0] == 50 || NPC.ai[0] == 150)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot"), NPC.position);
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1212f, 62f);
                            if (StellaMultiplayer.IsHost)
                            {
                                Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                                direction.Normalize();
                                int damage = Main.expertMode ? 50 : 68;
                                Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero,
                                    ModContent.ProjectileType<JackSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
                                Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero,
                                    ModContent.ProjectileType<JackSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
                                for (int j = -2; j <= 2; j++)
                                {
                                    Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(j * 0.5f) * 6f,
                                        ModContent.ProjectileType<NovaBlast>(), damage, 0f, Owner: Main.myPlayer);
                                }
                            }
                        }
                        if (NPC.ai[0] == 100 || NPC.ai[0] == 200)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot1"), NPC.position);
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1212f, 62f);
                            if (StellaMultiplayer.IsHost)
                            {
                                Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                                direction.Normalize();
                                int damage = Main.expertMode ? 50 : 68;
                                Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero,
                                    ModContent.ProjectileType<JackSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
                                Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero,
                                    ModContent.ProjectileType<JackSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
                                for (int j = -1; j <= 1; j++)
                                {
                                    Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(j * 0.5f) * 6f,
                                        ModContent.ProjectileType<NovaBlast>(), damage, 0f, Owner: Main.myPlayer);
                                }
                            }

                        }
                        if (NPC.ai[0] >= 250)
                        {
                            PrevAttac = 2;
                            NPC.ai[1] = 0;
                            NPC.ai[0] = 0;
                        }
                        break;
                    case 2:
                        NPC.ai[0]++;
                        if (PH2TP)
                        {
                            NPC.velocity *= 0.90f;
                        }
                        else
                        {
                            CasuallyApproachChild();
                        }
                        if (NPC.ai[0] == 70 || NPC.ai[0] == 100 || NPC.ai[0] == 130)
                        {
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);

                            if (StellaMultiplayer.IsHost)
                            {

                                float offsetX = Main.rand.Next(-1, 1);
                                float offsetY = Main.rand.Next(-1, 1);
                                int damage = Main.expertMode ? 50 : 68;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, (direction.X * 1.5f) + offsetX, (direction.Y * 1.5f) + offsetY,
                                    ModContent.ProjectileType<NovaFlame>(), damage, 1, Owner: Main.myPlayer);
                            }
                        }
                        if (NPC.ai[0] >= 150)
                        {
                            PrevAttac = 2;
                            NPC.ai[1] = 0;
                            NPC.ai[0] = 0;
                        }
                        break;

                    case 3:
                        NPC.ai[0]++;
                        if (PH2TP)
                        {
                            NPC.velocity *= 0.90f;
                        }
                        else
                        {
                            CasuallyApproachChild();
                        }
                        if (NPC.ai[0] == 70)
                        {
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Sun_Shot1"), NPC.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Sun_Shot2"), NPC.position);
                            }
                            float offsetRandom = Main.rand.Next(0, 50);
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 124f);
                            for (int i = 0; i < 20; i++)
                            {
                                Dust.NewDustPerfect(NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
                            }

                            float spread = 45f * 0.0174f;
                            double startAngle = Math.Atan2(1, 0) - spread / 2;
                            double deltaAngle = spread / 8f;
                            double offsetAngle;
                            for (int i = 0; i < 4; i++)
                            {
                                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                                if (StellaMultiplayer.IsHost)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)(Math.Sin(offsetAngle) * 9f), (float)(Math.Cos(offsetAngle) * 9f),
                                        ModContent.ProjectileType<NovaBomb>(), 45, 0, Main.myPlayer);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)(-Math.Sin(offsetAngle) * 9f), (float)(-Math.Cos(offsetAngle) * 9f),
                                        ModContent.ProjectileType<NovaBomb>(), 45, 0, Main.myPlayer);
                                }
                            }
                        }
                        if (NPC.ai[0] >= 100)
                        {
                            PrevAttac = 2;
                            NPC.ai[1] = 0;
                            NPC.ai[0] = 0;
                        }
                        break;

                    case 4:
                        NPC.ai[0]++;
                        if (PH2TP)
                        {
                            NPC.velocity *= 0.90f;
                        }
                        else
                        {
                            CasuallyApproachChild();
                        }
                        if (SingularityPhaze == 1)
                        {
                            if (NPC.ai[0] == 70)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X - 1050 + 175, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 80)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X - 700 + 175, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 90)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X - 350 + 175, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 100)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X + 175, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 110)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X + 350 + 175, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 120)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X + 700 + 175, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 130)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X + 1050 + 175, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] >= 250)
                            {
                                PrevAttac = 2;
                                NPC.ai[1] = 0;
                                NPC.ai[0] = 0;
                            }
     
                        }
                        else
                        {

                            if (NPC.ai[0] == 50)
                            {
                                int Ofset = Main.rand.Next(1, 100);

                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, Ofset);
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver4 + Ofset);
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver2 + Ofset);
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, -MathHelper.PiOver4 + Ofset);

                            }

                            if (NPC.ai[0] == 150)
                            {
                                int Ofset = Main.rand.Next(1, 100);
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, Ofset);
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver4 + Ofset);
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver2 + Ofset);
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, -MathHelper.PiOver4 + Ofset);


                            }
                            if (NPC.ai[0] == 250)
                            {
                                int Ofset = Main.rand.Next(1, 100);
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, Ofset);
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver4 + Ofset);
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver2 + Ofset);
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, -MathHelper.PiOver4 + Ofset);

                            }

                            if (NPC.ai[0] >= 300)
                            {
                                PrevAttac = 2;
                                NPC.ai[1] = 0;
                                NPC.ai[0] = 0;
                            }
                        }

                        break;
                    case 5:
                        NPC.ai[0]++;
                        if (SingularityPhaze == 1)
                        {
                            if (NPC.ai[0] == 70)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X - 1050, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 80)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X - 700, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 90)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X - 350, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 100)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 110)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X + 350, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 120)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X + 700, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] == 130)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X + 1050, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarn>());
                                }
                            }
                            if (NPC.ai[0] >= 250)
                            {
                                PrevAttac = 2;
                                NPC.ai[1] = 0;
                                NPC.ai[0] = 0;
                            }
                        }
                        else
                        {
                            NPC.velocity *= 0.90f;
                            if (NPC.ai[0] == 5)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge"));
                                }
                            }
                            if (NPC.ai[0] == 150)
                            {
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 424f);
                                Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero, ModContent.ProjectileType<SupernovaGodExplosion>(), 800, 0f, Owner: Main.myPlayer);

                            }
                            if (NPC.ai[0] >= 200)
                            {
                                PrevAttac = 2;
                                NPC.ai[1] = 0;
                                NPC.ai[0] = 0;
                            }
                        }
                        break;
                    case 6:
                        NPC.ai[0]++;
        

                        break;
                    case 15:
    
                        NPC.ai[0]++;
                        if (!Lazer)
                        {
                            if (NPC.ai[0] <= 2)
                            {
                                NPC.damage = 0;
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPOut"), NPC.position);
                            }
                            if (!TP)
                            {
                                NPC.velocity.Y += 0.05f;
                                NPC.scale -= 0.015f;
                                if (NPC.scale <= 0)
                                {
                                    NPC.scale = 0;
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPIn"), NPC.position);
                                    TP = true;
                                    NPC.velocity.Y = 0;
                                    NPC.position = player.Center;
                                }
                            }
                            else
                            {
                                NPC.scale += 0.015f;
                                if (NPC.scale >= 1)
                                {
                                    NPC.AddBuff(ModContent.BuffType<SupernovaChained>(), 9999999);
                                    float radius = 900;
                                    float rot = MathHelper.TwoPi / 7;
                                    for (int I = 0; I < 7; I++)
                                    {
                                        SingularityOrbs = 7;
                                        if (StellaMultiplayer.IsHost)
                                        {
                                            Vector2 position = NPC.Center + radius * (I * rot).ToRotationVector2();
                                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(position.X), (int)(position.Y),
                                                ModContent.NPCType<SupernovaOrb>(), NPC.whoAmI, NPC.whoAmI, I * rot, radius);
                                        }
                                    }
     
                                    SingularityPhaze = 1;
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb_Explode"), NPC.position);
                                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1212f, 62f);
                                    for (int i = 0; i < 14; i++)
                                    {
                                        Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default, 4f).noGravity = true;
                                    }
                                    for (int i = 0; i < 40; i++)
                                    {
                                        Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default, 1f).noGravity = false;
                                    }
                                    for (int i = 0; i < 40; i++)
                                    {
                                        Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default, 6f).noGravity = true;
                                    }
                                    for (int i = 0; i < 20; i++)
                                    {
                                        Dust.NewDustPerfect(NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default, 2f).noGravity = false;
                                    }
                                    NPC.ai[1] = 0;
                                    PH2TP = true;               
                                    NPC.scale = 1;
                                    NPC.damage = 99999;
                                    TP = false;
                                    _invincible = true;
                                    NPC.ai[0] = 0;
                                }
                            }
                        }
                        break;
                }
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedSupernovaFragmentBoss, -1);

        }

        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, target, 0.5f), NPC.Center.Distance(target) * Speed), 0.1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Lighting.AddLight(NPC.Center, Color.Orange.ToVector3() * 1.25f * Main.essScale);
            Texture2D texture = Request<Texture2D>(Texture).Value;


            Vector2 frameSize = NPC.frame.Size();
            Vector2 drawPos = NPC.Center - screenPos + frameSize / 2;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, NPC.frame, new Color(255, 233, 197, 50), NPC.rotation, frameSize, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 16f).RotatedBy(radians) * time, NPC.frame, new Color(244, 142, 72, 77), NPC.rotation, frameSize, NPC.scale, Effects, 0);
            }

            return true;
        }
        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.Goldenrod * num107 * .8f;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(
                GlowTexture,
                NPC.Center - Main.screenPosition + Drawoffset,
                NPC.frame,
                color1,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                effects,
                0
            );
            SpriteEffects spriteEffects3 = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Goldenrod);
            for (int num103 = 0; num103 < 1; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
        }

        private void Disappear()
        {
            if (!Dead)
            {
                NPC.DelBuff(NPC.FindBuffIndex(ModContent.BuffType<SupernovaChained>()));
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPOut"), NPC.position);
                Dead = true;
            }
            NPC.velocity.Y += 0.1f;
            NPC.scale -= 0.01f;
            if (NPC.scale <= 0)
            {

                NPC.active = false;
            }
        }
    }
}
