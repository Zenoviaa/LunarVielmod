using Terraria.UI;

namespace Stellamod.Common.WeaponUpgrade.UI
{
    public class WeaponUpgradeUIState : UIState
    {
        public WeaponUpgradeUI ui;
        public WeaponUpgradeUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new WeaponUpgradeUI();
            Append(ui);
        }
    }
}
