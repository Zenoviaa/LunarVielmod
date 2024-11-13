using Terraria.UI;

namespace Stellamod.UI.CellConverterSystem
{
    internal class ConverterUIState : UIState
    {
        public ConverterUI converterUI;
        public ConverterUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            converterUI = new ConverterUI();
            Append(converterUI);
        }
    }
}
