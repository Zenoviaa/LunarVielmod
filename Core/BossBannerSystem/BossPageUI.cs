using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
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
        }

        public void SetBossPage(BossPage bossPage)
        {
            BossPage = bossPage;
            _bossStarsUI.SetBossPage(bossPage);
            _bossRewardsUI.SetBossPage(bossPage);
            _bossPhotoUI.SetBossPage(bossPage);
            _displayNameText.SetText(bossPage.DisplayName);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

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
