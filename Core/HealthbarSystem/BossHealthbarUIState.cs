using Terraria.UI;

namespace Stellamod.Core.HealthbarSystem
{
    public class BossHealthbarUIState : UIState
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
