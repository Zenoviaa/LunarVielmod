using Stellamod.Core.UI;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.Map.UI
{
    internal class MapIcon : UIButtonIcon
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            MapUISystem uiSystem = ModContent.GetInstance<MapUISystem>();
            uiSystem.ToggleUI();
        }
    }
}
