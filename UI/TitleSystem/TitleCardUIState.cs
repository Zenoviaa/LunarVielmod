using Terraria.UI;


namespace Stellamod.UI.TitleSystem
{
    public class TitleCardUIState : UIState
    {
        public TitleCardUI titleCardUI;
        public TitleCardUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            titleCardUI = new TitleCardUI();
            Append(titleCardUI);
        }
    }
}
