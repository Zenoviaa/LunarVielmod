using Terraria.UI;

namespace Stellamod.Common.WeaponTypes.CombatTools.UI;

public class CombatToolUIState : UIState
{
    public CombatToolSlotPanel panel;
    public CombatToolUIState() : base()
    {

    }

    public override void OnInitialize()
    {
        panel = new();
        Append(panel);
    }
}

