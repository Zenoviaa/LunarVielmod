using Terraria.UI;

namespace Stellamod.Core.XixianFlaskSystem.UI
{
    public class XixianFlaskUIState : UIState
    {
        public XixianFlaskUI xixianFlaskUI;
        public XixianFlaskUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            xixianFlaskUI = new XixianFlaskUI();
            Append(xixianFlaskUI);
        }
    }
}
