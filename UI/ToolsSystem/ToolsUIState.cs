using Terraria.UI;

namespace Stellamod.UI.ToolsSystem
{
    public class ToolsUIState : UIState
    {
        public ToolsUI ui;
        public ToolsUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new ToolsUI();
            Append(ui);
        }
    }
}
