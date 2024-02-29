using Microsoft.Xna.Framework;
using ReLogic.OS.Windows;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Projectiles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.WorldG
{


    public class EventWorld : ModSystem
    {
        //Gintzing--------------------------
        public static bool Gintzing;
        public static bool TryForGintze;
        public static bool GintzeDayReset;
        public static int GintzeKills;

        public static bool GintzingBoss;

        //SoulStorm--------------------------
        public static bool SoulStorm;

        //AuroreanStars--------------------------
        public static bool AuroreanSpawn;
        public static bool Aurorean;

        //-----------------------------------
        public static bool HasHadBloodMoon;
        public static void GintzeWin()
        {
            if (GintzingBoss)
            {
                GintzeDayReset = true;
                Main.NewText("The Gintze army has been defeated!", 34, 121, 100);
                GintzeKills = 0;
                GintzingBoss = false;
            }
        }


        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Gintzing);
            writer.Write(TryForGintze);
            writer.Write(GintzeDayReset);
            writer.Write(GintzeKills);
            writer.Write(GintzingBoss);
            writer.Write(AuroreanSpawn);
            writer.Write(Aurorean);
            writer.Write(HasHadBloodMoon);
        }

        public override void NetReceive(BinaryReader reader)
        {
            Gintzing = reader.ReadBoolean();
            TryForGintze = reader.ReadBoolean();
            GintzeDayReset = reader.ReadBoolean();
            GintzeKills = reader.ReadInt32();
            GintzingBoss = reader.ReadBoolean();
            AuroreanSpawn = reader.ReadBoolean();
            Aurorean = reader.ReadBoolean();
            HasHadBloodMoon = reader.ReadBoolean();
        }

        private int CountTownNPCs()
        {
            int count = 0;
            for(int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].townNPC)
                    count++;
            }
            return count;
        }

        private void TrySpawnGintzeArmy()
        {
            int townNpcCount = CountTownNPCs();
            if (Gintzing)
            {
                if (Main.expertMode)
                {
                    if (GintzeKills >= 80)
                    {
                        GintzingBoss = true;
                        Gintzing = false;
                        GintzeKills = 0;
                        NetMessage.SendData(MessageID.WorldData);
                    }
                }
                else if (Main.masterMode)
                {
                    if (GintzeKills >= 100)
                    {
                        GintzingBoss = true;
                        Gintzing = false;
                        GintzeKills = 0;
                        NetMessage.SendData(MessageID.WorldData);
                    }
                }
                else
                {
                    if (GintzeKills >= 65)
                    {
                        GintzingBoss = true;
                        Gintzing = false;
                        GintzeKills = 0;
                        NetMessage.SendData(MessageID.WorldData);
                    }
                }

                for(int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (!player.active)
                        continue;
                    player.AddBuff(ModContent.BuffType<GintzeSeen>(), 2);
                } 
            }

            if (!Main.dayTime)
            {
                TryForGintze = false;
                GintzeDayReset = false;
                NetMessage.SendData(MessageID.WorldData);
            }

            if (!TryForGintze && Main.dayTime && townNpcCount >= 3 && DownedBossSystem.downedStoneGolemBoss 
                && !Main.hardMode && !GintzeDayReset && !GintzingBoss && !DownedBossSystem.downedGintzlBoss)
            {
                Gintzing = true;
                string message = "The Gintze army is approaching...";
                if(Main.netMode == NetmodeID.Server)
                {
                    NetworkText txt = NetworkText.FromLiteral(message);
                    ChatHelper.BroadcastChatMessage(txt, new Color(34, 121, 100));
                }
                else
                {
                    Main.NewText(message, 34, 121, 100);
                }
      
                TryForGintze = true;
                NetMessage.SendData(MessageID.WorldData);
            }

            if (!TryForGintze && Main.dayTime && townNpcCount >= 3 && !Main.hardMode && !GintzeDayReset && !GintzingBoss && DownedBossSystem.downedGintzlBoss)
            {
                if (Main.rand.NextBool(40))
                {
                    Gintzing = true;
                    string message = "The Gintze army is returning for another round...";
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetworkText txt = NetworkText.FromLiteral(message);
                        ChatHelper.BroadcastChatMessage(txt, new Color(34, 121, 100));
                    }
                    else
                    {
                        Main.NewText(message, 34, 121, 100);
                    }
                }
                TryForGintze = true;
                NetMessage.SendData(MessageID.WorldData);
            }   
        }

        private void TrySpawnAuroreanStarfall()
        {
            //If not eye of cthulu is killed then don't run aurorean starfall code
            if (!NPC.downedBoss1)
                return;
            //AuroreanStars--------------------------
            if (!Main.dayTime && !Aurorean && !AuroreanSpawn)
            {
                AuroreanSpawn = true;
                if (Main.rand.NextBool(5))
                {
                    Aurorean = true;
                    if(Main.netMode == NetmodeID.Server)
                    {
                        NetworkText auroeanStarfall = NetworkText.FromLiteral("Aurorean Stars are falling!");
                        ChatHelper.BroadcastChatMessage(auroeanStarfall, new Color(234, 96, 114));
                    }
                    else
                    {
                        Main.NewText("Aurorean Stars are falling!", 234, 96, 114);
                    }
                }
                NetMessage.SendData(MessageID.WorldData);
            }
            else if (Main.dayTime && Aurorean)
            {
                Aurorean = false;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetworkText auroeanStarfallEnded = NetworkText.FromLiteral("The Aurorean starfall has ended");
                    ChatHelper.BroadcastChatMessage(auroeanStarfallEnded, new Color(234, 96, 114));
                }
                else
                {
                    Main.NewText("The Aurorean starfall has ended", 234, 96, 114);
                }
                NetMessage.SendData(MessageID.WorldData);
            }
            else if (Main.dayTime && AuroreanSpawn)
            {
                AuroreanSpawn = false;
                NetMessage.SendData(MessageID.WorldData);
            }
        }

        private void TryForceBloodmoon()
        {
            if (!Main.dayTime && !Aurorean && !HasHadBloodMoon && DownedBossSystem.downedDaedusBoss)
            {
                HasHadBloodMoon = true;
                string message = "The Moon has turned red for tonight! The Govheil Castle calls you...";
                if (Main.netMode == NetmodeID.Server)
                {
                    NetworkText txt = NetworkText.FromLiteral(message);
                    ChatHelper.BroadcastChatMessage(txt, new Color(234, 16, 50));
                }
                else
                {
                    Main.NewText(message, 234, 16, 50);
                }

                Main.bloodMoon = true;
                NetMessage.SendData(MessageID.WorldData);
            }
        }

        public static bool ChaosD;
        public static bool ChaosP;
        public static bool ChaosT;


        public override void PostUpdateWorld()
        {
            TrySpawnGintzeArmy();
            TrySpawnAuroreanStarfall();
            TryForceBloodmoon();
            SpawnAuroreanStars();


            /*
            if (!ChaosD)
            {
                string message = "Chaos has plaged the ocean...";
                if (NPC.downedMechBoss1)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/CHAOS"));
                    NetworkText txt = NetworkText.FromLiteral(message);
                    ChatHelper.BroadcastChatMessage(txt, new Color(34, 121, 100));
                    ChaosD = true;
                }
            }
            if (!ChaosT)
            {
                string message = "Chaos has plaged the dungeon...";
                if (NPC.downedMechBoss2)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/CHAOS"));
                    NetworkText txt = NetworkText.FromLiteral(message);
                    ChatHelper.BroadcastChatMessage(txt, new Color(34, 121, 100));
                    ChaosT = true;
                }
            }
            if (!ChaosP)
            {
                string message = "Chaos has plaged the hell...";
                if (NPC.downedMechBoss3)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/CHAOS"));
                    NetworkText txt = NetworkText.FromLiteral(message);
                    ChatHelper.BroadcastChatMessage(txt, new Color(34, 121, 100));
                    ChaosP = true;
                }
            }
            */
        }


        private void SpawnAuroreanStars()
        {
            if (!Aurorean)
                return;

            for(int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;
                if (player.ZoneOverworldHeight || player.ZoneSkyHeight)
                {
                    if (Main.rand.NextBool(90))
                    {
                        int offsetX = Main.rand.Next(-1000, 1000) * 2;
                        int offsetY = Main.rand.Next(-1000, 1000) - 1700;
                        int damage = Main.expertMode ? 0 : 0;
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center.X + offsetX, player.Center.Y + offsetY, 0f, 10f, 
                            ModContent.ProjectileType<AuroreanStar>(), damage, 1, Main.myPlayer, 0, 0);
                    }

                    //Don't spawn the npc if it already exists
                    if (Main.rand.NextBool(4500) && !NPCHelper.IsBossAlive() && Main.hardMode)
                    {
                        int offsetX = Main.rand.Next(-10, 10) * 2;
                        int offsetY = Main.rand.Next(-500, 500) - 1700;
                        int damage = Main.expertMode ? 0 : 0;
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center.X + offsetX, player.Center.Y + offsetY, 0f, 10f, 
                            ModContent.ProjectileType<AuroreanStarbomber>(), damage, 1, Main.myPlayer, 0, 0);
                    }
                }
            }
        }

        private static void ResetEventWorld() { }

        public override void ClearWorld()
        {
            HasHadBloodMoon = false;
            Aurorean = false;
            Gintzing = false;
            GintzingBoss = false;
            TryForGintze = false;
            GintzeKills = 0;
            GintzeDayReset = false;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            HasHadBloodMoon = tag.GetBool("HasHadBloodmoon");
            Aurorean = tag.GetBool("Aurorean");
            Gintzing = tag.GetBool("Gintzing");
            GintzingBoss = tag.GetBool("GintzingBoss");
            TryForGintze = tag.GetBool("TryForGintze");
            GintzeKills = tag.GetInt("GintzeKills");
            GintzeDayReset = tag.GetBool("GintzeDayReset");
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["HasHadBloodmoon"] = HasHadBloodMoon;
            tag["Aurorean"] = Aurorean;
            tag["Gintzing"] = Gintzing;
            tag["GintzingBoss"] = GintzingBoss;
            tag["GintzeKills"] = GintzeKills;
            tag["GintzeDayReset"] = GintzeDayReset;
            tag["TryForGintze"] = TryForGintze;
        }
    }
}



















