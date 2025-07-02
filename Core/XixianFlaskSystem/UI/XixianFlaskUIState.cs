using Terraria.UI;

namespace Stellamod.Core.XixianFlaskSystem.UI
{
    internal class XixianFlaskUIState : UIState
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
