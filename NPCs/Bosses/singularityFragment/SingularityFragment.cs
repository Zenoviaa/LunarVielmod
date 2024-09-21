
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.NPCs.Bosses.singularityFragment.Phase1;
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

namespace Stellamod.NPCs.Bosses.singularityFragment
{
    [AutoloadBossHead]
    public class SingularityFragment : ModNPC
    {
        private float _incresionDiskFrameBottom = 0;
        private bool Dead;
        private bool _invincible;
        public bool PH2 = false;
        public bool Spawned = false;
        public bool TP = false;
        public bool Lazer = false;
        public int Timer = 0;
        public int PrevAttac = 0;
        public int MaxAttac = 0;
        public int Starter;
        public static int LazerType = 0;
        public static int SingularityOrbs = 0;
        public static Vector2 SingularityPos;
        public static Vector2 SingularityStart;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 60;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(Type);
          
            // Influences how the NPC looks in the Bestiary

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/singularityFragment/SingularityFragmentBestiary";
            drawModifiers.PortraitScale = 1f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A powerful gift that was given to Cozmire, yet was stolen away by Fenix to seal away Verlia.")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Singularity Fragment", "2"))
            });
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return Spawner > 30;
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
            NPC.defense = 11;
            NPC.lifeMax = 4500;
            NPC.scale = 0.9f;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/VoidDead1") with { PitchVariance = 0.1f };
            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 10f;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SingularityFragment");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/VoidHit") with { PitchVariance = 0.1f };
            NPC.BossBar = ModContent.GetInstance<SInBossBar>();
            NPC.aiStyle = 0;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 2)
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
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.SOMBossRel>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VoidLantern>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SingularityBag>()));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SpacialDistortionFragments>(), minimumDropped: 40, maximumDropped: 65));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TomeOfTheSingularity>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<VoidBlaster>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<VoidStaff>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<EventHorizon>(), chanceDenominator: 2));
            npcLoot.Add(notExpertRule);
        }

        public void CasuallyApproachChild()
        {
            Player player = Main.player[NPC.target];
            NPC.velocity.Y *= 0.94f;
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.008f);
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
            DrawHelper.UpdateFrame(ref _incresionDiskFrameBottom, 0.8f, 1, 40);
            Spawner++;
            PH2 = NPC.life < NPC.lifeMax * 0.4f;
            if (PH2)
            {
                MaxAttac = 7;
            }
            else             
            {
                MaxAttac = 5;
            }

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
                    if (StellaMultiplayer.IsHost)
                    {
                        _invincible = true;
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    if (SingularityOrbs > 0)
                    {
                        SparkCountMax = 3;
                        if (StellaMultiplayer.IsHost)
                        {
                            _invincible = true;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (StellaMultiplayer.IsHost)
                        {
                            _invincible = false;
                            NPC.netUpdate = true;
                        }
                    }
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


                                        float radius = 250;
                                        float rot = MathHelper.TwoPi / 5;
                                        for (int I = 0; I < 5; I++)
                                        {
                                            SingularityOrbs = 5;
                                            if (StellaMultiplayer.IsHost)
                                            {
                                                Vector2 position = NPC.Center + radius * (I * rot).ToRotationVector2();
                                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)(position.X), (int)(position.Y),
                                                    ModContent.NPCType<SingularityOrb>(), NPC.whoAmI, NPC.whoAmI, I * rot, radius);
                                            }
                                        }
                                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb_Explode"), NPC.position);
                                        Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                                        for (int i = 0; i < 14; i++)
                                        {
                                            Dust.NewDustPerfect(base.NPC.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
                                        }
                                        for (int i = 0; i < 40; i++)
                                        {
                                            Dust.NewDustPerfect(base.NPC.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
                                        }
                                        for (int i = 0; i < 40; i++)
                                        {
                                            Dust.NewDustPerfect(base.NPC.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
                                        }
                                        for (int i = 0; i < 20; i++)
                                        {
                                            Dust.NewDustPerfect(base.NPC.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
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
                                if (SingularityOrbs == 0)
                                {
                                    if (StellaMultiplayer.IsHost)
                                    {
                                         
                                        Attack = Main.rand.Next(1, MaxAttac);
                                        if (MaxAttac == 7)
                                        {
                                            //Hi Azza
                                            //You're probably looking at this and are confused at what is happening
                                            //This code is just checking that the previous attack is one of the deathrays
                                            //And constantly randomizing until it is not
                                            //It runs the normal code otherwise
                                            //:)
                                            if (PrevAttac == 5 || PrevAttac == 6)
                                            {
                                                while (Attack == 5 || Attack == 6)
                                                {
                                                    Attack = Main.rand.Next(1, MaxAttac);
                                                }

                                                NPC.ai[1] = Attack;
                                            }
                                            else
                                            {
                                                if (Attack == PrevAttac)
                                                {
                                                    Attack = Main.rand.Next(1, MaxAttac);
                                                }
                                                else
                                                {
                                                    NPC.ai[1] = Attack;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Attack == PrevAttac)
                                            {
                                                Attack = Main.rand.Next(1, MaxAttac);
                                            }
                                            else
                                            {
                                                NPC.ai[1] = Attack;
                                            }
                                        }
                        
                                        NPC.netUpdate = true;
                                    }
                                }
                                else
                                {
                                    if (StellaMultiplayer.IsHost)
                                    {
                                        Attack = Main.rand.Next(1, 3);
                                        NPC.netUpdate = true;
                                    }
                                    if (Attack == PrevAttac)
                                    {
                                        if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                                        {
                                            Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
                                        }
                                        if (StellaMultiplayer.IsHost)
                                        {
                                            Attack = Main.rand.Next(1, 3);
                                            NPC.netUpdate = true;
                                        }
                                    }
                                    else
                                    {
                                        NPC.ai[1] = Attack;
                                    }
                                }
                                NPC.scale = 1;          
                            }
                        }
                        break;
                    case 1:
                        NPC.ai[0]++;
                        if (SingularityOrbs == 0)
                        {
                            CasuallyApproachChild();
                            NPC.velocity *= 0.90f;
                        }
                        else
                        {
                            NPC.velocity *= 0.90f;
                        }
                        if(SparkCount < SparkCountMax)
                        {
                            if (NPC.ai[0] == 20)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, 
                                        ModContent.NPCType<SingularitySpark>());
                                }
                            }
                            if (NPC.ai[0] == 70 || NPC.ai[0] == 75 || NPC.ai[0] == 80 || NPC.ai[0] == 85 || NPC.ai[0] == 90)
                            {
                                //
                                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                                Vector2 Backlash = Vector2.Normalize(NPC.Center - Main.player[NPC.target].Center ) * 8.5f;
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                                if (SingularityOrbs == 0)
                                {
                                    NPC.velocity += Backlash / 2;
                                }

                                if (StellaMultiplayer.IsHost)
                                {
                                    float offsetX = Main.rand.Next(-5, 5);
                                    float offsetY = Main.rand.Next(-5, 5);
                                    int damage = 32;
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, (direction.X * 1.5f) + offsetX, (direction.Y * 1.5f) + offsetY, 
                                        ModContent.ProjectileType<SingularitySparkProj>(), damage, 1, Owner: Main.myPlayer);
                                }
                                
                            }
                            if (NPC.ai[0] == 110)
                            {
                                SparkCount += 1;
                                NPC.ai[0] = 0;
                            }
                        }
                        else
                        {
                            if (SingularityOrbs == 0)
                            {
                                if (NPC.ai[0] >= 10)
                                {
                                    SparkCount = 0;
                                    PrevAttac = 1;
                                    NPC.ai[1] = 0;
                                    NPC.ai[0] = 0;
                                }
                                CasuallyApproachChild();
                            }
                            else
                            {
                                if (NPC.ai[0] >= 50)
                                {
                                    SparkCount = 0;
                                    PrevAttac = 1;
                                    NPC.ai[1] = 0;
                                    NPC.ai[0] = 0;
                                }
                                NPC.velocity *= 0.90f;
                            }
                        }
                        break;
                    case 2:
                        NPC.ai[0]++;
    
                        if (SingularityOrbs == 0)
                        {
                            if (NPC.ai[0] >= 150)
                            {
                                PrevAttac = 2;
                                NPC.ai[1] = 0;
                                NPC.ai[0] = 0;
                            }
                            CasuallyApproachChild();
                        }
                        else
                        {
                            if (NPC.ai[0] >= 190)
                            {
                                PrevAttac = 2;
                                NPC.ai[1] = 0;
                                NPC.ai[0] = 0;
                            }
                            NPC.velocity *= 0.90f;
                        }
                        if (NPC.ai[0] == 70 || NPC.ai[0] == 100 || NPC.ai[0] == 130)
                        {
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);

                            if (StellaMultiplayer.IsHost)
                            {

                                float offsetX = Main.rand.Next(-1, 1);
                                float offsetY = Main.rand.Next(-1, 1);
                                int damage = 30;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, (direction.X * 1.5f) + offsetX, (direction.Y * 1.5f) + offsetY, 
                                    ModContent.ProjectileType<VoidFlame>(), damage, 1, Owner: Main.myPlayer);
                            }
                        }

                        break;
                    case 3:
                        NPC.ai[0]++;
                        if (NPC.ai[0] <= 5)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge"));
                        }
                        if (NPC.ai[0] >= 275)
                        {
                            PrevAttac = 3;
                            NPC.ai[1] = 0;
                            NPC.ai[0] = 0;
                        }
                        NPC.velocity *= 0.90f;
                        if (NPC.ai[0] == 100)
                        {
                            LastBacklash = Vector2.Normalize(NPC.Center - Main.player[NPC.target].Center) * 8.5f;
                            LastDirection = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                            if (StellaMultiplayer.IsHost)
                            {
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SingularitySparkBig>());
                            }
                        }
                        if (NPC.ai[0] == 170 || NPC.ai[0] == 175 || NPC.ai[0] == 180 || NPC.ai[0] == 185 || NPC.ai[0] == 190  || NPC.ai[0] == 195 || NPC.ai[0] == 200 || NPC.ai[0] == 205 || NPC.ai[0] == 210 || NPC.ai[0] == 215 || NPC.ai[0] == 220 || NPC.ai[0] == 225 || NPC.ai[0] == 230 || NPC.ai[0] == 235 || NPC.ai[0] == 240 || NPC.ai[0] == 245 || NPC.ai[0] == 250 || NPC.ai[0] == 255)
                        {

                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                            SoundEngine.PlaySound(SoundID.Item91, NPC.position);

                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 2212f, 13f);
                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero,
                                    ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
                            }
       
                            if (SingularityOrbs == 0)
                            {
                                NPC.velocity += LastBacklash / 5;
                            }

                            if (StellaMultiplayer.IsHost)
                            {
                                float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                                float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                                int damage = 30;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, (LastDirection.X * 1.5f) + offsetX, (LastDirection.Y * 1.5f) + offsetY, 
                                    ModContent.ProjectileType<PulsarBeam>(), damage, 1, Owner: Main.myPlayer);
                            }
                          
                        }

                        break;
                    case 4:
                        NPC.ai[0]++;
                        CasuallyApproachChild();
                        NPC.netUpdate = true;
                        base.NPC.velocity.Y *= 0.95f;
                        if (NPC.ai[0] == 50 || NPC.ai[0] == 150)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot"), NPC.position);
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                            SoundEngine.PlaySound(SoundID.Item8, NPC.position);

                            if (StellaMultiplayer.IsHost)
                            {
                                float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                                float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                                int damage = 30;

                                Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero,
                                    ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, 
                                    ModContent.ProjectileType<SoulBlast>(), damage, 1, Owner: Main.myPlayer);
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
                                int damage = 30;
                                Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero, 
                                    ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
                                Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero, 
                                    ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
                                for (int j = -1; j <= 1; j++)
                                {
                                    Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(j * 0.5f) * 6f,
                                        ModContent.ProjectileType<SoulBlast>(), damage, 0f, Owner: Main.myPlayer);
                                }
                            }

                        }

                        if (NPC.ai[0] == 220)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                        }
                        break;
                    case 5:
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
                                    NPC.position = SingularityStart;
                                }
                            }
                            else
                            {
                                NPC.scale += 0.015f;
                                if (NPC.scale >= 1)
                                {
                                    Lazer = true;
                                    NPC.scale = 1;
                                    NPC.damage = 999;
                                    TP = false;
                                    NPC.ai[0] = 0;
                                }
                            }
                        }
                        else
                        {
                            if (NPC.ai[0] <= 5)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge2"));
                            }
                            if (NPC.ai[0] >= 440)
                            {
                                Lazer = false;
                                PrevAttac = 5;
                                NPC.ai[1] = 0;
                                NPC.ai[0] = 0;
                            }
                            NPC.velocity *= 0.90f;
                            if (NPC.ai[0] == 50)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    LazerType = Main.rand.Next(0, 2);
                                    if (LazerType == 0)
                                    {
                                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SingularityLazer>());
                                    }
                                    else
                                    {
                                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SingularityLazer2>());
                                    }
                                    NPC.netUpdate = true;
                                }

                                LastBacklash = Vector2.Normalize(NPC.Center - Main.player[NPC.target].Center) * 8.5f;
                                LastDirection = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
         
                            }
                            if (NPC.ai[0] == 170)
                            {
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2212f, 62f);
                                if (StellaMultiplayer.IsHost)
                                {
                                    Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero,
                                        ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
                                    Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero,
                                        ModContent.ProjectileType<RuneSpawnEffect2>(), 0, 0f, Owner: Main.myPlayer);
                                }
     
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_LAZER"));
                                float radius = 10;
                                float rot = MathHelper.TwoPi / 2;
                                for (int I = 0; I < 1; I++)
                                {
                                    if (StellaMultiplayer.IsHost)
                                    {
                                        Vector2 position = NPC.Center + radius * (I * rot).ToRotationVector2();
                                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)(position.X), (int)(position.Y),
                                            ModContent.NPCType<LazerOrb>(), NPC.whoAmI, NPC.whoAmI, I * rot, radius);
                                    }


                                }

                            }
                        }




                        break;
                    case 6:
                        NPC.ai[0]++;
                        if (!Lazer)
                        {

                            if (NPC.ai[0] == 1)
                            {
                                NPC.damage = 0;
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPOut"), NPC.position);
                            }
                            if (!TP)
                            {
                                NPC.velocity.Y += 0.01f;
                                NPC.scale -= 0.015f;
                                if (NPC.scale <= 0)
                                {
                                    NPC.scale = 0;
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPIn"), NPC.position);
                                    TP = true;
                                    NPC.velocity.Y = 0;
                                    NPC.position = SingularityStart;
                                }
                            }
                            else
                            {
                                NPC.scale += 0.015f;
                                if (NPC.scale >= 1)
                                {
                                    Lazer = true;
                                    NPC.scale = 1;
                                    NPC.damage = 999;
                                    TP = false;
                                    NPC.ai[0] = 0;
                                }
                            }
                        }
                        else
                        {
                            if (NPC.ai[0] <= 5)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge2"));
                            }
                            if (NPC.ai[0] >= 440)
                            {
                                Lazer = false;
                                PrevAttac = 6;
                                NPC.ai[1] = 0;
                                NPC.ai[0] = 0;
                            }
                            NPC.velocity *= 0.90f;
                            if (NPC.ai[0] == 50)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    LazerType = Main.rand.Next(0, 2);
                                    if (LazerType == 0)
                                    {

                                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SingularityLazer>());
                                    }
                                    else
                                    {
                                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SingularityLazer2>());
                                    }
                                    NPC.netUpdate = true;
                                }
    
                                LastBacklash = Vector2.Normalize(NPC.Center - Main.player[NPC.target].Center) * 8.5f;
                                LastDirection = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                           
                            }
                            if (NPC.ai[0] == 270)
                            {
                                if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                                {
                                    Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
                                }
                            }
                            if (NPC.ai[0] == 170)
                            {
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2212f, 62f);
                                if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                                {
                                    Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(NPC.Center);

                                }

                                if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                                {
                                    float progress = (180f - bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                                    Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
                                }

                                if (StellaMultiplayer.IsHost)
                                {
                                    Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero,
                                        ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f, Owner: Main.myPlayer);
                                    Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero,
                                        ModContent.ProjectileType<RuneSpawnEffect2>(), 0, 0f, Owner: Main.myPlayer);
                                }

                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_LAZER"));
                                float radius = 10;
                                float rot = MathHelper.TwoPi / 2;
                                for (int I = 0; I < 1; I++)
                                {
                                    Vector2 position = NPC.Center + radius * (I * rot).ToRotationVector2();

                                    if (StellaMultiplayer.IsHost)
                                    {
                                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)(position.X), (int)(position.Y), 
                                            ModContent.NPCType<LazerOrb>(), NPC.whoAmI, NPC.whoAmI, I * rot, radius);
                                    }
                                }

                            }
                        }
                        break;
                }
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedSOMBoss, -1);

        }

        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, target, 0.5f), NPC.Center.Distance(target) * Speed), 0.1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Lighting.AddLight(NPC.Center, Color.LightBlue.ToVector3() * 1.25f * Main.essScale);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 frameSize = NPC.frame.Size();
            Vector2 drawPos = NPC.Center - screenPos + frameSize / 2;// + offset;

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

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, NPC.frame, new Color(93, 203, 243, 50), NPC.rotation, frameSize, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 16f).RotatedBy(radians) * time, NPC.frame, new Color(59, 72, 168, 77), NPC.rotation, frameSize, NPC.scale, Effects, 0);
            }

            DrawIncresionDiskBottom(spriteBatch, screenPos, lightColor);
            return true;
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
            float drawScale = NPC.scale * 0.5f;
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
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
            Color color1 = Color.LightBlue * num107 * .8f;
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
            SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);
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
