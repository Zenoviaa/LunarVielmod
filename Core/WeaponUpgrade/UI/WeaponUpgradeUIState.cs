using Terraria.UI;

namespace Stellamod.Core.WeaponUpgrade.UI
{
    internal class WeaponUpgradeUIState : UIState
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
