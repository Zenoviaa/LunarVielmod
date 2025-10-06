using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Core.QuestSystem
{
    internal class QuestLoader : ModSystem
    {
        internal static readonly IDictionary<int, Quest> quests = new Dictionary<int, Quest>();
        public static int QuestCount { get; private set; }

        internal static void RegisterQuest(Quest quest)
        {
            int id = QuestCount++;
            quest.Type = id;
            quests.TryAdd(id, quest);
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            quests.Clear();
            QuestCount = 0;
        }

        internal static Quest GetQuest(int type)
        {
            quests.TryGetValue(type, out var quest);
            return quest;
        }

        public static int QuestType<T>() where T : Quest => ModContent.GetInstance<T>()?.Type ?? 0;
        public static Quest GetInstance<T>() where T : Quest
        {
            int questType = QuestType<T>();
            Quest quest = GetQuest(questType);
            return quest;
        }
    }
}
