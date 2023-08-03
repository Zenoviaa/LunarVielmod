
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;
using System.Collections.Generic;
using Steamworks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

using Stellamod.NPCs.Bosses.Jack;
using System.Reflection.Metadata;
using Stellamod.Utilis;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Terraria.GameContent.ItemDropRules;
using Stellamod.Items.Weapons.Ranged;

namespace Stellamod.NPCs.Bosses.singularityFragment
{
    [AutoloadBossHead]
    public class SingularityFragment : ModNPC
    {
        private const int TELEPORT_DISTANCE = 400;
        public bool TP = true;
        public bool DarkHoldPos = true;
        public bool Flying = false;
        public bool HP50 = false;
        public bool Spawned = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Binding Abyss");
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.Next(1) == 0)
                target.AddBuff(Mod.Find<ModBuff>("AbyssalFlame").Type, 200);
        }
        public override void SetDefaults()
        {
            NPC.scale = 0;
            NPC.width = 100;
            NPC.height = 60;
            NPC.damage = 999;
            NPC.defense = 23;
            NPC.lifeMax = 6300;
            NPC.scale = 0.9f;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/VoidDead1") with { PitchVariance = 0.1f };
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/SingularityFragment");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/VoidHit") with { PitchVariance = 0.1f };
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EventHorizon>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VoidBlaster>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TomeOfTheSingularity>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SingularityBag>()));

        }

        public override void AI()
        {

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
                NPC.position.X = player.Center.X;
                NPC.position.Y = player.Center.Y - 200;
                NPC.scale = 0;
                Spawned = false;
                NPC.ai[2] = 1;
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
                                    NPC.velocity.Y -= 0.05f;
                                    NPC.scale += 0.010f;
                                    NPC.ai[0]++;
                                    if (NPC.scale >= 1)
                                    {
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
                                        TP = false;
                                        NPC.ai[0] = 0;
                                        NPC.ai[1] = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
           
             
                            }
                            else
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    NPC.scale = 1;
                                }
       
                                if (NPC.life > NPC.lifeMax * 0.5f)
                                {
                                    NPC.ai[1] = Main.rand.Next(1, 5);
                                    NPC.scale = 1;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    if (Main.rand.Next(2) == 0)
                                    {

                                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y + 80, ModContent.NPCType<Voidling>());
                                    }
                                    if (NPC.life > NPC.lifeMax * 0.2f)
                                    {
                                        NPC.ai[1] = Main.rand.Next(1, 7);
                                        NPC.scale = 1;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        NPC.defense = 30;
                                        NPC.ai[1] = Main.rand.Next(1, 8);
                                        NPC.scale = 1;
                                        NPC.netUpdate = true;
                                    }

                                }
                                NPC.scale = 1;
                                NPC.netUpdate = true;
                            }
       



                        }
                        break;
                    case 1:
                        NPC.ai[0]++;
                        if (NPC.life > NPC.lifeMax * 0.5f)
                        {
                            base.NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.006f);
                        }
                        else
                        {
                            if (NPC.life > NPC.lifeMax * 0.2f)
                            {
                                base.NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.008f);
                            }
                            else
                            {
                                NPC.defense = 32;
                                base.NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.011f);
                            }

                        }

                        base.NPC.velocity.Y *= 0.95f;
                        if (NPC.ai[0] == 50 || NPC.ai[0] == 150)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot"), NPC.position);
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                            float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                            float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                            int damage = Main.expertMode ? 32 : 45;

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
                            int damage = expertMode ? 32 : 45;
                            Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                            Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                            for (int j = -1; j <= 1; j++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Normalize(Main.player[base.NPC.target].Center - base.NPC.Center).RotatedBy((float)j * 0.5f) * 6f, ModContent.ProjectileType<SoulBlast>(), 30, 0f);
                            }
                        }
                        if (NPC.ai[0] == 220)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    case 2:
                        NPC.ai[0]++;
                        base.NPC.velocity *= 0.97f;

                        if (NPC.ai[0] >= 70)
                        {

                            if (Main.rand.Next(2) == 0)
                            {
                                if (Main.netMode != NetmodeID.Server)
                                {
                                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 22f);
                                    Vector2 vector = base.NPC.Center + (VectorHelper.Right * 125f).RotatedByRandom(180.0);
                                    Dust dust = Dust.NewDustDirect(vector, NPC.width, NPC.height, DustID.Firework_Blue, 0, 0, 0, Color.White, 2);
                                    dust.velocity *= -1f;
                                    dust.scale *= .8f;
                                    dust.noGravity = true;
                                    Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                                    vector2_1.Normalize();
                                    Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                                    dust.velocity = vector2_2;
                                    vector2_2.Normalize();
                                    Vector2 vector2_3 = vector2_2 * 34f;
                                    dust.position = NPC.Center - vector2_3;
                                    NPC.netUpdate = true;
                                }
                                if (Main.netMode != NetmodeID.Server)
                                {
                                    Vector2 vector = base.NPC.Center + (VectorHelper.Right * 125f).RotatedByRandom(180.0);
                                    Dust dust = Dust.NewDustDirect(vector, NPC.width, NPC.height, DustID.Firework_Blue, 0, 0, 0, Color.White, 2);
                                    dust.velocity *= -1f;
                                    dust.scale *= .8f;
                                    dust.noGravity = true;
                                    Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                                    vector2_1.Normalize();
                                    Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                                    dust.velocity = vector2_2;
                                    vector2_2.Normalize();
                                    Vector2 vector2_3 = vector2_2 * 34f;
                                    dust.position = vector - vector2_3;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                        if (NPC.life > NPC.lifeMax * 0.5f)
                        {
                            if (NPC.ai[0] == 200)
                            {
                                Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb_Explode"), NPC.position);
                                int offsetRandom = Main.rand.Next(0, 50);
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 92f);
                                for (int i = 0; i < 20; i++)
                                {
                                    Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
                                }
                                float spread = 45f * 0.0174f;
                                double startAngle = Math.Atan2(1, 0) - spread / 2;
                                double deltaAngle = spread / 8f;
                                double offsetAngle;

                                for (int i = 0; i < 4; i++)
                                {
                                    offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)(Math.Sin(offsetAngle) * 9f), (float)(Math.Cos(offsetAngle) * 9f), ModContent.ProjectileType<AbyssalChargeProjectile>(), 22, 0, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)(-Math.Sin(offsetAngle) * 9f), (float)(-Math.Cos(offsetAngle) * 9f), ModContent.ProjectileType<AbyssalChargeProjectile>(), 22, 0, Main.myPlayer);
                                    }


                                }
                            }
                            if (NPC.ai[0] == 70)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge"), NPC.position);

                            }
                            if (NPC.ai[0] >= 220)
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[1] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (NPC.ai[0] == 200 || NPC.ai[0] == 230)
                            {
                                Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb_Explode"), NPC.position);
                                int offsetRandom = Main.rand.Next(0, 50);
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 92f);
                                for (int i = 0; i < 20; i++)
                                {
                                    Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
                                }
                                float spread = 45f * 0.0174f;
                                double startAngle = Math.Atan2(1, 0) - spread / 2;
                                double deltaAngle = spread / 8f;
                                double offsetAngle;

                                for (int i = 0; i < 4; i++)
                                {
                                    offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)(Math.Sin(offsetAngle) * 9f), (float)(Math.Cos(offsetAngle) * 9f), ModContent.ProjectileType<AbyssalChargeProjectile>(), 22, 0, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)(-Math.Sin(offsetAngle) * 9f), (float)(-Math.Cos(offsetAngle) * 9f), ModContent.ProjectileType<AbyssalChargeProjectile>(), 22, 0, Main.myPlayer);
                                    }


                                }
                            }
                            if (NPC.ai[0] == 70)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge"), NPC.position);

                            }
                            if (NPC.ai[0] >= 250)
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[1] = 0;
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    case 3:
                        NPC.ai[0]++;
                        if (NPC.life > NPC.lifeMax * 0.5f)
                        {
                            base.NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.006f);
                        }
                        else
                        {
                            if (NPC.life > NPC.lifeMax * 0.2f)
                            {
                                base.NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.008f);
                            }
                            else
                            {
                                NPC.defense = 30;
                                base.NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.011f);
                            }

                        }
                        base.NPC.velocity.Y *= 0.95f;
                        if (NPC.ai[0] == 50 || NPC.ai[0] == 80 || NPC.ai[0] == 110)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_DrakinShot);
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                            float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                            float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                            int damage = Main.expertMode ? 32 : 45;

                            Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, ModContent.ProjectileType<VoidFlame>(), damage, 1, Main.myPlayer, 0, 0);
                        }

                        if (NPC.ai[0] >= 130)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    case 4:
                        NPC.ai[0]++;
                        if (NPC.life > NPC.lifeMax * 0.5f)
                        {
                            base.NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.006f);
                        }
                        else
                        {
                            if (NPC.life > NPC.lifeMax * 0.2f)
                            {
                                base.NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.008f);
                            }
                            else
                            {
                                NPC.defense = 30;
                                base.NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.011f);
                            }

                        }
                        base.NPC.velocity *= 0.7f;
                        if (NPC.ai[0] <= 100)
                        {

                            if (Main.rand.Next(2) == 0)
                            {
                                if (Main.netMode != NetmodeID.Server)
                                {
                                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 22f);
                                    Vector2 vector = base.NPC.Center + (VectorHelper.Right * 125f).RotatedByRandom(180.0);
                                    Dust dust = Dust.NewDustDirect(vector, NPC.width, NPC.height, DustID.Firework_Blue, 0, 0, 0, Color.White, 2);
                                    dust.velocity *= -1f;
                                    dust.scale *= .8f;
                                    dust.noGravity = true;
                                    Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                                    vector2_1.Normalize();
                                    Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                                    dust.velocity = vector2_2;
                                    vector2_2.Normalize();
                                    Vector2 vector2_3 = vector2_2 * 34f;
                                    dust.position = NPC.Center - vector2_3;
                                    NPC.netUpdate = true;
                                }
    
                            }
                        }
                        if (NPC.ai[0] == 100)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_DrakinShot);
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                            float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                            float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                            int damage = Main.expertMode ? 20 : 31;

                            Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, ModContent.ProjectileType<AbyssWave>(), damage, 1, Main.myPlayer, 0, 0);
                        }

                        if (NPC.ai[0] >= 100)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    case 5:

                        NPC.ai[0]++;
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
                                NPC.position.X = player.Center.X + (int)(TELEPORT_DISTANCE * angle.X);
                                NPC.position.Y = player.Center.Y + (int)(TELEPORT_DISTANCE * angle.Y);
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.velocity.Y -= 0.05f;
                            NPC.scale += 0.015f;
                            if (NPC.scale >= 1)
                            {
                                NPC.scale = 1;
                                NPC.damage = 999;
                                TP = false;
                                NPC.ai[0] = 0;
                                NPC.ai[1] = 0;
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    case 6:
                        NPC.ai[0]++;
                        base.NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.008f);
                        base.NPC.velocity.Y *= 0.95f;
                        if (NPC.ai[0] == 50 )
                        {
                            SoundEngine.PlaySound(SoundID.DD2_DrakinShot);
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                            float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                            float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                            int damage = Main.expertMode ? 20 : 31;

                            Projectile.NewProjectile(entitySource, base.NPC.Center, Vector2.Zero, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, ModContent.ProjectileType<AbyssalChargeProjectile2>(), damage, 1, Main.myPlayer, 0, 0);
                        }

                        if (NPC.ai[0] >= 60)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    case 7:
                        NPC.ai[0]++;
                        base.NPC.velocity *= 0.97f;

                        if (NPC.ai[0] >= 70)
                        {

                            if (Main.rand.Next(2) == 0)
                            {
                                if (Main.netMode != NetmodeID.Server)
                                {
                                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 22f);
                                    Vector2 vector = base.NPC.Center + (VectorHelper.Right * 125f).RotatedByRandom(180.0);
                                    Dust dust = Dust.NewDustDirect(vector, NPC.width, NPC.height, DustID.Firework_Blue, 0, 0, 0, Color.White, 2);
                                    dust.velocity *= -1f;
                                    dust.scale *= .8f;
                                    dust.noGravity = true;
                                    Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                                    vector2_1.Normalize();
                                    Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                                    dust.velocity = vector2_2;
                                    vector2_2.Normalize();
                                    Vector2 vector2_3 = vector2_2 * 34f;
                                    dust.position = NPC.Center - vector2_3;
                                    NPC.netUpdate = true;
                                }
                                if (Main.netMode != NetmodeID.Server)
                                {
                                    Vector2 vector = base.NPC.Center + (VectorHelper.Right * 125f).RotatedByRandom(180.0);
                                    Dust dust = Dust.NewDustDirect(vector, NPC.width, NPC.height, DustID.Firework_Blue, 0, 0, 0, Color.White, 2);
                                    dust.velocity *= -1f;
                                    dust.scale *= .8f;
                                    dust.noGravity = true;
                                    Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                                    vector2_1.Normalize();
                                    Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                                    dust.velocity = vector2_2;
                                    vector2_2.Normalize();
                                    Vector2 vector2_3 = vector2_2 * 34f;
                                    dust.position = vector - vector2_3;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                        if (NPC.ai[0] >= 70)
                        {
                            if(player.position.X >= NPC.position.X)
                            {
                                player.velocity.X -= 0.1f;
                                player.velocity.X -= 90.9f / distance;
                            }
                            else
                            {
                                player.velocity.X += 0.1f;
                                player.velocity.X += 90.9f / distance;
                            }
                        }
                        if (NPC.ai[0] == 70)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_LAZER"), NPC.position);
                            Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y - 900, 0, 10, ModContent.ProjectileType<VoidBeam>(), 100, 0f, -1, 0, NPC.whoAmI);

                        }
                        if (NPC.ai[0] == 1)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge"), NPC.position);

                        }
                        if (NPC.ai[0] >= 420)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;
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
        Vector2 targetPos;
        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Player player = Main.player[NPC.target];
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, target, 0.5f), NPC.Center.Distance(target) * Speed), 0.1f);
        }

        float alphaCounter = 0;
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
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X + 8, NPC.height - NPC.frame.Height + 0);
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
                Vector2 vector29 = NPC.Center + ((float)num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * (float)num103;
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