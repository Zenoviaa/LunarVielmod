using Terraria.UI;

namespace Stellamod.Common.ClassReworkSystem.AmmoRework.UI;

public class AmmoToolUIState : UIState
{
    public AmmoToolSlotPanel panel;
    public AmmoToolUIState() : base()
    {

    }

    public override void OnInitialize()
    {
        panel = new();
        Append(panel);
    }
}
