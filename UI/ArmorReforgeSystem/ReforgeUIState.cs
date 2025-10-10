using Terraria.UI;

namespace Stellamod.UI.ArmorReforgeSystem
{
    public class ReforgeUIState : UIState
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
