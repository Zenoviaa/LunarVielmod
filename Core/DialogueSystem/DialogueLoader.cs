using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.DialogueSystem
{
    public class DialogueLoader : ModSystem
    {
        public static IDictionary<int, BaseDialogue> quests = new Dictionary<int, BaseDialogue>();
        public static int QuestCount { get; private set; }

        public static void RegisterQuest(BaseDialogue quest)
        {
            int id = QuestCount++;
            quest.Type = id;
            quests.TryAdd(id, quest);
        }
        public override void Load()
        {
            base.Load();
            quests = new Dictionary<int, BaseDialogue>();
            QuestCount = 0;
        }

        public override void Unload()
        {
            base.Unload();
            quests.Clear();
            quests = null;
            QuestCount = 0;
        }

        public static BaseDialogue GetDialogue(int type)
        {
            quests.TryGetValue(type, out var quest);
            return quest;
        }

        public static int QuestType<T>() where T : BaseDialogue => ModContent.GetInstance<T>()?.Type ?? 0;
        public static BaseDialogue GetInstance<T>() where T : BaseDialogue
        {
            int questType = QuestType<T>();
            BaseDialogue quest = GetDialogue(questType);
            return quest;
        }
    }
}
