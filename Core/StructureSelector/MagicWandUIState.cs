﻿using Terraria.UI;

namespace Stellamod.Core.StructureSelector
{
    internal class MagicWandUIState : UIState
    {
        public MagicWandUI ui;
        public MagicWandUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new MagicWandUI();
            Append(ui);
        }
    }
}
