using Microsoft.Xna.Framework;
using Stellamod.UI.CollectionSystem;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.BossBannerSystem
{
    /// <summary>
    /// Opens a window that shows all information about the boss
    /// </summary>
    public class BossPageUI : RightPageUI
    {
        private UIText _displayNameText;
        private BossFindButtonUI _glassUI;
        private BossLoreButtonUI _bossLoreUI;
        private BossPhotoUI _bossPhotoUI;
        private BossRewardsUI _bossRewardsUI;
        private BossStarsUI _bossStarsUI;
        public BossPageUI()
        {
            _displayNameText = new UIText("Your Mom");
            _glassUI = new BossFindButtonUI(this);
            _bossLoreUI = new BossLoreButtonUI(this);
            _bossPhotoUI = new BossPhotoUI(this);
            _bossRewardsUI = new BossRewardsUI(this);
            _bossStarsUI = new BossStarsUI(this);
        }

        public BossPage BossPage { get; private set; }
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

            Append(_displayNameText);
            Append(_bossLoreUI);
            Append(_glassUI);
            Append(_bossPhotoUI);
            Append(_bossRewardsUI);
            Append(_bossStarsUI);
        }

        public void SetBossPage(BossPage bossPage)
        {
            BossPage = bossPage;
            _bossStarsUI.SetBossPage(bossPage);
            _bossRewardsUI.SetBossPage(bossPage);
            _bossPhotoUI.SetBossPage(bossPage);
            _displayNameText.SetText(bossPage.DisplayName, 1.35f, false);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;


            _bossPhotoUI.Top.Pixels = 32;
            _bossPhotoUI.Left.Pixels = 0;

            _displayNameText.Top.Pixels = 8;
            _glassUI.Top.Pixels = 220;
            _bossLoreUI.Top.Pixels = _glassUI.Top.Pixels;
            _bossLoreUI.Left.Pixels = _glassUI.Left.Pixels + 48;

            int width = BossPage.RequestBossIcon().Width();
            _bossStarsUI.Left.Pixels = width + 4;
            _bossStarsUI.Top.Pixels = 48;

            //rewards
            _bossRewardsUI.Top.Pixels = 380;
            _bossRewardsUI.Left.Pixels = 16;
        }


        public void ToggleLocationWindow(UIMouseEvent evt, UIElement listeningElement)
        {
         //   throw new NotImplementedException();
        }

        public void ToggleLoreWindow(UIMouseEvent evt, UIElement listeningElement)
        {
        //    throw new NotImplementedException();
        }
    }
}
