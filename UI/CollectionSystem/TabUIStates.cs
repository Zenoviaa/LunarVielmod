using Stellamod.UI.CollectionSystem.Quests;
using Terraria;
using Terraria.UI;

namespace Stellamod.UI.CollectionSystem
{




    internal class CollectionItemTabRecipeUIState : UIState
    {
        public CollectionItemRecipesUI ui;
        public CollectionItemTabRecipeUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new CollectionItemRecipesUI();
            Append(ui);
        }
    }

    internal class CollectionItemTabUIState : UIState
    {
        public CollectionItemTabUI ui;
        public CollectionItemTabUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new CollectionItemTabUI();
            Append(ui);
        }
    }
}
