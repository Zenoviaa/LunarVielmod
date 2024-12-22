using Terraria.UI;

namespace Stellamod.UI.AdvancedMagicSystem
{
    internal class StaffUIState : UIState
    {
        public AdvancedMagicStaffUI staffUI;
        public AdvancedMagicElementUI elementUI;
        public StaffUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            staffUI = new AdvancedMagicStaffUI();
            Append(staffUI);

            elementUI = new AdvancedMagicElementUI();
            Append(elementUI);
        }
    }

    internal class ItemUIState : UIState
    {
        public AdvancedMagicItemUI itemUI;
        public ItemUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            itemUI = new AdvancedMagicItemUI();
            Append(itemUI);
        }
    }

    internal class ButtonUIState : UIState
    {
        private AdvancedMagicButtonUI _buttonUI;
        public ButtonUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            _buttonUI = new AdvancedMagicButtonUI();
            Append(_buttonUI);
        }


    }
}
