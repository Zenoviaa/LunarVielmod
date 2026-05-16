using Terraria.UI;

namespace Stellamod.Common.ArmorRework
{
    public class ArmorTooltipUIState : UIState
    {
        public ArmorInspectorUI inspectorUI;
        public ArmorTooltipUIState() : base()
        {
            inspectorUI = new();
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Append(inspectorUI);
        }
    }
}
