using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace Stellamod.UI.GunHolsterSystem
{
    internal class ScorpionHolsterUIState : UIState
    {
        public ScorpionHolsterUI ui;
        public ScorpionHolsterUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new ScorpionHolsterUI();
            Append(ui);
        }
    }
}
