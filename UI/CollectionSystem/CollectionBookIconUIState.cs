using Terraria.UI;

namespace Stellamod.UI.CollectionSystem
{
    public class CollectionBookIconUIState : UIState
    {
        public CollectionBookIconUI bookIconUI;
        public CollectionBookIconUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            bookIconUI = new CollectionBookIconUI();
            Append(bookIconUI);
        }
    }
}
