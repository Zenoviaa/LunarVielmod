using Terraria.UI;

namespace Stellamod.Core.CollectionSystem
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
