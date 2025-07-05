using Terraria.UI;

namespace Stellamod.Core.Map.UI
{
    internal class MapIconUIState : UIState
    {
        public MapIconUI ui;
        public MapIconUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new MapIconUI();
            Append(ui);
        }
    }
}
