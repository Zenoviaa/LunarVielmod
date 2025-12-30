using Terraria.UI;

namespace Stellamod.Common.HealthbarSystem
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
