using Terraria.UI;

namespace Stellamod.UI.PowderSystem
{
    public class PowderUIState : UIState
    {
        public PowderUI powderUI;
        public PowderUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            powderUI = new PowderUI();
            Append(powderUI);
        }
    }
}
