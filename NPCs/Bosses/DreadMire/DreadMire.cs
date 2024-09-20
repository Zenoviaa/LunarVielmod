using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Stellamod.Utilis;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.DreadMire
{
    [AutoloadBossHead]
    public class DreadMire : ModNPC
    {
        private bool _invincible;
        private bool _spawnHeart;
        private int _heartKillCount;

        bool p3;
        bool p2;
        Vector2 Light;
        public bool Dir = false;
        public int AtackNum = 5;
        public int PrevAtack;
        public float DR = 0;
        int Att = 0;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.lifeMax = 2200;
            NPC.defense = 9;
            NPC.damage = 1;
            NPC.value = 65f;
            NPC.knockBackResist = 0f;
            NPC.width = 30;
            NPC.height = 40;
            NPC.scale = 0.9f;
            NPC.lavaImmune = false;
            NPC.noTileCollide = false;
            NPC.alpha = 0;
            NPC.boss = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = false;
            NPC.npcSlots = 10f;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath23;
            NPC.aiStyle = 0;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        private void Disappear()
        {
            Player obj = Main.player[NPC.target];
            NPC.velocity.Y += 0.5f;
            if (Vector2.Distance(obj.position, NPC.position) > 1000f)
            {
                NPC.active = false;
            }
        }

        public int previousAttack;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                var entitySource = NPC.GetSource_FromThis();
                Player player = Main.player[NPC.target];
                player.GetModPlayer<MyPlayer>().heartDead = 0;
                player.GetModPlayer<MyPlayer>().heart = false;
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Firework_Red, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Firework_Red, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
            }
            else
            {
                for (int k = 0; k < 7; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Firework_Red, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Firework_Red, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                }
            }
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedDreadBoss, -1);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Att);
            writer.Write(AtackNum);
            writer.Write(_invincible);
            writer.Write(_heartKillCount);
            writer.Write(_spawnHeart);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Att = reader.ReadInt32();
            AtackNum = reader.ReadInt32();
            _invincible = reader.ReadBoolean();
            _heartKillCount = reader.ReadInt32();
            _spawnHeart = reader.ReadBoolean();
        }

        private bool TryDespawn()
        {
            if (!NPC.HasPlayerTarget)
            {
                NPC.TargetClosest(false);
                Player player1 = Main.player[NPC.target];

                if (!NPC.HasPlayerTarget || NPC.Distance(player1.Center) > 3000f)
                {
                    NPC.velocity.Y -= 1f;
                    if (NPC.timeLeft > 30)
                        NPC.timeLeft = 30;
                    return true;
                }
            }

            return false;
        }

        private void AI_HeartPhaseSwitch()
        {
            Player player = Main.player[NPC.target];
            int dreadMiresHeartType = ModContent.NPCType<DreadMiresHeart>();
            if (p2 && !_spawnHeart && _heartKillCount == 0)
            {
                var entitySource = NPC.GetSource_FromThis();
                if (StellaMultiplayer.IsHost)
                {
                    _spawnHeart = true;
                    _invincible = true;
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, dreadMiresHeartType);
                }

                NPC.ai[0] = 0;
                NPC.ai[2] = 5;
                NPC.netUpdate = true;
            } 
            else if (p2 && _spawnHeart && _heartKillCount == 0 && !NPC.AnyNPCs(dreadMiresHeartType) && NPC.ai[2] == 5)
            {
                AtackNum = 6;
                if (StellaMultiplayer.IsHost)
                {
                    _spawnHeart = false;
                    _invincible = false;
                    _heartKillCount++;
                }

                NPC.ai[2] = 1;
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
                NPC.ai[3] = 1;  
                NPC.netUpdate = true;
            }

            if (p3 && !_spawnHeart && _heartKillCount == 1)
            {
                var entitySource = NPC.GetSource_FromThis();
                if (StellaMultiplayer.IsHost)
                {
                    _spawnHeart = true;
                    _invincible = true;
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, dreadMiresHeartType);
                }

                NPC.ai[0] = 0;
                NPC.ai[2] = 5;
          
                NPC.netUpdate = true;
            }
            else if (p3 && _spawnHeart && _heartKillCount == 1 && !NPC.AnyNPCs(dreadMiresHeartType) && NPC.ai[2] == 5)
            {
                AtackNum = 7; 
                if (StellaMultiplayer.IsHost)
                {
                    _spawnHeart = false;
                    _invincible = false;
                    _heartKillCount++;
                }

                NPC.ai[2] = 1;
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
                NPC.ai[3] = 1;
                NPC.netUpdate = true;
            }

            if (_spawnHeart)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/DreadHeart");
                NPC.Center = player.Center - Vector2.UnitY * (-200);
                NPC.alpha = 255;
            }
            else
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/DreadmireV2");
            }
        }

        public override void AI()
        {
            NPC.damage = 0;
            Player player = Main.player[NPC.target];
            bool expertMode = Main.expertMode;

            if (TryDespawn())
                return;

            Player playerT = Main.player[NPC.target];
            int distance = (int)(NPC.Center - playerT.Center).Length();
            if (distance > 3000f || playerT.dead)
            {
                NPC.ai[2] = 2;
                Disappear();
            }


            NPC.rotation = NPC.velocity.X * 0.05f;
            Lighting.AddLight((int)(NPC.Center.X / 16), (int)(NPC.Center.Y / 16), 0.46f, 0.32f, .1f);
            if (NPC.ai[2] == 0)
            {
                NPC.Center = player.Center - Vector2.UnitY * (-200);
                NPC.ai[0]++;
                if (NPC.ai[0] >= 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Spawn1"), NPC.position);
                    NPC.velocity.Y -= 10f;
                    NPC.alpha = 255;
                    NPC.ai[0] = 0;
                    NPC.ai[2] = 1;
                    if (StellaMultiplayer.IsHost)
                    {
                        _invincible = true;
                        NPC.netUpdate = true;
                    }
                }
            }

            p3 = NPC.life < NPC.lifeMax * 0.3f;
            if (p3)
            {
                p2 = false;
            }
            else
            {
                p2 = NPC.life < NPC.lifeMax * 0.6f;
            }

            NPC.dontTakeDamage = _invincible;
            NPC.dontCountMe = _invincible;
            AI_HeartPhaseSwitch();
            Vector2 targetPos;
            if (NPC.ai[2] == 1)
            {
                switch (NPC.ai[1])
                {
                    case 0:
                        // default attack, just moves above player, waits  seconds then does a random attack
                        targetPos = player.Center;
                        if (NPC.ai[3] == 0)
                        {
                            if (NPC.ai[0] > 100 && NPC.ai[0] < 200)
                            {
                                Main.bloodMoon = true;
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 512f, 32f);
                                Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.Firework_Red);
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
                            }
                            if (NPC.ai[0] > 100 && NPC.ai[0] < 150)
                            {
                                NPC.alpha -= 1;
                            }
                            if (NPC.ai[0] > 150 && NPC.ai[0] < 200)
                            {
                                NPC.alpha += 1;
                            }
                            NPC.velocity.Y *= 0.96f;
                            if (NPC.alpha >= 0)
                            {

                                NPC.alpha -= 10;
                            }
                            if (NPC.ai[0] == 230)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    _invincible = false;
                                    NPC.netUpdate = true;
                                }
                   
                                NPC.ai[0] = 0;
                                NPC.ai[1] = 0;
                                NPC.ai[3] = 1;
                            }
                            if (NPC.ai[0] == 100)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Spawn2"), NPC.position);
                                var entitySource = NPC.GetSource_FromThis();
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagram>());
                                }
                            }

                            if (NPC.alpha <= 0)
                            {
                                NPC.ai[0]++;
                            }
                        }
                        else
                        {
                            if (NPC.ai[0] == 0)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    _invincible = true;
                                    NPC.netUpdate = true;
                                }
                                if (NPC.alpha <= 255)
                                {

                                    NPC.alpha += 10;
                                }
                                if (NPC.alpha >= 255)
                                {
                                    NPC.ai[0] = 1;
                                }

                            }
                            if (NPC.alpha <= 20)
                            {

                            }
                            if (NPC.ai[0] >= 1)
                            {
                                NPC.ai[0]++;
                            }

                            if (NPC.ai[0] == 40)
                            {
                                NPC.velocity.Y = 0;
                                if (Main.dayTime)
                                {
                                    NPC.active = false;
                                }
                                NPC.velocity.X = 0;
                                if (StellaMultiplayer.IsHost)
                                {
                                    Att = Main.rand.Next(1, AtackNum);
                                    if (Att == PrevAtack)
                                    {
                                        NPC.ai[0] = 39;
                                    }
                                    NPC.netUpdate = true;
                                }
                            }
                            if (NPC.ai[0] == 50)
                            {  
                                NPC.velocity.Y -= 5f;
                                if (StellaMultiplayer.IsHost)
                                {
                                    Vector2 TPPos;
                                    distance = (int)Math.Sqrt((NPC.Center.X - player.Center.X) * (NPC.Center.X - player.Center.X) + (NPC.Center.Y - player.Center.Y) * (NPC.Center.Y - player.Center.Y));
                                    if (distance <= 100)
                                    {
                                        NPC.ai[0] = 49;
                                    }
                                    if (Att == 1 || Att == 3 || Att == 4 || Att == 5)
                                    {
                                        TPPos.Y = Main.rand.NextFloat(player.Center.Y + 300, player.Center.Y - 300 + 1);
                                        TPPos.X = Main.rand.NextFloat(player.Center.X + 300, player.Center.X - 300 + 1);
                                        NPC.position = TPPos;
                                    }
                                    if (Att == 2)
                                    {
                                        TPPos.Y = player.Center.Y + 50;
                                        TPPos.X = Main.rand.NextFloat(player.Center.X + 600, player.Center.X - 600 + 1);
                                        NPC.position = TPPos;
                                    }
                                    NPC.netUpdate = true;
                                }
  
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_TP_In"), NPC.position);
                          
                            }
                            if (p3)
                            {
                                if (NPC.ai[0] >= 80)
                                {
                                    NPC.ai[3] = 0;
                                    NPC.ai[0] = 0;
                                    NPC.ai[1] = Att;
                                }
                            }
                            else
                            {
                                if (NPC.ai[0] >= 100)
                                {
            
                                    NPC.ai[3] = 0;
                                    NPC.ai[0] = 0;
                                    NPC.ai[1] = Att;
                                }

                            }
                            if (NPC.ai[0] >= 50)
                            {
                                NPC.velocity.Y *= 0.96f;
                                if (NPC.alpha >= 0)
                                {
                                    if (StellaMultiplayer.IsHost)
                                    {
                                        _invincible = false;
                                        NPC.netUpdate = true;
                                    }
                                    NPC.alpha -= 10;
                                }
                            }
                        }
                        break;
                    case 1:
                        if (NPC.alpha >= 0)
                        {
                            NPC.alpha -= 2;
                        }
                        NPC.ai[0]++;
                        targetPos = player.Center;
                        if (NPC.ai[0] == 10 || NPC.ai[0] == 30 || NPC.ai[0] == 50)
                        {
                            NPC.alpha = 20;
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn1"), NPC.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn2"), NPC.position);
                            }
                            Vector2 Vdirection = Vector2.Normalize(NPC.Center - Main.player[NPC.target].Center) * 8.5f;
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                            Movement(targetPos, (Vdirection.X * 170), (Vdirection.Y * 170), 0.05f);
                            if (StellaMultiplayer.IsHost)
                            {
                                int amountOfProjectiles = Main.rand.Next(1, 3);
                                for (int i = 0; i < amountOfProjectiles; ++i)
                                {
                                    float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                                    float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                                    int damage = Main.expertMode ? 20 : 24;
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY,
                                        ModContent.ProjectileType<DreadFire>(), damage, 1, Owner: Main.myPlayer);
                                }
                            }
                        }
                        if (NPC.ai[0] >= 70)
                        {
                            var entitySource = NPC.GetSource_FromThis();
                            if (StellaMultiplayer.IsHost)
                            {
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagramSmall>());
                            }
                      
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_TP_Out"), NPC.position);
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[3] = 1;
                            PrevAtack = 1;
                        }
                        NPC.velocity.X *= 0.94f;
                        NPC.velocity.Y *= 0.94f;
                        break;
                    case 2:
                        NPC.velocity.Y *= 0.95f;
                        if (NPC.position.X >= player.position.X)
                        {
                            Dir = true;
                        }
                        else
                        {
                            Dir = false;
                        }
                        NPC.ai[0]++;
                        if (NPC.ai[0] == 10)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Fire1"), NPC.position);
                            if (p3)
                            {
    
                                float radius = 130;
                                float rot = MathHelper.TwoPi / 5;
                                for (int I = 0; I < 5; I++)
                                {
                                    if (StellaMultiplayer.IsHost)
                                    {
                                        Vector2 position = NPC.Center + radius * (I * rot).ToRotationVector2();
                                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)(position.X), (int)(position.Y),
                                            ModContent.NPCType<DreadFireCircle>(), NPC.whoAmI, NPC.whoAmI, I * rot, radius);
                                    }
                                }
                            }
                            else if (p2)
                            {
                                float radius = 80;
                                float rot = MathHelper.TwoPi / 4;
                                for (int I = 0; I < 4; I++)
                                {
                                    if (StellaMultiplayer.IsHost)
                                    {
                                        Vector2 position = NPC.Center + radius * (I * rot).ToRotationVector2();
                                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)(position.X), (int)(position.Y), ModContent.NPCType<DreadFireCircle>(), NPC.whoAmI, NPC.whoAmI, I * rot, radius);
                                    }
                                }
                            }
                            else
                            {
                                float radius = 50;
                                float rot = MathHelper.TwoPi / 3;
                                for (int I = 0; I < 3; I++)
                                {
                                   
                                    if (StellaMultiplayer.IsHost)
                                    {
                                        Vector2 position = NPC.Center + radius * (I * rot).ToRotationVector2();
                                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)(position.X), (int)(position.Y), ModContent.NPCType<DreadFireCircle>(), NPC.whoAmI, NPC.whoAmI, I * rot, radius);
                                    }
                                   
                                }
                            }
                            NPC.netUpdate = false;
                        }
                        if (NPC.ai[0] > 10 && NPC.ai[0] < 30)
                        {
                            DR -= 0.5f;
                        }
                        if (NPC.ai[0] > 240 && NPC.ai[0] < 260)
                        {
                            DR += 0.5f;
                        }
                        if (NPC.ai[0] >= 10)
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.025f), NPC.Center.Distance(player.Center) * 0.15f), 0.008f);
                        }
                        else
                        {
                            NPC.velocity.Y *= 0.94f;
                        }
                        if (NPC.ai[0] >= 260)
                        {
                            var entitySource = NPC.GetSource_FromThis();
                            if (StellaMultiplayer.IsHost)
                            {
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagramSmall>());
                            }
           
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_TP_Out"), NPC.position);
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[3] = 1;
                            PrevAtack = 2;
      
                        }


                        break;
                    case 3:

                        if (NPC.ai[0] > 10 && NPC.ai[0] < 40)
                        {
                            DR -= 0.5f;
                        }
                        if (NPC.ai[0] > 40 && NPC.ai[0] < 80)
                        {
                            DR += 0.5f;
                        }
                        NPC.velocity.Y *= 0.94f;
                        NPC.ai[0]++;
                        if (p3)
                        {
                            if (NPC.ai[0] == 30 || NPC.ai[0] == 45 || NPC.ai[0] == 60 || NPC.ai[0] == 75)
                            {
                                int Sound = Main.rand.Next(1, 3);
                                if (Sound == 1)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Pentagram_Skull1"), NPC.position);
                                }
                                else
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Pentagram_Skull1"), NPC.position);
                                }

                                if (StellaMultiplayer.IsHost)
                                {
                                    Vector2 RockPos;
                                    RockPos.Y = Main.rand.NextFloat(NPC.Center.Y + 100, NPC.Center.Y - 100 + 1);
                                    RockPos.X = Main.rand.NextFloat(NPC.Center.X + 100, NPC.Center.X - 100 + 1);
                                    var entitySource = NPC.GetSource_FromThis();
                                    NPC.NewNPC(entitySource, (int)RockPos.X, (int)RockPos.Y, ModContent.NPCType<DreadSurvent>());
                                }
                            }
                        }
                        else
                        {
                            if (NPC.ai[0] == 30 || NPC.ai[0] == 50 || NPC.ai[0] == 70)
                            {
                                int Sound = Main.rand.Next(1, 3);
                                if (Sound == 1)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Pentagram_Skull1"), NPC.position);
                                }
                                else
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Pentagram_Skull1"), NPC.position);
                                }
                                Vector2 RockPos;
                                RockPos.Y = Main.rand.NextFloat(NPC.Center.Y + 100, NPC.Center.Y - 100 + 1);
                                RockPos.X = Main.rand.NextFloat(NPC.Center.X + 100, NPC.Center.X - 100 + 1);
                                if (StellaMultiplayer.IsHost)
                                {
                                    var entitySource = NPC.GetSource_FromThis();
                                    NPC.NewNPC(entitySource, (int)RockPos.X, (int)RockPos.Y, ModContent.NPCType<DreadSurvent>());
                                }
                            }
                        }

                        if (NPC.ai[0] == 10)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Pentagram"), NPC.position);
                            if (StellaMultiplayer.IsHost)
                            {
                                var entitySource = NPC.GetSource_FromThis();
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagramMid>());
                            }
                        }
                        if (NPC.ai[0] >= 100)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                var entitySource = NPC.GetSource_FromThis();
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagramSmall>());
                            }

                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_TP_Out"), NPC.position);
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[3] = 1;
                            PrevAtack = 3;
                        }

                        break;
                    case 4:
                        targetPos = player.Center;
                        NPC.ai[0]++;
                        if (NPC.ai[0] == 25)
                        {
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 124f);
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__Dash"), NPC.position);
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                            NPC.alpha = 255;
                            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                            SoundEngine.PlaySound(SoundID.Zombie53, NPC.position);
                            if (StellaMultiplayer.IsHost)
                            {
                                int damage = Main.expertMode ? 16 : 24;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 2, direction.Y * 2,
                                    ModContent.ProjectileType<DreadMireDash>(), damage, 1, Owner: Main.myPlayer);
                            }
                        }
                        if (NPC.ai[0] == 10)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__PreDash"), NPC.position);
                            Vector2 Vdirection = Vector2.Normalize(NPC.Center - Main.player[NPC.target].Center) * 8.5f;
                            Movement(targetPos, (Vdirection.X * 170), (Vdirection.Y * 170), 0.05f);

                        }
                        if (NPC.ai[0] >= 55)
                        {

                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[3] = 1;
                            PrevAtack = 4;
                        }
                        break;
                    case 5:

                        NPC.velocity.Y *= 0.94f;
                        NPC.ai[0]++;

                        if (NPC.ai[0] > 10 && NPC.ai[0] < 30)
                        {
                            NPC.alpha -= 2;
                        }
                        if (NPC.ai[0] > 30 && NPC.ai[0] < 60)
                        {
                            NPC.alpha += 2;
                        }

                        if (NPC.ai[0] == 10)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                var entitySource = NPC.GetSource_FromThis();
                                Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0),
                                    Mod.Find<ModProjectile>("DreadMireMagic").Type, 19, 0, Owner: Main.myPlayer);
                            }

                            Light.Y = player.Center.Y;
                            Light.X = player.Center.X;
                        }
                        if (NPC.ai[0] == 20)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                var entitySource = NPC.GetSource_FromThis();
                                NPC.NewNPC(entitySource, (int)Light.X - 10, (int)Light.Y, ModContent.NPCType<DreadMireZapwarn>());
                            }
                        }

                        if (NPC.ai[0] == 30)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                var entitySource = NPC.GetSource_FromThis();
                                NPC.NewNPC(entitySource, (int)Light.X - 200, (int)Light.Y, ModContent.NPCType<DreadMireZapwarn>());
                                NPC.NewNPC(entitySource, (int)Light.X + 200, (int)Light.Y, ModContent.NPCType<DreadMireZapwarn>());
                            }
                        }
                        if (NPC.ai[0] == 40)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                var entitySource = NPC.GetSource_FromThis();
                                NPC.NewNPC(entitySource, (int)Light.X - 400, (int)Light.Y, ModContent.NPCType<DreadMireZapwarn>());
                                NPC.NewNPC(entitySource, (int)Light.X + 400, (int)Light.Y, ModContent.NPCType<DreadMireZapwarn>());
                            }
                        }
                        if (NPC.ai[0] == 50)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                var entitySource = NPC.GetSource_FromThis();
                                NPC.NewNPC(entitySource, (int)Light.X - 600, (int)Light.Y, ModContent.NPCType<DreadMireZapwarn>());
                                NPC.NewNPC(entitySource, (int)Light.X + 600, (int)Light.Y, ModContent.NPCType<DreadMireZapwarn>());
                            }
                        }
                        if (NPC.ai[0] == 60)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                var entitySource = NPC.GetSource_FromThis();
                                NPC.NewNPC(entitySource, (int)Light.X - 800, (int)Light.Y, ModContent.NPCType<DreadMireZapwarn>());
                                NPC.NewNPC(entitySource, (int)Light.X + 800, (int)Light.Y, ModContent.NPCType<DreadMireZapwarn>());
                            }

                        }
                        if (NPC.ai[0] >= 61)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_TP_Out"), NPC.position);
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[3] = 1;
                            PrevAtack = 4;
                        }
                        break;
                    case 6:

                        NPC.velocity.Y *= 0.94f;
                        NPC.ai[0]++;

                        if (NPC.ai[0] == 30)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                var entitySource = NPC.GetSource_FromThis();
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMireZapwarn>());
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMireZapwarn>());
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMireZapwarn>());
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMireZapwarn>());
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMireZapwarn>());
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMireZapwarn>());
                            }
                        }

                        if (NPC.ai[0] == 130)
                        {
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 124f);
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__FinalBeam"));
                            if (StellaMultiplayer.IsHost)
                            {
                                Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y - 900, 0, 10,
                                    ModContent.ProjectileType<FinalBeam>(), 500, 0f, owner: Main.myPlayer, 0, NPC.whoAmI);
                            }

                        }
                        if (NPC.ai[0] >= 200 && NPC.ai[0] <= 600)
                        {
                            if (NPC.ai[0] % 20 == 0)
                            {
                                int Sound = Main.rand.Next(1, 3);
                                if (Sound == 1)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ShadeHand"));
                                }
                                else
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ShadeHand2"));
                                }

                                var entitySource = NPC.GetSource_FromThis();
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 2048f, 64f);
                                if (StellaMultiplayer.IsHost)
                                {
                                    int OffSet = Main.rand.Next(-240, 240 + 1);
                                    Vector2 NukePos;
                                    NukePos.X = NPC.Center.X;
                                    NukePos.Y = player.Center.Y + OffSet;
                                    Projectile.NewProjectile(entitySource, NukePos, new Vector2(15, 0),
                                        Mod.Find<ModProjectile>("RedSkull").Type, 19, 0, Owner: Main.myPlayer);
                                    Projectile.NewProjectile(entitySource, NukePos.X, NukePos.Y, 0, 0,
                                        ModContent.ProjectileType<DreadSpawnEffect>(), 40, 1, Owner: Main.myPlayer);

                                    OffSet = Main.rand.Next(-240, 240 + 1);
                                    NukePos.X = NPC.Center.X;
                                    NukePos.Y = player.Center.Y + OffSet;
                                    Projectile.NewProjectile(entitySource, NukePos, new Vector2(-15, 0),
                                        Mod.Find<ModProjectile>("RedSkull").Type, 19, 0, Owner: Main.myPlayer);
                                    Projectile.NewProjectile(entitySource, NukePos.X, NukePos.Y, 0, 0,
                                        ModContent.ProjectileType<DreadSpawnEffect>(), 40, 1, Owner: Main.myPlayer);
                                }

                            }
                        }
                        if (NPC.ai[0] >= 650)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_TP_Out"), NPC.position);
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[3] = 1;
                            PrevAtack = 7;
                        }
                        break;

                }
            }

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
                new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "One of the three powerful cosmic deities that has been tainted by devilish intent")),
            });
        }

        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            NPC.velocity = Vector2.Lerp(base.NPC.velocity, VectorHelper.MovemontVelocity(base.NPC.Center, Vector2.Lerp(base.NPC.Center, target, 0.5f), base.NPC.Center.Distance(target) * Speed), 0.1f);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DreadmireBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.DreadBossRel>()));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DreadFoil>(), minimumDropped: 40, maximumDropped: 65));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Aneuriliac>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TheRedSkull>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Pericarditis>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Myocardia>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DreadBroochA>()));
            npcLoot.Add(notExpertRule);
        }

        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (GlowTexture is not null)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                Vector2 halfSize = new Vector2(GlowTexture.Width / 2, GlowTexture.Height / Main.npcFrameCount[NPC.type] / 2);
                spriteBatch.Draw(
                    GlowTexture,
                    new Vector2(NPC.position.X - screenPos.X + NPC.width / 2 - GlowTexture.Width * NPC.scale / 2f + halfSize.X * NPC.scale, NPC.position.Y - screenPos.Y + NPC.height - GlowTexture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + halfSize.Y * NPC.scale + Main.NPCAddHeight(NPC) + NPC.gfxOffY),
                    NPC.frame,
                    Color.White,
                    NPC.rotation,
                    halfSize,
                    NPC.scale,
                    spriteEffects,
                0);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;



            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X - 15, NPC.height - NPC.frame.Height + 8);
            Vector2 DrawPos = NPC.position - screenPos + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, DR / 2).RotatedBy(radians) * time, NPC.frame, new Color(99, 39, 51, 0), 0, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, DR).RotatedBy(radians) * time, NPC.frame, new Color(255, 8, 55, 0), 0, frameOrigin, NPC.scale, Effects, 0);
            }

            Lighting.AddLight(NPC.Center, Color.DarkRed.ToVector3() * 2.25f * Main.essScale);
                 int spOff = NPC.alpha / 6;
            for (float j = -(float)Math.PI; j <= (float)Math.PI / 3f; j += (float)Math.PI / 3f)
            {
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[base.NPC.type], base.NPC.Center + new Vector2(0f, -2f) + new Vector2(4f + NPC.alpha * 0.25f + spOff, 0f).RotatedBy(base.NPC.rotation + j) - Main.screenPosition, base.NPC.frame, Color.FromNonPremultiplied(255 + spOff * 2, 255 + spOff * 2, 255 + spOff * 2, 100 - base.NPC.alpha), base.NPC.rotation, base.NPC.frame.Size() / 2f, base.NPC.scale, Effects, 0f);
            }
            spriteBatch.Draw((Texture2D)TextureAssets.Npc[base.NPC.type], base.NPC.Center - Main.screenPosition, base.NPC.frame, base.NPC.GetAlpha(lightColor), base.NPC.rotation, base.NPC.frame.Size() / 2f, base.NPC.scale, Effects, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(255, 8, 55), new Color(99, 39, 51), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }
       


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }

}