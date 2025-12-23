using Terraria.UI;

namespace Stellamod.Core.BlackSystem
{
    public class BlackUIState : UIState
    {
        public BlackUI ui;
        public BlackUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new BlackUI();
            Append(ui);
        }
    }
}
