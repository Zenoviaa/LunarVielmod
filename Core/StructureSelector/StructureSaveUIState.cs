using Terraria.UI;

namespace Stellamod.Core.StructureSelector
{
    internal class StructureSaveUIState : UIState
    {
        public SaveStructureUI ui;
        public StructureSaveUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new SaveStructureUI();
            Append(ui);
        }
    }
}
