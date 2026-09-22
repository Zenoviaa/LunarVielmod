using Stellamod.UI;
using Stellamod.UI.CollectionSystem;
using Terraria.UI;

namespace Stellamod.Common.BossBannerSystem
{
    public class BossBannerTabUIState : UIState
    {
        private BossPageUI _pageUI;
        private FancyScrollbar _scrollbar;
        public BossTabUI ui;
        public BossBannerTabUIState(BossPageUI pageUI) : base()
        {
            _pageUI  = pageUI;
        }

        public override void OnInitialize()
        {
            _scrollbar = new();
            ui = new BossTabUI(_pageUI, _scrollbar);
            Append(ui);
            Append(_scrollbar);
        }
    }
}
