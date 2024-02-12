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
            writer.Write(new BitsByte
            {
                [0] = ThreeQuestsCompleted,
                [1] = SixQuestsCompleted,
                [2] = TenQuestsCompleted,
                [3] = TwentyQuestsCompleted,
                [4] = ThirtyQuestsCompleted
            });

            writer.Write(QuestsCompleted);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ThreeQuestsCompleted = flags[0];
            SixQuestsCompleted = flags[1];
            TenQuestsCompleted = flags[2];
            TwentyQuestsCompleted = flags[3];
            ThirtyQuestsCompleted = flags[4];
            QuestsCompleted = reader.ReadInt32();
        }
    }
}
