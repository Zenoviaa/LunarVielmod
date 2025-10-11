using Terraria.UI;

namespace Stellamod.Core.StructureSelector
{
    public class StructureSelectorUIState : UIState
    {
        public StructureSelectorUI ui;
        public StructureSelectorUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new StructureSelectorUI();
            Append(ui);
        }
    }
}
