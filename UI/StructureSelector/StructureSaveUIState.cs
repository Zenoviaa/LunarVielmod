using Terraria.UI;

namespace Stellamod.UI.StructureSelector
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
