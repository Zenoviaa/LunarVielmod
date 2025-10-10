using Stellamod.UI.ToolsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace Stellamod.UI.StructureSelector
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
