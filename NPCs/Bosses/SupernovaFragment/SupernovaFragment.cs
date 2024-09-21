
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Mage.Stein;
using Stellamod.NPCs.Bosses.Jack;
using Stellamod.NPCs.Bosses.Verlia;
using System;
using System.IO;
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
        private float _incresionDiskFrameBottom = 0;
        private float _incresionDiskFrameTop = 0;

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

        private ref float AITimer => ref NPC.ai[0];
        private ref float AIState => ref NPC.ai[1];
        private Player TargetPlayer => Main.player[NPC.target]; 
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 60;
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
            NPC.defense = 52;
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
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A powerful gift from Lumi V. to her trusted sisters.")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Supernova Fragment", "2"))
            });
        }

        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 1)
            {
                frame++;
                NPC.frameCounter = 0;
            }
            if (frame >= 60)
            {
                frame = 0;
            }

            NPC.frame.Y = frameHeight * frame;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 5, 13));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Superfragment>(), 1, 20, 45));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TempleKeyPart>()));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SupernovaBag>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.SupernovaBossRel>()));
            
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Iknoctstein>(), 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Dulahaun>()));
            npcLoot.Add(notExpertRule);
        }   

        public void CasuallyApproachChild()
        {
            //This will make him go a bit further
            Player player = Main.player[NPC.target];
            Vector2 diff = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            Vector2 targetCenter = player.Center + diff * 48;

            NPC.velocity.Y *= 0.94f;
            NPC.velocity = Vector2.Lerp(NPC.velocity,
                VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, targetCenter, 0.040f), NPC.Center.Distance(player.Center) * 0.20f), 0.009f);
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
            //Update Frame Counters
            DrawHelper.UpdateFrame(ref _incresionDiskFrameBottom, 0.8f, 1, 40);
            DrawHelper.UpdateFrame(ref _incresionDiskFrameTop, 0.8f, 1, 76);

            Spawner++;
            if (SingularityPhaze == 2)
            {
                int buffType = ModContent.BuffType<SupernovaChained>();
                int buffIndex = NPC.FindBuffIndex(buffType);
                if (buffIndex != -1)
                {
                    NPC.DelBuff(buffIndex);
                }
            }
            else if (SingularityPhaze == 1)
            {
                int buffType = ModContent.BuffType<SupernovaChained>();
                NPC.AddBuff(buffType, 99999);
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
                if (AIState >= 5)
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
                switch (AIState)
                {
                    case 0:
                        // default attack, just moves above player, waits  seconds then does a random attack
                        AI_Spawn();
                        break;
                    case 1:
                        AI_SpreadShot();
                        break;
                    case 2:
                        AI_SniperShot();
                        break;

                    case 3:
                        AI_NovaBomb();
                        break;

                    case 4:
                        AI_ZapLaser1();
                        break;

                    case 5:
                        AI_ZapLaser2();
                        break;

                    case 6:
                        AITimer++; 
                        break;

                    case 7:
                        AI_Ram();
                        break;

                    case 8:
                        AI_SweepingLaser();
                        break;

                    case 15:
                        AI_SpinnyLaser();
                        break;
                }
        }

        private void AI_Spawn()
        {
            AITimer++;
            if (AITimer > 2)
            {
                if (!Spawned)
                {
                    if (AITimer == 0)
                    {
                        NPC.scale = 0;
                        NPC.position.X = TargetPlayer.Center.X - 30;
                        NPC.position.Y = TargetPlayer.Center.Y - 200;
                    }
                    if (NPC.scale == 0)
                    {
                        Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(NPC.Center, 8f);
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPIn"), NPC.position);
                        AITimer = 1;
                    }
                    if (AITimer >= 1)
                    {
                        NPC.velocity.Y -= 0.01f;
                        NPC.scale += 0.010f;
                        AITimer++;
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
                            AITimer = 0;
                            AIState = 0;
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
                            AIState = 15;
                            PH2TP = true;
                        }
                        else
                        {
                            if (SingularityPhaze == 1)
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
                            AIState = Attack;
                            NPC.netUpdate = true;
                            NPC.scale = 1;
                            if (SingularityPhaze == 1 && SingularityOrbs == 0)
                            {
                                NPC.life = NPC.lifeMax / 2;
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
                        AIState = Attack;
                        NPC.netUpdate = true;
                        NPC.scale = 1;
                    }

                }
            }
        }

        private void AI_SpreadShot()
        {
            var entitySource = NPC.GetSource_FromThis();
            AITimer++;
            if (PH2TP)
            {
                NPC.velocity *= 0.92f;
            }
            else
            {
                CasuallyApproachChild();
            }

            if (AITimer == 50 || AITimer == 150)
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
            if (AITimer == 100 || AITimer == 200)
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
            if (AITimer >= 250)
            {
                PrevAttac = 2;
                AIState = 0;
                AITimer = 0;
            }
        }

        private void AI_SniperShot()
        {
            AITimer++;
            if (PH2TP)
            {
                NPC.velocity *= 0.90f;
            }
            else
            {
                CasuallyApproachChild();
            }
            if (AITimer == 70 || AITimer == 100 || AITimer == 130)
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
            if (AITimer >= 150)
            {
                PrevAttac = 2;
                AIState = 0;
                AITimer = 0;
            }
        }

        private void AI_NovaBomb()
        {
            AITimer++;
            if (PH2TP)
            {
                NPC.velocity *= 0.90f;
            }
            else
            {
                CasuallyApproachChild();
            }
            if (AITimer == 70)
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
            if (AITimer >= 100)
            {
                PrevAttac = 2;
                AIState = 0;
                AITimer = 0;
            }
        }

        private void AI_ZapLaser1()
        {
            var entitySource = NPC.GetSource_FromThis();
            AITimer++;
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
                int laserProjType = ModContent.ProjectileType<SupernovaZapwarn>();
                Vector2 spawnVelocity = Vector2.Zero;
                if (AITimer == 70)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X - 1050 + 175, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 80)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X - 700 + 175, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 90)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X - 350 + 175, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 100)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X + 175, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 110)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X + 350 + 175, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 120)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X + 700 + 175, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 130)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X + 1050 + 175, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer >= 250)
                {
                    PrevAttac = 2;
                    AIState = 0;
                    AITimer = 0;
                }

            }
            else
            {

                if (AITimer == 50)
                {
                    int Ofset = Main.rand.Next(1, 100);

                    if (StellaMultiplayer.IsHost)
                    {
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, Ofset);
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver4 + Ofset);
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver2 + Ofset);
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, -MathHelper.PiOver4 + Ofset);
                    }


                }

                if (AITimer == 150)
                {
                    int Ofset = Main.rand.Next(1, 100);
                    if (StellaMultiplayer.IsHost)
                    {
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, Ofset);
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver4 + Ofset);
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver2 + Ofset);
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, -MathHelper.PiOver4 + Ofset);
                    }
                }
                if (AITimer == 250)
                {
                    int Ofset = Main.rand.Next(1, 100);
                    if (StellaMultiplayer.IsHost)
                    {
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, Ofset);
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver4 + Ofset);
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, MathHelper.PiOver2 + Ofset);
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SupernovaZapwarnFinal>(), 0, 0, -MathHelper.PiOver4 + Ofset);
                    }
                }

                if (AITimer >= 300)
                {
                    PrevAttac = 2;
                    AIState = 0;
                    AITimer = 0;
                }
            }
        }

        private void AI_ZapLaser2()
        {
            var entitySource = NPC.GetSource_FromThis();
            AITimer++;
            if (SingularityPhaze == 1)
            {
                int laserProjType = ModContent.ProjectileType<SupernovaZapwarn>();
                Vector2 spawnVelocity = Vector2.Zero;
                if (AITimer == 70)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X - 1050, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 80)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X - 700, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 90)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X - 350, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 100)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 110)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X + 350, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 120)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X + 700, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer == 130)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(NPC.Center.X + 1050, NPC.Center.Y);
                        Projectile.NewProjectile(entitySource, spawnPos, spawnVelocity, laserProjType, 250, 1, Main.myPlayer);
                    }
                }
                if (AITimer >= 250)
                {
                    PrevAttac = 2;
                    AIState = 0;
                    AITimer = 0;
                }
            }
            else
            {
                NPC.velocity *= 0.90f;
                if (AITimer == 5)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge"));
                    }
                }
                if (AITimer == 150)
                {
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 424f);
                    Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero, ModContent.ProjectileType<SupernovaGodExplosion>(), 800, 0f, Owner: Main.myPlayer);

                }
                if (AITimer >= 200)
                {
                    PrevAttac = 2;
                    AIState = 0;
                    AITimer = 0;
                }
            }
        }

        private void AI_SpinnyLaser()
        {

            AITimer++;
            if (!Lazer)
            {
                if (AITimer <= 2)
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
                        NPC.position = TargetPlayer.Center;
                    }
                }
                else
                {
                    NPC.scale += 0.015f;
                    NPC.AddBuff(ModContent.BuffType<SupernovaChained>(), 9999999);
                    if (NPC.scale >= 1)
                    {

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
                        AIState = 0;
                        PH2TP = true;
                        NPC.scale = 1;
                        NPC.damage = 99999;
                        TP = false;
                        _invincible = true;
                        AITimer = 0;
                    }
                }
            }
        }

        private void AI_Ram()
        {

        }

        private void AI_SweepingLaser()
        {

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
            DrawIncresionDiskBottom(spriteBatch, screenPos, lightColor);
            return true;
        }

        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        { 
            DrawIncresionDiskTop(spriteBatch, screenPos, drawColor);
        }

        private void DrawIncresionDiskBottom(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameBottom, columns: 5, frameWidth: 400, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(Texture + "_Disk").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;
            drawPos += new Vector2(4, -44);

            float drawScale = NPC.scale;
            float drawRotation = NPC.rotation;
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
       
        private void DrawIncresionDiskTop(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameTop, columns: 4, frameWidth: 480, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(Texture + "_Top").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;
            drawPos += new Vector2(4, -28);

            float drawScale = NPC.scale  * 1.5f;
            float drawRotation = NPC.rotation;
 
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        private void Disappear()
        {
            if (!Dead)
            {
                int buffIndex = NPC.FindBuffIndex(ModContent.BuffType<SupernovaChained>());
                if(buffIndex != -1)
                {
                    NPC.DelBuff(buffIndex);
                }
            
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
