using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.NPCs.Town
{
    internal class ZuiQuestSystem : ModSystem
    {
        public static bool ThreeQuestsCompleted;
        public static bool SixQuestsCompleted;
        public static bool TenQuestsCompleted;
        public static bool TwentyQuestsCompleted;
        public static bool ThirtyQuestsCompleted;

        public static int QuestsCompleted;



        public static Condition ShopCondition3 => new Condition("ThreeQuests", () => ThreeQuestsCompleted);
        public static Condition ShopCondition6 => new Condition("SixQuests", () => SixQuestsCompleted);
        public static Condition ShopCondition10 => new Condition("TenQuests", () => TenQuestsCompleted);
        public static Condition ShopCondition20 => new Condition("TwentyQuests", () => TwentyQuestsCompleted);
        public static Condition ShopCondition30 => new Condition("ThirtyQuests", () => ThirtyQuestsCompleted);


        public override void ClearWorld()
        {
            ThreeQuestsCompleted = false;
            SixQuestsCompleted = false;
            TenQuestsCompleted = false;
            TwentyQuestsCompleted = false;
            ThirtyQuestsCompleted = false;
            QuestsCompleted = 0;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["ThreeQuests"] = ThreeQuestsCompleted;
            tag["SixQuests"] = SixQuestsCompleted;
            tag["TenQuests"] = TenQuestsCompleted;
            tag["TwentyQuests"] = TwentyQuestsCompleted;
            tag["ThirtyQuests"] = ThirtyQuestsCompleted;

            tag["QuestsCompleted"] = QuestsCompleted;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            ThreeQuestsCompleted = tag.GetBool("ThreeQuests");
            SixQuestsCompleted = tag.GetBool("SixQuests");
            TenQuestsCompleted = tag.GetBool("TenQuests");
            TwentyQuestsCompleted = tag.GetBool("TwentyQuests");
            ThirtyQuestsCompleted = tag.GetBool("ThirtyQuests");
            QuestsCompleted = tag.GetInt("QuestsCompleted");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(ThreeQuestsCompleted);
            writer.Write(SixQuestsCompleted);
            writer.Write(TenQuestsCompleted);
            writer.Write(TwentyQuestsCompleted);
            writer.Write(ThirtyQuestsCompleted);
            writer.Write(QuestsCompleted);
        }

        public override void NetReceive(BinaryReader reader)
        {
            bool con1 = reader.ReadBoolean();
            bool con2 = reader.ReadBoolean();
            bool con3 = reader.ReadBoolean();
            bool con4 = reader.ReadBoolean();
            bool con5 = reader.ReadBoolean();
            int questsCompleted = reader.ReadInt32();

            if (!ThreeQuestsCompleted)
                ThreeQuestsCompleted = con1;
            if (!SixQuestsCompleted)
                SixQuestsCompleted = con2;
            if(!TenQuestsCompleted)
                TenQuestsCompleted = con3;
            if(!TwentyQuestsCompleted)
                TwentyQuestsCompleted = con4;
            if(!ThirtyQuestsCompleted)
                ThirtyQuestsCompleted = con5;
            if (QuestsCompleted <= questsCompleted)
                QuestsCompleted = questsCompleted;
        }
    }
}
