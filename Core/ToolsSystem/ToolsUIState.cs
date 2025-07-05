using Terraria.UI;

namespace Stellamod.Core.ToolsSystem
{
    internal class ToolsUIState : UIState
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
