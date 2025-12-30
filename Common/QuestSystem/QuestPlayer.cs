using Stellamod.Content.Quests.ZuiQuest;
using Stellamod.UI.PopupSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.QuestSystem
{
    public class QuestTracker : ModSystem
    {
        public Quest[] quests;
        public override void OnModLoad()
        {
            base.OnModLoad();
            quests = ModContent.GetContent<Quest>().ToArray();
        }
    }

    public class QuestPlayer : ModPlayer
    {
        private List<Quest> _activeQuests;
        private List<Quest> _completedQuests;
        private List<Quest> _rewardQuests;
        public List<Quest> ActiveQuests
        {
            get
            {
                _activeQuests ??= new List<Quest>();
                _activeQuests.Sort((x, y) => x.IsSideQuest.CompareTo(y.IsSideQuest));
                return _activeQuests;
            }
            private set
            {
                _activeQuests = value;
            }
        }

        public List<Quest> CompletedQuests
        {
            get
            {
                _completedQuests ??= new List<Quest>();
                return _completedQuests;
            }
            private set
            {
                _completedQuests = value;
            }
        }

        public List<Quest> RewardQuests
        {
            get
            {
                _rewardQuests ??= new List<Quest>();
                return _rewardQuests;
            }
            private set
            {
                _rewardQuests = value;
            }
        }

        public bool RecalculateUI { get; set; }

        public bool FreshQuests()
        {
            int totalQuestCount = ActiveQuests.Count + CompletedQuests.Count + RewardQuests.Count;
            return totalQuestCount <= 0;
        }


        public override void ResetEffects()
        {
            base.ResetEffects();


        }


        public bool HasActiveQuest(Quest quest)
        {
            return ActiveQuests.Contains(quest);
        }
        public bool HasCompletedQuest(Quest quest)
        {
            return CompletedQuests.Contains(quest);
        }
        public bool HasRewardQuest(Quest quest)
        {
            return RewardQuests.Contains(quest);
        }
        public bool HasFinishedQuest(Quest quest)
        {
            return CompletedQuests.Contains(quest) || RewardQuests.Contains(quest);
        }
        public bool GiveQuest(Quest quest)
        {
            if (HasActiveQuest(quest) || HasCompletedQuest(quest) || HasRewardQuest(quest))
                return false;
            if (!quest.CanGiveQuest(Player))
                return false;

            ActiveQuests.Add(quest);
            quest.StartQuest(Player);
            if(Main.netMode != NetmodeID.Server)
            {
                PopupUISystem popupUISystem = ModContent.GetInstance<PopupUISystem>();
                popupUISystem.OpenUI("NewQuest");
            }
       
            RecalculateUI = true;
            return true;
        }

        public void CompleteQuest(Quest quest)
        {
            if (!ActiveQuests.Contains(quest))
                return;
            if (RewardQuests.Contains(quest))
                return;
            if (CompletedQuests.Contains(quest))
                return;

            ActiveQuests.Remove(quest);
            RewardQuests.Add(quest);
            if (Main.netMode != NetmodeID.Server)
            {
                PopupUISystem popupUISystem = ModContent.GetInstance<PopupUISystem>();
                popupUISystem.OpenUI("CompleteQuest");
            }
            RecalculateUI = true;
        }

        public void CollectQuestReward(Quest quest)
        {
            if (!RewardQuests.Contains(quest))
                return;
            if (CompletedQuests.Contains(quest))
                return;

            CompletedQuests.Add(quest);
            RewardQuests.Remove(quest);
            quest.Reward(Player);
            RecalculateUI = true;
        }

        private bool ShouldGrantQuest(Quest quest)
        {
            if (!quest.IsAutoQuest)
                return false;
            if (HasFinishedQuest(quest))
                return false;
            if (!quest.CanGiveQuest(Player))
                return false;
            return true;
        }

        private bool ShouldCompleteQuest(Quest quest)
        {
            if (!ActiveQuests.Contains(quest))
                return false;
            return quest.CheckCompletion(Player);
        }
        public override void PostUpdate()
        {
            base.PostUpdate();
 
      
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
   
            if (Player.dead)
                return;
            if (Main.GameUpdateCount % 30 == 0)
            {
                CheckQuestProgression();
            }
        }
        private void CheckQuestProgression()
        {
            //Very first quest that you start off with
            if (FreshQuests())
            {
                GiveQuest(ModContent.GetInstance<TalkToZui>());
            }
            QuestTracker tracker = ModContent.GetInstance<QuestTracker>();
            Quest[] quests = tracker.quests;
            for (int i = 0; i < quests.Length; i++)
            {
                Quest quest = quests[i];
                if (ShouldGrantQuest(quest))
                {
                    GiveQuest(quest);
                }
                if (ShouldCompleteQuest(quest))
                {
                    CompleteQuest(quest);
                    ActiveQuests.Remove(quest);
                }
            }
        }
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["activeQuests"] = ActiveQuests;
            tag["completedQuests"] = CompletedQuests;
            tag["rewardQuests"] = RewardQuests;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            ActiveQuests = tag.Get<List<Quest>>("activeQuests");
            CompletedQuests = tag.Get<List<Quest>>("completedQuests");
            RewardQuests = tag.Get<List<Quest>>("rewardQuests");
        }
    }
}
