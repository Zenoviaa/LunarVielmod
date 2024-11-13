using Terraria.UI;

namespace Stellamod.UI.XixianFlaskSystem
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
