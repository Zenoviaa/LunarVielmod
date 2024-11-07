using Terraria.UI;

namespace Stellamod.UI.CollectionSystem
{
    internal class CollectionBookUIState : UIState
    {
        public CollectionBookUI bookUI;
        public CollectionBookUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            bookUI = new CollectionBookUI();
            Append(bookUI);
        }
    }
}
