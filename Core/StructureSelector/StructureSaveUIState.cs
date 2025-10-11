using Terraria.UI;

namespace Stellamod.Core.StructureSelector
{
    public class StructureSaveUIState : UIState
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
