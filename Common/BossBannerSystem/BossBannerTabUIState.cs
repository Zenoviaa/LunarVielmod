using Stellamod.UI.CollectionSystem;
using Terraria.UI;

namespace Stellamod.Common.BossBannerSystem
{
    public class BossBannerTabUIState : UIState
    {
        private BossPageUI _pageUI;
        public BossTabUI ui;
        public BossBannerTabUIState(BossPageUI pageUI) : base()
        {
            _pageUI  = pageUI;
        }

        public override void OnInitialize()
        {
            ui = new BossTabUI(_pageUI);
            Append(ui);
        }
    }
}
