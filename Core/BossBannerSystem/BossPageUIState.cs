using Terraria.UI;

namespace Stellamod.Core.BossBannerSystem
{
    public class BossPageUIState : UIState
    {
        public BossPageUI ui;
        public BossPageUIState() : base()
        {

        }
        public override void OnInitialize()
        {
            ui = new BossPageUI();
            Append(ui);
        }
    }
}
