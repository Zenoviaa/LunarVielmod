using Stellamod.Core.GunHolsterSystem.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace Stellamod.Core.HealthbarSystem
{
    internal class BossHealthbarUIState : UIState
    {
        public BossHealthbarUI ui;
        public BossHealthbarUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new();
            Append(ui);
        }
    }
}
