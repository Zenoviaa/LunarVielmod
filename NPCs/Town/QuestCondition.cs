using Stellamod.Common.QuestSystem;
using Stellamod.Content.Quests.OldManQuest;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Town
{
    public class QuestCondition
    {
        public static readonly Condition CompletedCollectFlowersI = new Condition("Conditions.CompleteCollectFlowersI",
            () => Main.LocalPlayer.GetModPlayer<QuestPlayer>().HasCompletedQuest(ModContent.GetInstance<CollectFlowersI>()));
    }
}
