using Terraria.UI;

namespace Stellamod.Core.ArmorReforge.UI
{
    internal class ReforgeUIState : UIState
    {
        public ReforgeUI ui;
        public ReforgeUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new ReforgeUI();
            Append(ui);
        }
    }
}
