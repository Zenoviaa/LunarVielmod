using Stellamod.Core.QuestSystem;
using System.Collections.Generic;

namespace Stellamod.UI.CollectionSystem.Quests
{
    public class QuestComparer : IComparer<Quest>
    {
        public int Compare(Quest x, Quest y)
        {
         return x.DisplayName.CompareTo(y.DisplayName);
        }
    }
}
