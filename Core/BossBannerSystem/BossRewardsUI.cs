using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.Materials;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.BossBannerSystem
{


    /// <summary>
    /// Draws the rewards that a boss hass
    /// </summary>
    public class BossRewardsUI : UIPanel
    {
        private BossPageRewardType _rewardsToShow;
        private readonly int _rewardContext;
        private readonly BossPageUI _parent;
        private BossPage _bossPage;
        private UIText _difficultyText;
        public BossRewardsUI(BossPageUI parent)
        {
            _parent = parent;
            _rewardContext = ItemSlot.Context.BankItem;
            _difficultyText = new UIText("Rewards");
        }

     
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width = _parent.Width;
            Height.Pixels = 32;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Append(_difficultyText);
        }

        public void SetBossPage(BossPage bossPage)
        {
            _bossPage = bossPage;
        }


        private string GetRewardsText(BossPageRewardType rewardType)
        {
            string text = LangText.BossBanners("RewardsNormal");
            switch (rewardType)
            {
                case BossPageRewardType.NoHitRewards:
                    text = LangText.BossBanners("RewardsNoHit");
                    break;
                case BossPageRewardType.MasterModeRewards:
                    text = LangText.BossBanners("RewardsMaster");
                    break;

            }
            return text;
        }
        public void SetRewardsToShow(BossPageRewardType rewardType)
        {
            _rewardsToShow = rewardType;
            _difficultyText.SetText(GetRewardsText(rewardType));
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            DrawRewards(spriteBatch);
        }

      
        private void DrawRewards(SpriteBatch spriteBatch)
        {
            _difficultyText.Top.Pixels = -48;
            if (_parent.Page != 2)
            {
                _difficultyText.SetText(string.Empty);
                return;
            }


            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            List<Item> rewards = _bossPage.GetRewards(_rewardsToShow);
            if(rewards.Count == 0)
            {
                for(int i =0; i < 7; i++)
                {
                    var ivythorn = ModContent.GetInstance<Ivythorn>();
                    rewards.Add(ivythorn.Item);
                }
            }
            for (int i = 0; i < rewards.Count; i++)
            {
                Item reward = rewards[i];
                float distanceBetween = 32;
                Vector2 drawPosition = topLeft + new Vector2(distanceBetween * i, 0);
                ItemSlot.DrawItemIcon(reward, _rewardContext, spriteBatch, drawPosition, 1, 32, Color.White);
            }
        }
    }
}
