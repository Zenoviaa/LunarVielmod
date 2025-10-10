using Terraria.UI;

namespace Stellamod.UI.GunHolsterSystem
{
    public class ScorpionHolsterUIState : UIState
    {
        public ScorpionHolsterUI ui;
        public ScorpionHolsterUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new ScorpionHolsterUI();
            Append(ui);
        }
    }
}
