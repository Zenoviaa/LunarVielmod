using Terraria.UI;

namespace Stellamod.Core.TabletSystem
{
    internal class TabletUIState : UIState
    {
        public TabletUI tabletUI;
        public TabletUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            tabletUI = new TabletUI();
            Append(tabletUI);
        }
    }
}
