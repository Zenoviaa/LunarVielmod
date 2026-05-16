using Terraria.UI;

namespace Stellamod.UI.CollectionSystem
{




    public class CollectionArmorUIState : UIState
    {
        public CollectionArmorInfoUI ui;
        public CollectionArmorUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new CollectionArmorInfoUI();
            Append(ui);
        }
    }


    public class CollectionItemTabRecipeUIState : UIState
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

    public class CollectionItemTabUIState : UIState
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
