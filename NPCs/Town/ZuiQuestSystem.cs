using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.NPCs.Town
{
    internal class ZuiQuestSystem : ModSystem
    {
        /*
        public static bool ThreeQuestsCompleted;
        public static bool SixQuestsCompleted;
        public static bool TenQuestsCompleted;
        public static bool TwentyQuestsCompleted;
        public static bool ThirtyQuestsCompleted;
        */
        public static int QuestsCompleted;



        public static Condition ShopCondition3 => new Condition("ThreeQuests", () => QuestsCompleted >= 3);
        public static Condition ShopCondition6 => new Condition("SixQuests", () => QuestsCompleted >= 6);
        public static Condition ShopCondition10 => new Condition("TenQuests", () => QuestsCompleted >= 10);
        public static Condition ShopCondition20 => new Condition("TwentyQuests", () => QuestsCompleted >= 20);
        public static Condition ShopCondition30 => new Condition("ThirtyQuests", () => QuestsCompleted >= 30);


        public override void ClearWorld()
        {
            QuestsCompleted = 0;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            /*
            tag["ThreeQuests"] = ThreeQuestsCompleted;
            tag["SixQuests"] = SixQuestsCompleted;
            tag["TenQuests"] = TenQuestsCompleted;
            tag["TwentyQuests"] = TwentyQuestsCompleted;
            tag["ThirtyQuests"] = ThirtyQuestsCompleted;
            */
            tag["QuestsCompleted"] = QuestsCompleted;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            /*
            ThreeQuestsCompleted = tag.GetBool("ThreeQuests");
            SixQuestsCompleted = tag.GetBool("SixQuests");
            TenQuestsCompleted = tag.GetBool("TenQuests");
            TwentyQuestsCompleted = tag.GetBool("TwentyQuests");
            ThirtyQuestsCompleted = tag.GetBool("ThirtyQuests");
            */
            QuestsCompleted = tag.GetInt("QuestsCompleted");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(QuestsCompleted);
        }

        public override void NetReceive(BinaryReader reader)
        {
            int questsCompleted = reader.ReadInt32();
            if (QuestsCompleted <= questsCompleted)
                QuestsCompleted = questsCompleted;
        }

        public static void CompleteQuest()
        {
            QuestsCompleted++;
            SendCompleteQuestPacket();
        }

        public static void SendCompleteQuestPacket()
        {
            if(Main.netMode != NetmodeID.SinglePlayer)
            {
                Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.CompleteZuiQuest).Send(-1);
            }
        }
    }
}
