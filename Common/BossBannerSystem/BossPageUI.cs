using Microsoft.Xna.Framework;
using Stellamod.Common.UI;
using Stellamod.Helpers;
using Stellamod.UI;
using Stellamod.UI.CollectionSystem;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.BossBannerSystem
{
    /// <summary>
    /// Opens a window that shows all information about the boss
    /// </summary>
    public class BossPageUI : RightPageUI
    {
        private CommonClaimButton _claimButton;
        private UIText _pageText;
        private UIText _displayNameText;
        private UIPanel _panel;
        private FancyScrollbar _scrollbar;
        private UIList _uiList;
        private BossFindButtonUI _glassUI;
        private BossLoreButtonUI _bossLoreUI;
        private BossPhotoUI _bossPhotoUI;
        private BossRewardsUI _bossRewardsUI;
        private BossStarsUI _bossStarsUI;
        private BossRewardsButtonUI _bossRewardsButton;
        public BossPageUI()
        {
            _claimButton = new CommonClaimButton(ClaimRewards);
            _pageText = new UIText("Find Your Mom");
            _displayNameText = new UIText("Your Mom");
            _glassUI = new BossFindButtonUI(this);
            _bossLoreUI = new BossLoreButtonUI(this);
            _bossPhotoUI = new BossPhotoUI(this);
            _bossRewardsUI = new BossRewardsUI(this);
            _bossStarsUI = new BossStarsUI(this);
            _bossRewardsButton = new BossRewardsButtonUI(this);
            _panel = new UIPanel();
            _scrollbar = new FancyScrollbar();
            _uiList = new UIList();
        }

        public BossPage BossPage { get; private set; }
        public BossPageRewardType RewardType { get; private set; }
        public int Page { get; private set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = GetPageWidth();
            Height.Pixels = GetPageHeight();
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _displayNameText.Height.Pixels = Height.Pixels;
            _displayNameText.Width.Pixels = Width.Pixels;
            _displayNameText.IsWrapped = true;
            _displayNameText.ShadowColor = Color.Black;

            _pageText.Height.Pixels = 32;
            _pageText.Width.Pixels = Width.Pixels;
            _pageText.IsWrapped = true;
            _pageText.ShadowColor = Color.Black;

            _panel.Append(_displayNameText);
            _panel.Append(_bossLoreUI);
            _panel.Append(_glassUI);
            _panel.Append(_bossPhotoUI);
            _panel.Append(_bossRewardsUI);
            _panel.Append(_bossStarsUI);
            _panel.Append(_bossRewardsButton);

            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(150, 0);
            _scrollbar.Left.Set(0, 0.98f);
            _scrollbar.Top.Set(0, 0.6f);

            float maxViewSize = 48 * 8f;
            _scrollbar.SetView(0, maxViewSize);
            Append(_scrollbar);


            _uiList = new UIList();
            _uiList.Top.Pixels = 260;
            _uiList.Width.Pixels = Width.Pixels;
            _uiList.Height.Pixels = 140;
            _uiList.Add(_pageText);
            _uiList.SetScrollbar(_scrollbar);
            Append(_uiList);
            Append(_claimButton);
        }

        public void SetBossPage(BossPage bossPage)
        {
            BossPage = bossPage;
            _bossStarsUI.SetBossPage(bossPage);
            _bossRewardsUI.SetBossPage(bossPage);
            _bossPhotoUI.SetBossPage(bossPage);
            _pageText.SetText(bossPage.WhereToFind);
            bool isHidden = bossPage.IsHidden();
            if (isHidden)
            {
                _displayNameText.SetText("???", 1.35f, false);
            } else
            {
                _displayNameText.SetText(bossPage.DisplayName, 1.35f, false);
            }
        }

        private void UpdateClaimButton()
        {
            _claimButton.Left.Pixels = Width.Pixels / 2 - _claimButton.Width.Pixels / 2 - 32;
            _claimButton.Top.Pixels = Height.Pixels - _claimButton.Height.Pixels - 100;

            _claimButton.alreadyClaimed = false;
            _claimButton.notBeaten = true;
            _claimButton.canClaim = false;
            _claimButton.notClaimed = false;


            //idk why im confusing myself with this, this should be so simple.
            if (HasAlreadyClaimed())
            {
                _claimButton.alreadyClaimed = true;
            }

            if (CanClaimRewards())
            {
                _claimButton.notBeaten = false;
            }

            if (!_claimButton.notBeaten && !HasAlreadyClaimed())
            {
                _claimButton.canClaim = true;
            }


            if (Page != 2)
                _claimButton.Top.Pixels = 99999;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            UpdateClaimButton();

            _bossPhotoUI.Top.Pixels = 32;
            _bossPhotoUI.Left.Pixels = 0;

            _displayNameText.Top.Pixels = 8;
            _glassUI.Top.Pixels = 210;
            _bossLoreUI.Top.Pixels = _glassUI.Top.Pixels;
            _bossLoreUI.Left.Pixels = _glassUI.Left.Pixels + 48;

            _bossRewardsButton.Top.Pixels = _bossLoreUI.Top.Pixels;
            _bossRewardsButton.Left.Pixels = _bossLoreUI.Left.Pixels + 32;

            int width = BossPage.RequestBossIcon().Width();
            _bossStarsUI.Left.Pixels = width + 4;
            _bossStarsUI.Top.Pixels = 48;

            //rewards
            _bossRewardsUI.Top.Pixels = 380;
            _bossRewardsUI.Left.Pixels = 16;
    
            float listHeight = _uiList.GetTotalHeight();
            _pageText.Height.Pixels = 32;
            _pageText.Top.Pixels = 0;

            if (listHeight < _uiList.Height.Pixels)
            {
                _scrollbar.Top.Set(500000, 0f);
            }
            else
            {
                _scrollbar.Top.Set(0, 0.6f);
            }
    
        }


        public void ToggleLocationWindow(UIMouseEvent evt, UIElement listeningElement)
        {
            _pageText.SetText(BossPage.WhereToFind);
            Page = 0;
        }

        public void ToggleLoreWindow(UIMouseEvent evt, UIElement listeningElement)
        {
            _pageText.SetText(BossPage.Lore);
            if (BossPage.IsHidden())
            {
                _pageText.SetText(string.Empty);
            }

            Page = 1;
        }


        public bool HasAlreadyClaimed()
        {
            DownedBossRewardPlayer rewardPlayer = Main.LocalPlayer.GetModPlayer<DownedBossRewardPlayer>();
            int flag = (int)BossPage.flag;
            switch (RewardType)
            {
                default:
                case BossPageRewardType.Rewards:
                    return rewardPlayer.claimedRegularRewards[flag];
                case BossPageRewardType.MasterModeRewards:
                    return rewardPlayer.claimedMasterRewards[flag];
                case BossPageRewardType.NoHitRewards:
                    return rewardPlayer.claimedNoHit[flag];
            }
        }
        public bool CanClaimRewards()
        {
            int flag = (int)BossPage.flag;
            switch (RewardType)
            {
                default:
                case BossPageRewardType.Rewards:
                    return BossPage.CanClaimRewards();
                case BossPageRewardType.MasterModeRewards:
                    return BossPage.CanClaimMasterRewards();
                case BossPageRewardType.NoHitRewards:
                    return BossPage.CanClaimNoHitRewards();
            }
        }
        public void ClaimRewards()
        {
            if (!CanClaimRewards())
                return;
            if (HasAlreadyClaimed())
                return;

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Harv1"));
            DownedBossRewardPlayer rewardPlayer = Main.LocalPlayer.GetModPlayer<DownedBossRewardPlayer>();
            int flag = (int)BossPage.flag;
            switch (RewardType)
            {
                case BossPageRewardType.Rewards:
                    BossPage.Grant(BossPage.Rewards);
                    rewardPlayer.claimedRegularRewards[flag] = true;
                    break;
                case BossPageRewardType.MasterModeRewards:
                    BossPage.Grant(BossPage.MasterModeRewards);
                    rewardPlayer.claimedMasterRewards[flag] = true;
                    break;
                case BossPageRewardType.NoHitRewards:
                    BossPage.Grant(BossPage.NoHitRewards);
                    rewardPlayer.claimedNoHit[flag] = true;
                    break;
            }
        }
        public void CycleRewardsType(UIMouseEvent evt, UIElement listeningElement)
        {
            if (string.IsNullOrEmpty(_pageText.Text))
            {
                int index = (int)RewardType;
                index++;
                if (index >= 3)
                {
                    index = 0;
                }
                RewardType = (BossPageRewardType)index;
            }
            _pageText.SetText(string.Empty);
            _bossRewardsUI.SetRewardsToShow(RewardType);
            Page = 2;
        }
    }
}
