using Microsoft.Xna.Framework;
using Stellamod.Content.Quests.ZuiQuest;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.QuestSystem
{
    public class QuestSerializer : TagSerializer<Quest, TagCompound>
    {
        public override TagCompound Serialize(Quest value) => new TagCompound
        {
            ["quest"] = value.Name,
        };

        public override Quest Deserialize(TagCompound tag)
        {
            string type = tag.GetString("quest");
            if(ModContent.TryFind<Quest>(Stellamod.Instance.Name, type, out Quest quest))
            {
                return quest;
            }
            
            //Failsafe
            return ModContent.GetInstance<TalkToZui>();
        }
    }

    public abstract class Quest : ModType,
        ILocalizedModType
    {
        private List<Item> _rewards;
        public string DisplayName
        {
            get
            {
                return LangText.Quest(this, "DisplayName");
            }
        }

        public string Description
        {
            get
            {
                return LangText.Quest(this, "Description");
            }
        }

        public string Objective
        {
            get
            {
                return LangText.Quest(this, "Objective");
            }
        }
        public string IntroText
        {
            get
            {
                return LangText.Quest(this, "Intro");
            }
        }
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

        public string LocalizationCategory => "Quests";
        protected sealed override void Register()
        {
            ModTypeLookup<Quest>.Register(this);
           
        }
        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
            this.GetLocalization(nameof(DisplayName), () => "");
            this.GetLocalization(nameof(Description), () => "");
            this.GetLocalization(nameof(Objective), () => "");
            this.GetLocalization(nameof(IntroText), () => "");
        }

        public override void Unload()
        {
            base.Unload();
            _rewards = null;
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
            SoundStyle sound = new SoundStyle("Stellamod/Assets/Sounds/Bliss2");
            SoundEngine.PlaySound(sound);

        }

        public virtual bool CheckCompletion(Player player) { return true; }

        public virtual void Reward(Player player)
        {
            foreach (var reward in Rewards)
            {
                player.QuickSpawnItem(player.GetSource_FromThis(), reward, reward.stack);
            }
            SoundStyle questCompleteSound = new SoundStyle("Stellamod/Assets/Sounds/Bliss1");
            SoundEngine.PlaySound(questCompleteSound);
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
            text = IntroText;
        }

        public static bool HasCompletedQuest<T>(Player player) where T : Quest
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            return questPlayer.HasFinishedQuest(ModContent.GetInstance<T>());
        }
    }
}
