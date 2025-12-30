using Terraria.UI;

namespace Stellamod.Common.SummonerSystem.UI
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
