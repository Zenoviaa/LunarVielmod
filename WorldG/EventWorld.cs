using Stellamod.Buffs;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.ID;
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

        private void TrySpawnGintzeArmy()
        {
            Player player = Main.LocalPlayer;
            //------------------------------------------------------------------------------
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

                player.AddBuff(ModContent.BuffType<GintzeSeen>(), 2);
            }

            if (!Main.dayTime && TryForGintze)
            {
                TryForGintze = false;
                GintzeDayReset = false;
                NetMessage.SendData(MessageID.WorldData);
            }

            if (!TryForGintze && Main.dayTime && player.townNPCs >= 3 && DownedBossSystem.downedStoneGolemBoss && !Main.hardMode && !GintzeDayReset && !GintzingBoss && !DownedBossSystem.downedGintzlBoss)
            {
                Gintzing = true;
                Main.NewText("The Gintze army is approaching...", 34, 121, 100);
                TryForGintze = true;
                NetMessage.SendData(MessageID.WorldData);
            }


            if (!TryForGintze && Main.dayTime && player.townNPCs >= 3 && player.ZoneOverworldHeight && player.ZoneForest && !Main.hardMode && !GintzeDayReset && !GintzingBoss && DownedBossSystem.downedGintzlBoss)
            {
                if (Main.rand.NextBool(40))
                {
                    Gintzing = true;
                    Main.NewText("The Gintze army is returning for another round...", 34, 121, 100);
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
                    Main.NewText("Aurorean Stars are falling!", 234, 96, 114);
                }
                NetMessage.SendData(MessageID.WorldData);
            }
            else if (Main.dayTime && Aurorean)
            {
                Aurorean = false;
                Main.NewText("The Aurorean starfall has ended", 234, 96, 114);
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
                Main.NewText("The Moon has turned red for tonight!", 234, 16, 50);
                Main.bloodMoon = true;
                NetMessage.SendData(MessageID.WorldData);
            }
        }

        public override void PostUpdateWorld()
        {
            TrySpawnGintzeArmy();
            TrySpawnAuroreanStarfall();
            TryForceBloodmoon();
        }


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



















