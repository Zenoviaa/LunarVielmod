using Stellamod.Core.CollectionSystem.Medallion;
using Terraria.UI;

namespace Stellamod.Core.CollectionSystem
{

    internal class FragmentDescriptionTabUIState : UIState
    {
        public DescriptionPageUI ui;
        public FragmentDescriptionTabUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new DescriptionPageUI();
            Append(ui);
        }
    }


    internal class FragmentsTabUIState : UIState
    {
        public MedallionPageUI ui;
        public FragmentsTabUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new MedallionPageUI();
            Append(ui);
        }
    }


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
