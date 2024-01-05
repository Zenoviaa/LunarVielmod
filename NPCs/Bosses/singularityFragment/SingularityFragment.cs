
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
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.singularityFragment
{
    [AutoloadBossHead]
    public class SingularityFragment : ModNPC
    {
        private const int TELEPORT_DISTANCE = 400;
        public bool PH2 = false;
        public bool Spawned = false;
        public bool TP = false;
        public bool Lazer = false;
        public int Timer = 0;
        public int PrevAttac = 0;
        public int MaxAttac = 0;
        public static int LazerType = 0;
        public static int SingularityOrbs = 0;
        public static Vector2 SingularityPos;
        public static Vector2 SingularityStart;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 30;
            // DisplayName.SetDefault("Binding Abyss");
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // This will always be true, so it's a nothing statement
            /*
            if (Main.rand.Next(1) == 0)
                target.AddBuff(Mod.Find<ModBuff>("AbyssalFlame").Type, 200);
            */

            //Use ModContent.BuffType<> instead of Mod.Find, it's faster and cleaner
            target.AddBuff(BuffType<AbyssalFlame>(), 200);
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
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 10f;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SingularityFragment");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/VoidHit") with { PitchVariance = 0.1f };
            NPC.BossBar = ModContent.GetInstance<VerliaBossBar>();
            NPC.aiStyle = 0;
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
            NPC.netUpdate = true;
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
        
        public override void AI()
        {
            Spawner++;
            /*
            Player players = Main.player[NPC.target];
            if (Spawner == 2)
            {
                int distanceY = Main.rand.Next(-150, -150);
                NPC.position.X = players.Center.X;
                NPC.position.Y = players.Center.Y + distanceY;
            }*/

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
            if (Spawned)
            {
                if (NPC.ai[1] >= 5)
                {
                    NPC.damage = 0;
                    NPC.dontTakeDamage = true;
                    NPC.dontCountMe = true;
                }
                else
                {
                    if (SingularityOrbs > 0)
                    {
                        SparkCountMax = 3;
                        NPC.dontTakeDamage = true;
                        NPC.dontCountMe = true;
                    }
                    else
                    {
                        SparkCountMax = 1;
                        NPC.dontTakeDamage = false;
                        NPC.dontCountMe = false;
                    }
                    NPC.damage = 9999;
                }
            }
            else
            {
                NPC.damage = 0;
                NPC.dontTakeDamage = true;
                NPC.dontCountMe = true;
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
                                    NPC.netUpdate = true;
                 
                                }
                                if (NPC.scale == 0)
                                {
                                    Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 8f);
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
                                            Vector2 position = NPC.Center + radius * (I * rot).ToRotationVector2();
                                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(position.X), (int)(position.Y), ModContent.NPCType<SingularityOrb>(), NPC.whoAmI, NPC.whoAmI, I * rot, radius);

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
                                        NPC.netUpdate = true;
                                    }
                                }
           
             
                            }
                            else
                            {
                                if (SingularityOrbs == 0)
                                {
                                    Attack = Main.rand.Next(1, MaxAttac);
                                    if (Attack == PrevAttac)
                                    {
                                        Attack = Main.rand.Next(1, MaxAttac);
                                    }
                                    else
                                    {
                                        NPC.ai[1] = Attack;
                                    }
                                }
                                else
                                {
                                    Attack = Main.rand.Next(1, 3);
                                    if (Attack == PrevAttac)
                                    {
                                        if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                                        {
                                            Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
                                        }
                                        Attack = Main.rand.Next(1, 3);
                                    }
                                    else
                                    {
                                        NPC.ai[1] = Attack;
                                    }
                                }
         
                                NPC.scale = 1;
           
                                NPC.scale = 1;
                                NPC.netUpdate = true;
                            }
       



                        }
                        break;
                    case 1:
                        NPC.ai[0]++;


                        if (SingularityOrbs == 0)
                        {
                            CasuallyApproachChild();
                            NPC.velocity *= 0.90f;
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.velocity *= 0.90f;
                        }
                        if(SparkCount < SparkCountMax)
                        {
                            if (NPC.ai[0] == 20)
                            {
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SingularitySpark>());
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
                 
                                float offsetX = Main.rand.Next(-5, 5);
                                float offsetY = Main.rand.Next(-5, 5);
                                int damage = Main.expertMode ? 6 : 10;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, (direction.X * 1.5f) + offsetX, (direction.Y * 1.5f) + offsetY, ModContent.ProjectileType<SingularitySparkProj>(), damage, 1, Main.myPlayer, 0, 0);
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
                                NPC.netUpdate = true;
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
                            NPC.netUpdate = true;
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

                    
                            float offsetX = Main.rand.Next(-1, 1);
                            float offsetY = Main.rand.Next(-1, 1);
                            int damage = Main.expertMode ? 11 : 15;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, (direction.X * 1.5f) + offsetX, (direction.Y * 1.5f) + offsetY, ModContent.ProjectileType<VoidFlame>(), damage, 1, Main.myPlayer, 0, 0);
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
                            NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SingularitySparkBig>());
                        }
                        if (NPC.ai[0] == 170 || NPC.ai[0] == 175 || NPC.ai[0] == 180 || NPC.ai[0] == 185 || NPC.ai[0] == 190  || NPC.ai[0] == 195 || NPC.ai[0] == 200 || NPC.ai[0] == 205 || NPC.ai[0] == 210 || NPC.ai[0] == 215 || NPC.ai[0] == 220 || NPC.ai[0] == 225 || NPC.ai[0] == 230 || NPC.ai[0] == 235 || NPC.ai[0] == 240 || NPC.ai[0] == 245 || NPC.ai[0] == 250 || NPC.ai[0] == 255)
                        {

                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                            SoundEngine.PlaySound(SoundID.Item91, NPC.position);

                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 2212f, 13f);
                            Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                            if (SingularityOrbs == 0)
                            {
                                NPC.velocity += LastBacklash / 5;
                            }
                            float offsetX = Main.rand.Next(-5, 5);
                            float offsetY = Main.rand.Next(-5, 5);
                            int damage = Main.expertMode ? 11 : 15;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, (LastDirection.X * 1.5f) + offsetX, (LastDirection.Y * 1.5f) + offsetY, ModContent.ProjectileType<PulsarBeam>(), damage, 1, Main.myPlayer, 0, 0);
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
                            float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                            float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                            int damage = Main.expertMode ? 10 : 28;

                            Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, ModContent.ProjectileType<SoulBlast>(), damage, 1, Main.myPlayer, 0, 0);
                        }
                        if (NPC.ai[0] == 100 || NPC.ai[0] == 200)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot1"), NPC.position);
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                            Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                            direction.Normalize();
                            int damage = Main.expertMode ? 10 : 28;
                            Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                            Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                            for (int j = -1; j <= 1; j++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Normalize(Main.player[base.NPC.target].Center - base.NPC.Center).RotatedBy(j * 0.5f) * 6f, ModContent.ProjectileType<SoulBlast>(), damage, 0f);
                            }
                        }
                        if (NPC.ai[0] == 220)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    case 5:
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
                                NPC.velocity.Y += 0.05f;
                                NPC.scale -= 0.015f;
                                if (NPC.scale <= 0)
                                {
                                    NPC.scale = 0;
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPIn"), NPC.position);
                                    TP = true;
                                    NPC.velocity.Y = 0;
                                    Vector2 angle = Vector2.UnitX.RotateRandom(Math.PI * 2);

                                    NPC.position = SingularityStart;
                                    NPC.netUpdate = true;
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

                                    NPC.netUpdate = true;
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
                                LazerType = Main.rand.Next(0, 2);
                                if (LazerType == 0)
                                {

                                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SingularityLazer>());
                                }
                                else
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SingularityLazer2>());
                                }
                                LastBacklash = Vector2.Normalize(NPC.Center - Main.player[NPC.target].Center) * 8.5f;
                                LastDirection = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
         
                            }
                            if (NPC.ai[0] == 170)
                            {
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2212f, 62f);
                                Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                                Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect2>(), 0, 0f);
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_LAZER"));
                                float radius = 10;
                                float rot = MathHelper.TwoPi / 2;
                                for (int I = 0; I < 1; I++)
                                {
                                    Vector2 position = NPC.Center + radius * (I * rot).ToRotationVector2();
                                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)(position.X), (int)(position.Y), ModContent.NPCType<LazerOrb>(), NPC.whoAmI, NPC.whoAmI, I * rot, radius);

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
                                    Vector2 angle = Vector2.UnitX.RotateRandom(Math.PI * 2);

                                    NPC.position = SingularityStart;
                                    NPC.netUpdate = true;
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

                                    NPC.netUpdate = true;
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
                                LazerType = Main.rand.Next(0, 2);
                                if (LazerType == 0)
                                {

                                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SingularityLazer>());
                                }
                                else
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SingularityLazer2>());
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
                                Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                                Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect2>(), 0, 0f);
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_LAZER"));
                                float radius = 10;
                                float rot = MathHelper.TwoPi / 2;
                                for (int I = 0; I < 1; I++)
                                {
                                    Vector2 position = NPC.Center + radius * (I * rot).ToRotationVector2();
                                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)(position.X), (int)(position.Y), ModContent.NPCType<LazerOrb>(), NPC.whoAmI, NPC.whoAmI, I * rot, radius);

                                }

                            }
                        }




                        break;
                }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int j = 0; j < 50; j++)
                {

                }
            }
            for (int k = 0; k < 7; k++)
            {

            }
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedSOMBoss, -1);

        }

        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Player player = Main.player[NPC.target];
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, target, 0.5f), NPC.Center.Distance(target) * Speed), 0.1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            Lighting.AddLight(NPC.Center, Color.LightBlue.ToVector3() * 1.25f * Main.essScale);
            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;



            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X + (NPC.scale * 4), NPC.height - NPC.frame.Height + 0);
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
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, NPC.frame, new Color(93, 203, 243, 50), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 16f).RotatedBy(radians) * time, NPC.frame, new Color(59, 72, 168, 77), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
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
            Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
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

            Player obj = Main.player[NPC.target];
            NPC.velocity.Y += 0.1f;
            NPC.scale -= 0.01f;
            if (NPC.scale <= 0)
            {
                NPC.active = false;
            }
            NPC.netUpdate = true;
        }
    }
}