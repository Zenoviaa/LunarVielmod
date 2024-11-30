using Terraria.UI;

namespace Stellamod.UI.ArmorReforgeSystem
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
