using Terraria.UI;

namespace Stellamod.Common.WeaponTypes.CombatTools.UI
{
    #region UI
    public class CombatToolMeunUIState : UIState
    {
        public CombatToolBrowserWindow xixianFlaskUI;
        public CombatToolMeunUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            xixianFlaskUI = new CombatToolBrowserWindow();
            Append(xixianFlaskUI);
        }
    }
    #endregion
}
