using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.QuestSystem
{
    public class QuestSerializer : TagSerializer<Quest, TagCompound>
    {
        public override TagCompound Serialize(Quest value) => new TagCompound
        {
            ["type"] = value.Type,
        };

        public override Quest Deserialize(TagCompound tag)
        {
            Quest quest = QuestLoader.GetQuest(tag.GetInt("type"));
            return quest;
        }
    }

    public abstract class Quest : ModType, ILocalizedModType
    {
        private List<Item> _rewards;
        public string LocalizationCategory => "Quests";
        public LocalizedText DisplayName { get; private set; }
        public LocalizedText Description { get; private set; }
        public LocalizedText Objective { get; private set; }
        public virtual string IconTexture => (GetType().Namespace + "." + Name).Replace('.', '/');
        public virtual string BigTexture => IconTexture + "_Big";
        public int Type { get; internal set; }
        public bool IsSideQuest { get; set; }
        public bool IsAutoQuest { get; set; }
        public List<Item> Rewards
        {
            get
            {
                _rewards ??= new List<Item>();
                return _rewards;
            }
        }



        protected sealed override void Register()
        {
            ModTypeLookup<Quest>.Register(this);
            QuestLoader.RegisterQuest(this);
        }
        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName = this.GetLocalization("DisplayName");
            Description = this.GetLocalization("Description");
            Objective = this.GetLocalization("Objective");
        }
        public void AddReward(int itemId, int stack)
        {
            Item item = new Item(itemId);
            Item rewardItem = item.Clone();
            rewardItem.stack = stack;
            Rewards.Add(rewardItem);
        }
        public bool IsQuestAvailable(Player player)
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            if (questPlayer.HasActiveQuest(this) || questPlayer.HasCompletedQuest(this) || questPlayer.HasRewardQuest(this))
            {
                return false;
            }

            return CanGiveQuest(player);
        }

        /// <summary>
        /// Override this to determine when the quest should be available, defaults to true
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool CanGiveQuest(Player player)
        {
            return true;
        }

        /// <summary>
        /// Called when the quest is first given, you can do things like give starting quest items in here or something
        /// </summary>
        /// <param name="player"></param>
        public virtual void StartQuest(Player player)
        {

        }

        public virtual bool CheckCompletion(Player player) { return true; }

        public virtual void Reward(Player player)
        {
            foreach (var reward in Rewards)
            {
                player.QuickSpawnItem(player.GetSource_FromThis(), reward, reward.stack);
            }

            for (int i = 0; i < 32; i++)
            {
                float f = i;
                float num = 32;
                float p = f / num;
                float rot = p * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 3;
                Dust.NewDustPerfect(player.Center, DustID.GoldCoin, vel);
            }

        }

        public virtual void QuestIntroDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
        {

        }
    }
}
