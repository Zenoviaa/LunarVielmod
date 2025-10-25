using Terraria.UI;

namespace Stellamod.Core.SummonerSystem.UI
{
    public class BellUIState : UIState
    {
        public BellUI bellUI;
        public BellUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            bellUI = new BellUI();
            Append(bellUI);
        }
    }
}
