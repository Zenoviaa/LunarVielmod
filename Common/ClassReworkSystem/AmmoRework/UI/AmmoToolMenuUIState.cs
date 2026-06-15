using Terraria.UI;

namespace Stellamod.Common.ClassReworkSystem.AmmoRework.UI;

public class AmmoToolMenuUIState : UIState
{
    public AmmoToolBrowserWindow xixianFlaskUI;
    public AmmoToolMenuUIState() : base()
    {

    }

    public override void OnInitialize()
    {
        xixianFlaskUI = new AmmoToolBrowserWindow();
        Append(xixianFlaskUI);
    }
}
