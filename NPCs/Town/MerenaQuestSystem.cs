using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.NPCs.Town
{
    internal class MerenaQuestSystem : ModSystem
    {
        public static bool KillVerliaCompleted;
        public static bool ExploreMorrowedVillageCompleted;
        public static bool Give100DustBagsCompleted;
        public static bool MakeMagicPaperCompleted;
        public static bool MakeTomeOfInfiniteSorceryCompleted;

        public static Condition ShopConditionKillVerlia => new Condition("KillVerlia", () => KillVerliaCompleted);
        public static Condition ShopConditionExploreMorrowedVillage => new Condition("ExploreMorrowedVillage", () => ExploreMorrowedVillageCompleted);
        public static Condition ShopConditionGive100DustBags => new Condition("Give100DustBags", () => Give100DustBagsCompleted);
        public static Condition ShopConditionMakeMagicPaper => new Condition("MakeMagicPaper", () => MakeMagicPaperCompleted);
        public static Condition ShopConditionTome => new Condition("Tome", () => MakeTomeOfInfiniteSorceryCompleted);

        public override void ClearWorld()
        {
            KillVerliaCompleted = false;
            ExploreMorrowedVillageCompleted = false;
            Give100DustBagsCompleted = false;
            MakeMagicPaperCompleted = false;
            MakeTomeOfInfiniteSorceryCompleted = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["KillVerliaCompleted"] = KillVerliaCompleted;
            tag["ExploreMorrowedVillageCompleted"] = ExploreMorrowedVillageCompleted;
            tag["Give100DustBagsCompleted"] = Give100DustBagsCompleted;
            tag["MakeMagicPaperCompleted"] = MakeMagicPaperCompleted;
            tag["MakeTomeOfInfiniteSorceryCompleted"] = MakeTomeOfInfiniteSorceryCompleted;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            KillVerliaCompleted = tag.GetBool("KillVerliaCompleted");
            ExploreMorrowedVillageCompleted = tag.GetBool("ExploreMorrowedVillageCompleted");
            Give100DustBagsCompleted = tag.GetBool("Give100DustBagsCompleted");
            MakeMagicPaperCompleted = tag.GetBool("MakeMagicPaperCompleted");
            MakeTomeOfInfiniteSorceryCompleted = tag.GetBool("MakeTomeOfInfiniteSorceryCompleted");
        }
    }
}
