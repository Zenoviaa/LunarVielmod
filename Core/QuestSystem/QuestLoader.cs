using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Core.QuestSystem
{

    public class QuestLoader : ModSystem
    {
        public static readonly IDictionary<int, Quest> quests = new Dictionary<int, Quest>();
        public static int QuestCount { get; private set; }

        public static void RegisterQuest(Quest quest)
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

        public static Quest GetQuest(int type)
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
