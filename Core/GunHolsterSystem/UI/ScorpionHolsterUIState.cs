using Terraria.UI;

namespace Stellamod.Core.GunHolsterSystem.UI
{
    internal class ScorpionHolsterUIState : UIState
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
