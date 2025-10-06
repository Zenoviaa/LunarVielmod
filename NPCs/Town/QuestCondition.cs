using Stellamod.Core.QuestSystem;
using Stellamod.Core.QuestSystem.Quests.OldManQuest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
